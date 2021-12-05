using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Enums;
using CSharpCourse.Core.Lib.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
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
        private readonly IMerchPackItemRepository _merchPackItemRepository;
        private readonly IMerchRequestRepository _merchRequestRepository;
        private readonly IApplicationService _applicationService;
        private readonly IProducer<string, string> _producer;
        private readonly KafkaConfiguration _kafkaOptions;
        private readonly ITracer _tracer;
        private readonly ILogger<ProcessSupplyEventCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;


        public ProcessSupplyEventCommandHandler(
            IMerchPackItemRepository merchPackItemRepository,
            IMerchRequestRepository merchRequestRepository,
            IApplicationService applicationService,
            IProducer<string, string> producer,
            IOptions<KafkaConfiguration> kafkaOptions,
            ITracer tracer,
            ILogger<ProcessSupplyEventCommandHandler> logger,
            IUnitOfWork unitOfWork)
        {
            _merchPackItemRepository = merchPackItemRepository;
            _merchRequestRepository = merchRequestRepository;
            _applicationService = applicationService;
            _producer = producer;
            _tracer = tracer;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _kafkaOptions = kafkaOptions.Value;
        }

        public async Task<Unit> Handle(ProcessSupplyEventCommand command, CancellationToken cancellationToken)
        {
            using var span = _tracer
                .BuildSpan($"{nameof(ProcessSupplyEventCommandHandler)}.{nameof(Handle)}")
                .StartActive();

            var skus = command.Items.Select(x => x.SkuId);
            var requestMerchTypes = await _merchPackItemRepository.FindMerchTypesBySkuAsync(skus, cancellationToken);

            var outOfStockMerchRequests =
                await _merchRequestRepository.FindOutOfStockByRequestMerchTypesAsync(
                    requestMerchTypes,
                    cancellationToken);

            await _unitOfWork.StartTransactionAsync(cancellationToken);
            try
            {
                foreach (var request in outOfStockMerchRequests)
                {
                    var employee =
                        await _applicationService.GetEmployee(request.EmployeeId.Value, null, cancellationToken);

                    // если сотрудник сам приходил за мерчом (вызов был через REST API) - просто отсылаем ему уведомление,
                    // что интересующий его мерч появился на остатках.
                    if (request.Mode.Equals(CreationMode.User) && !request.IsEmailSended)
                    {
                        var isAvailable = await _applicationService.IsAvailable(request, cancellationToken);
                        if (!isAvailable) continue;

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
                        try
                        {
                            await _producer.ProduceAsync(
                                _kafkaOptions.EmailNotificationEventTopic,
                                message,
                                cancellationToken);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e,
                                "Error while sending event {Event}. Message: {Message}",
                                nameof(NotificationEvent),
                                e.Message);
                            continue;
                        }

                        request.SendEmail();
                        await _applicationService.SaveRequest(request, cancellationToken);

                        continue;
                    }

                    // Если же сотруднику не хватила мерча при автоматической выдаче - пытаемся снова автоматически
                    // выдать мерч по сценарию
                    if (request.Mode.Equals(CreationMode.System))
                    {
                        var isComplete =
                            await _applicationService.CanCheckAndReserve(request, employee, cancellationToken);
                        if (isComplete)
                        {
                            request.Complete(Date.Create(DateTime.Now));
                            await _applicationService.SaveRequest(request, cancellationToken);
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while process transaction. Message: {Message}", e.Message);
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }


            return Unit.Value;
        }
    }
}