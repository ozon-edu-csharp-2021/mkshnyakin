using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OzonEdu.MerchandiseService.Domain.AggregationModels.EmployeeAggregate;
using OzonEdu.MerchandiseService.GrpcServices;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;
using OzonEdu.MerchandiseService.Services;

namespace OzonEdu.MerchandiseService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<DatabaseConnectionOptions>(Configuration.GetSection(nameof(DatabaseConnectionOptions)));
            services.Configure<OzonEduStockApiGrpcOptions>(Configuration.GetSection(nameof(OzonEduStockApiGrpcOptions)));
            services.Configure<EmailOptions>(Configuration.GetSection(nameof(EmailOptions)));
            services.Configure<RedisOptions>(Configuration.GetSection(nameof(RedisOptions)));
            
            var redisOptions = Configuration.GetSection(nameof(RedisOptions)).Get<RedisOptions>();
            services.AddStackExchangeRedisCache(options =>
            {
                options.InstanceName = redisOptions.InstanceName;
                options.Configuration = redisOptions.Configuration;
            });

            services.AddDomainInfrastructure();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<MerchandiseServiceGrpcService>();
                endpoints.MapControllers();
            });
        }
    }
}