using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OzonEdu.MerchandiseService.GrpcServices;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;
using OzonEdu.MerchandiseService.Infrastructure.Extensions;
using OzonEdu.MerchandiseService.Platform.Jaeger;

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
            services.Configure<OzonEduEmployeeServiceHttpOptions>(Configuration.GetSection(nameof(OzonEduEmployeeServiceHttpOptions)));
            services.Configure<EmailOptions>(Configuration.GetSection(nameof(EmailOptions)));
            services.Configure<RedisOptions>(Configuration.GetSection(nameof(RedisOptions)));
            services.Configure<KafkaConfiguration>(Configuration.GetSection(nameof(KafkaConfiguration)));
            services.Configure<JaegerOptions>(Configuration.GetSection(nameof(JaegerOptions)));
            services.AddDomainInfrastructure(Configuration);
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