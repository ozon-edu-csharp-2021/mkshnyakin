using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using static OzonEdu.MerchandiseService.Platform.Helpers.AssemblyHelper;

namespace OzonEdu.MerchandiseService.Platform.Middlewares
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
            var (name, version) = GetEntryAssemblyInfo();
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