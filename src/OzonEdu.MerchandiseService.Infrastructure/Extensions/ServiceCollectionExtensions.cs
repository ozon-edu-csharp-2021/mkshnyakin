using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Domain.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.ApplicationServices;
using OzonEdu.MerchandiseService.Infrastructure.Cache;
using OzonEdu.MerchandiseService.Infrastructure.Clients.Implementation;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.Contracts.MessageBus;
using OzonEdu.MerchandiseService.Infrastructure.Handlers.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Implementation;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure;
using OzonEdu.MerchandiseService.Infrastructure.Repositories.Infrastructure.Interfaces;
using OzonEdu.MerchandiseService.Infrastructure.Stubs;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainInfrastructure(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ProcessMerchRequestCommandHandler).Assembly);
            services.AddDatabaseComponents();
            services.AddRepositories();
            services.AddExternalServices();
            services.AddApplicationServices();
            services.AddCache();
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
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddScoped<IOzonEduEmployeeServiceClient, OzonEduEmployeeServiceClient>();
            services.AddScoped<IOzonEduStockApiClient, OzonEduStockApiGrpcClient>();
            return services;
        }

        private static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IApplicationService, ApplicationService>();
            return services;
        }
        
        private static IServiceCollection AddCache(this IServiceCollection services)
        {
            services.AddSingleton<CacheKeysProvider>();
            return services;
        }
    }
}