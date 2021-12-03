using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using CSharpCourse.Core.Lib.Events;
using CSharpCourse.Core.Lib.Models;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OzonEdu.MerchandiseService.Infrastructure.Commands.SupplyEvent;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;

namespace OzonEdu.MerchandiseService.Infrastructure.HostedServices
{
    public class StockReplenishedEventConsumerBackgroundService : BackgroundService
    {
        private readonly KafkaConfiguration _kafkaConfiguration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StockReplenishedEventConsumerBackgroundService> _logger;

        public StockReplenishedEventConsumerBackgroundService(
            IOptions<KafkaConfiguration> kafkaConfiguration,
            IServiceScopeFactory scopeFactory,
            ILogger<StockReplenishedEventConsumerBackgroundService> logger
        )
        {
            _kafkaConfiguration = kafkaConfiguration.Value;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "StockReplenishedEventConsumerBackgroundService: {@KafkaConfiguration}",
                _kafkaConfiguration);

            await Task.Yield();

            var config = new ConsumerConfig
            {
                GroupId = _kafkaConfiguration.StockReplenishedEventGroupId,
                BootstrapServers = _kafkaConfiguration.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_kafkaConfiguration.StockReplenishedEventTopic);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Yield();
                    try
                    {
                        var cr = consumer.Consume(stoppingToken);
                        if (cr != null)
                        {
                            // Пополнились остатки

                            var message = JsonSerializer.Deserialize<StockReplenishedEvent>(cr.Message.Value);
                            if (message == null) continue;

                            _logger.LogInformation("Event: {@Message}", message);

                            var command = new ProcessSupplyEventCommand
                            {
                                Items = message.Type.Select(x => new SupplyShippedItem
                                {
                                    SkuId = x.Sku,
                                    Quantity = 1
                                }).ToArray()
                            };

                            try
                            {
                                using var scope = _scopeFactory.CreateScope();
                                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                                await mediator.Send(command, stoppingToken);
                                consumer.Commit();
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Error while process command {@Command}", command);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while get consume. Message: {Message}", ex.Message);
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}