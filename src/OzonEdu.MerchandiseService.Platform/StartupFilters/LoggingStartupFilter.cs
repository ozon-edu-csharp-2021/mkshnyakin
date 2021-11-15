using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using OzonEdu.MerchandiseService.Platform.Middlewares;

namespace OzonEdu.MerchandiseService.Platform.StartupFilters
{
    public class LoggingStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                app.UseMiddleware<RouteHeadersLoggingMiddleware>();
                next(app);
            };
        }
    }
}