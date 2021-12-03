using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Enums;
using CSharpCourse.Core.Lib.Events;
using MediatR;
using Microsoft.Extensions.Options;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationServices;
using OzonEdu.MerchandiseService.Infrastructure.Commands.SupplyEvent;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;

namespace OzonEdu.MerchandiseService.Infrastructure.Handlers.SupplyEvent
{
    public class ProcessSupplyEventCommandHandler : IRequestHandler<ProcessSupplyEventCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMerchPackItemRepository _merchPackItemRepository;
        private readonly IMerchRequestRepository _merchRequestRepository;
        private readonly IApplicationService _applicationService;
        private readonly IProducer<string, string> _producer;
        private readonly KafkaConfiguration _kafkaOptions;

        public ProcessSupplyEventCommandHandler(
            IUnitOfWork unitOfWork,
            IMerchPackItemRepository merchPackItemRepository,
            IMerchRequestRepository merchRequestRepository,
            IApplicationService applicationService,
            IProducer<string, string> producer,
            IOptions<KafkaConfiguration> kafkaOptions)
        {
            _unitOfWork = unitOfWork;
            _merchPackItemRepository = merchPackItemRepository;
            _merchRequestRepository = merchRequestRepository;
            _applicationService = applicationService;
            _producer = producer;
            _kafkaOptions = kafkaOptions.Value;
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
                    var message = new Message<string, string>
                    {
                        Key = employee.Id.ToString(),
                        Value = JsonSerializer.Serialize(new NotificationEvent
                        {
                            EmployeeName = employee.Name.ToString(),
                            EmployeeEmail = employee.Email.Value,
                            EventType = EmployeeEventType.MerchDelivery,
                            Payload = new MerchDeliveryEventPayload
                            {
                                MerchType = request.MerchType.ToMerchType(),
                                ClothingSize = ClothingSize.L
                            }
                        })
                    };
                    _producer.Produce(_kafkaOptions.EmployeeNotificationEventTopic, message);

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
                        await _unitOfWork.StartTransactionAsync(cancellationToken);
                        try
                        {
                            await _merchRequestRepository.UpdateAsync(request, cancellationToken);
                            await _unitOfWork.SaveChangesAsync(cancellationToken);
                        }
                        catch
                        {
                            await _unitOfWork.RollbackAsync(cancellationToken);
                            throw;
                        }
                    }
                }
            }

            return Unit.Value;
        }
    }
}