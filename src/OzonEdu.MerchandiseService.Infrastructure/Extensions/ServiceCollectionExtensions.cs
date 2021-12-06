using System;
using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationServices;
using OzonEdu.MerchandiseService.Infrastructure.Cache;
using OzonEdu.MerchandiseService.Infrastructure.Clients.Implementation;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.Handlers.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.HostedServices;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Implementation;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.Interfaces;
using OzonEdu.MerchandiseService.Infrastructure.Stubs;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMediatR(typeof(ProcessMerchRequestCommandHandler).Assembly);
            services.AddDatabaseComponents();
            services.AddRepositories();
            services.AddExternalServices();
            services.AddApplicationServices();
            services.AddCache(configuration);
            services.AddKafka(configuration);
            return services;
        }

        private static IServiceCollection AddDatabaseComponents(this IServiceCollection services)
        {
            services.AddScoped<IDbConnectionFactory<NpgsqlConnection>, NpgsqlConnectionFactory>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IChangeTracker, ChangeTracker>();
            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            services.AddScoped<IMerchPackItemRepository, MerchPackItemPostgreSqlRepository>();
            services.AddScoped<IMerchRequestRepository, MerchRequestPostgreSqlRepository>();
            return services;
        }

        private static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddScoped<IOzonEduEmployeeServiceClient, OzonEduEmployeeServiceHttpClient>();
            services.AddScoped<IOzonEduStockApiClient, OzonEduStockApiGrpcClient>();
            return services;
        }

        private static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IApplicationService, ApplicationService>();
            return services;
        }

        private static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<CacheKeysProvider>();

            var redisOptions = configuration.GetSection(nameof(RedisOptions)).Get<RedisOptions>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = redisOptions.InstanceName;
                options.Configuration = redisOptions.Configuration;
            });

            return services;
        }

        private static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IProducer<string, string>>(di =>
            {
                var iOptions = di.GetService<IOptions<KafkaConfiguration>>()
                               ?? throw new NullReferenceException($"{nameof(KafkaConfiguration)} is null");
                var kafkaConfiguration = iOptions.Value;
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = kafkaConfiguration.BootstrapServers,
                    Acks = Acks.All,
                    EnableIdempotence = true
                };
                var builder = new ProducerBuilder<string, string>(producerConfig);
                return builder.Build();
            });

            services.AddHostedService<EmployeeNotificationEventConsumerBackgroundService>();
            services.AddHostedService<StockReplenishedEventConsumerBackgroundService>();
            return services;
        }
    }
}