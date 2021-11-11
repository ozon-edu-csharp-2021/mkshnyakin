using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationServices;
using OzonEdu.MerchandiseService.Infrastructure.Commands.SupplyEvent;
using OzonEdu.MerchandiseService.Infrastructure.Contracts.MessageBus;

namespace OzonEdu.MerchandiseService.Infrastructure.Handlers.SupplyEvent
{
    public class ProcessSupplyEventCommandHandler : IRequestHandler<ProcessSupplyEventCommand>
    {
        private readonly IMerchPackItemRepository _merchPackItemRepository;
        private readonly IMerchRequestRepository _merchRequestRepository;
        private readonly IApplicationService _applicationService;
        private readonly IMessageBus _messageBus;

        public ProcessSupplyEventCommandHandler(
            IMerchPackItemRepository merchPackItemRepository,
            IMerchRequestRepository merchRequestRepository,
            IApplicationService applicationService,
            IMessageBus messageBus)
        {
            _merchPackItemRepository = merchPackItemRepository;
            _merchRequestRepository = merchRequestRepository;
            _applicationService = applicationService;
            _messageBus = messageBus;
        }

        public async Task<Unit> Handle(ProcessSupplyEventCommand command, CancellationToken cancellationToken)
        {
            var skus = command.Items.Select(x => x.SkuId);
            var requestMerchTypes = await _merchPackItemRepository.FindMerchTypesBySkuAsync(skus, cancellationToken);

            var outOfStockMerchRequests =
                await _merchRequestRepository.FindOutOfStockByRequestMerchTypesAsync(
                    requestMerchTypes,
                    cancellationToken);

            foreach (var request in outOfStockMerchRequests)
            {
                var employee = await _applicationService.GetEmployee(request.EmployeeId.Value, null, cancellationToken);

                // если сотрудник сам приходил за мерчом (вызов был через REST API) - просто отсылаем ему уведомление,
                // что интересующий его мерч появился на остатках.
                if (request.Mode.Equals(CreationMode.User))
                {
                    var employeeEmailMessage = new EmailMessage
                    {
                        ToEmail = employee.Email.Value,
                        ToName = employee.Name.ToString(),
                        Subject = "Мерч появился на остатках",
                        Body = string.Empty
                    };
                    _messageBus.Notify(employeeEmailMessage);

                    continue;
                }

                // Если же сотруднику не хватила мерча при автоматической выдаче - пытаемся снова автоматически
                // выдать мерч по сценарию
                if (request.Mode.Equals(CreationMode.System))
                {
                    var isComplete = await _applicationService.CheckAndReserve(request, employee, cancellationToken);
                    if (isComplete)
                    {
                        request.Complete(Date.Create(DateTime.Now));
                        await _merchRequestRepository.UpdateAsync(request, cancellationToken);
                    }
                }
            }

            return Unit.Value;
        }
    }
}