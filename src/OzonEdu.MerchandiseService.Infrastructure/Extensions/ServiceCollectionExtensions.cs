using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchPackItemAggregate;
using OzonEdu.MerchandiseService.Domain.AggregationModels.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.Contracts.MessageBus;
using OzonEdu.MerchandiseService.Infrastructure.Handlers.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Stubs;

namespace OzonEdu.MerchandiseService.Infrastructure.Extensions
{
    /// <summary>
    /// Класс расширений для типа <see cref="IServiceCollection"/> для регистрации инфраструктурных сервисов
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление в DI контейнер инфраструктурных сервисов
        /// </summary>
        /// <param name="services">Объект IServiceCollection</param>
        /// <returns>Объект <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddDomainInfrastructure(this IServiceCollection services)
        {
            services.AddMediatR(typeof(CreateMerchRequestCommandHandler).Assembly);
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddSingleton<IMerchPackItemRepository, MerchPackItemRepository>();
            services.AddSingleton<IMerchRequestRepository, MerchRequestRepository>();
            services.AddSingleton<IOzonEduEmployeeServiceClient, OzonEduEmployeeServiceClient>();
            services.AddSingleton<IOzonEduStockApiClient, OzonEduStockApiClient>();
            return services;
        }

        /// <summary>
        /// Добавление в DI контейнер инфраструктурных репозиториев
        /// </summary>
        /// <param name="services">Объект IServiceCollection</param>
        /// <returns>Объект <see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
        {
            return services;
        }
    }
}