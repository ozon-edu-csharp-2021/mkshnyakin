using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OzonEdu.MerchandiseService.Platform.Filters;
using OzonEdu.MerchandiseService.Platform.Interceptors;
using OzonEdu.MerchandiseService.Platform.StartupFilters;
using OzonEdu.MerchandiseService.Platform.Swagger;

namespace OzonEdu.MerchandiseService.Platform.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder AddInfrastructure(this IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IStartupFilter, LoggingStartupFilter>();
                services.AddSingleton<IStartupFilter, TerminalEndpointsStartupFilter>();

                services.AddControllers(options => options.Filters.Add<GlobalExceptionFilter>());

                services.AddSingleton<IStartupFilter, SwaggerStartupFilter>();
                services.AddSwaggerGen(SwaggerOptions.Setup);
                
                services.AddGrpc(options => options.Interceptors.Add<LoggingInterceptor>());
            });
            return builder;
        }
    }
}