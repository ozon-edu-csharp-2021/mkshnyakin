using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenTracing;

namespace OzonEdu.MerchandiseService.Platform.Middlewares
{
    public class JaegerMiddleware
    {
        private readonly RequestDelegate _next;
        
        public JaegerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context, ITracer tracer)
        {
            using var span = tracer
                .BuildSpan("Main")
                .StartActive();
            await _next(context);
        }
    }
}