using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OzonEdu.MerchandiseService.Platform.Middlewares
{
    public class ReadyMiddleware
    {
        private const string ResponseText = "200 Ok";

        public ReadyMiddleware(RequestDelegate next)
        {
        }

        public async Task InvokeAsync(HttpContext context) => await context.Response.WriteAsync(ResponseText);
    }
}