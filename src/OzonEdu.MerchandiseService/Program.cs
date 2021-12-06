using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using OzonEdu.MerchandiseService.Platform.Extensions;
using Serilog;

namespace OzonEdu.MerchandiseService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) => configuration
                    .ReadFrom
                    .Configuration(context.Configuration)
                    .WriteTo.Console())
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .AddInfrastructure();
    }
}