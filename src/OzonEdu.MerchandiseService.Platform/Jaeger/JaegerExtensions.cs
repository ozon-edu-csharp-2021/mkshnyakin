using System;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using Jaeger.Senders.Thrift;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using OzonEdu.MerchandiseService.Platform.StartupFilters;

namespace OzonEdu.MerchandiseService.Platform.Jaeger
{
    public static class JaegerExtensions
    {
        public static IServiceCollection AddJaeger(this IServiceCollection services)
        {
            services.AddSingleton<ITracer>(sp =>
            {
                var iOptions = sp.GetService<IOptions<JaegerOptions>>()
                               ?? throw new NullReferenceException($"{nameof(JaegerOptions)} is null");
                var jaegerOptions = iOptions.Value;
                
                var udpSender = new UdpSender(jaegerOptions.UdpSenderHost, jaegerOptions.UdpSenderPort, 0);
                var serviceName = sp.GetRequiredService<IWebHostEnvironment>().ApplicationName;
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                
                // LoggingReporter prints every reported span to the logging framework.
                var reporter = new RemoteReporter.Builder()
                    .WithLoggerFactory(loggerFactory)
                    .WithSender(udpSender)
                    .Build();

                // The constant sampler reports every span.
                var sampler = new ConstSampler(true);
                
                var tracer = new Tracer.Builder(serviceName)
                    .WithSampler(sampler)
                    .WithReporter(reporter)
                    .Build();
                
                return tracer;
            });
            
            services.AddSingleton<IStartupFilter, JaegerStartupFilter>();
            return services;
        }
    }
}