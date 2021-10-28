using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OzonEdu.MerchandiseService.Infrastructure.Middlewares
{
    public class VersionMiddleware
    {
        private const string ContentType = "application/json; charset=utf-8";

        private readonly string _json;

        private readonly JsonSerializerOptions _serializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public VersionMiddleware(RequestDelegate next)
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var name = assembly.Name ?? "ServiceName";
            var version = assembly.Version?.ToString() ?? "0.0.0.0";
            var model = new Model
            {
                Version = version,
                ServiceName = name
            };
            _json = JsonSerializer.Serialize(model, _serializeOptions);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.ContentType = ContentType;
            await context.Response.WriteAsync(_json);
        }

        public class Model
        {
            public string Version { get; set; }
            public string ServiceName { get; set; }
        }
    }
}