using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OzonEdu.MerchandiseService.Infrastructure.Swagger
{
    public class SwaggerOptions
    {
        public static void Setup(SwaggerGenOptions options)
        {
            var assembly = Assembly.GetExecutingAssembly().GetName();
            var name = assembly.Name ?? "ServiceName";
            var version = assembly.Version?.ToString() ?? "0.0.0.0";
            options.SwaggerDoc("v1", new OpenApiInfo {Title = name, Version = version});
            options.CustomSchemaIds(x => x.FullName);

            var xmlFileName = $"{name}.xml";
            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
            options.IncludeXmlComments(xmlFilePath);
        }
    }
}