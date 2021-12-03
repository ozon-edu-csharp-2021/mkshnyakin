using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Enums;
using CSharpCourse.Core.Lib.Events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;

namespace OzonEdu.MerchandiseService.Infrastructure.HostedServices
{
    public class EmployeeNotificationEventConsumerBackgroundService : BackgroundService
    {
        private const string Topic = "employee_notification_event";

        private readonly KafkaConfiguration _kafkaConfiguration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmployeeNotificationEventConsumerBackgroundService> _logger;

        public EmployeeNotificationEventConsumerBackgroundService(
            IOptions<KafkaConfiguration> kafkaConfiguration,
            IServiceScopeFactory scopeFactory,
            ILogger<EmployeeNotificationEventConsumerBackgroundService> logger
        )
        {
            _kafkaConfiguration = kafkaConfiguration.Value;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "EmployeeNotificationEventConsumerBackgroundService: {@KafkaConfiguration}",
                _kafkaConfiguration);
            
            await Task.Yield();

            var config = new ConsumerConfig
            {
                GroupId = _kafkaConfiguration.EmployeeNotificationEventGroupId,
                BootstrapServers = _kafkaConfiguration.BootstrapServers,
            };

            using (var c = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                c.Subscribe(_kafkaConfiguration.EmployeeNotificationEventTopic);
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            try
                            {
                                await Task.Yield();
                                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                                var cr = c.Consume(stoppingToken);
                                if (cr != null)
                                {
                                    // Событие о принятии на работу сотрудника и об участии сотрудника в конференции
                                    // слушаем из кафка-топика employee_notification_event. 

                                    var message = JsonSerializer.Deserialize<NotificationEvent>(cr.Message.Value);
                                    if (message == null) continue;

                                    _logger.LogInformation("Event: {@Message}", message);
                                    
                                    var merchType = message.EventType switch
                                    {
                                        EmployeeEventType.Hiring => MerchType.WelcomePack,
                                        EmployeeEventType.ConferenceAttendance => GetMerchTypeByPayload(message.Payload),
                                        _ => (MerchType)0
                                    };

                                    if ((int) merchType == 0) continue;
                                    
                                    var processMerchRequestCommand = new ProcessMerchRequestCommand
                                    {
                                        EmployeeEmail = message.EmployeeEmail,
                                        MerchType = merchType,
                                        IsSystem = true
                                    };
                                    await mediator.Send(processMerchRequestCommand, stoppingToken);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Error while get consume. Message {ex.Message}");
                            }
                        }
                    }
                }
                finally
                {
                    c.Commit();
                    c.Close();
                }
            }
        }

        private MerchType GetMerchTypeByPayload(object payload)
        {
            if (payload == null)
            {
                return 0;
            }

            try
            {
                var jsonElement = (JsonElement) payload;
                var json = jsonElement.ToString();
                if (!string.IsNullOrEmpty(json))
                {
                    var merchDeliveryEventPayload = JsonSerializer.Deserialize<MerchDeliveryEventPayload>(json);
                    if (merchDeliveryEventPayload != null)
                    {
                        return merchDeliveryEventPayload.MerchType;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,$"Error while deserializing payload");
            }
           
            return 0;
        }
    }
}