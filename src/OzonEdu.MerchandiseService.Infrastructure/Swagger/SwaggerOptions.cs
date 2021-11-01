using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using static OzonEdu.MerchandiseService.Infrastructure.Helpers.AssemblyHelper;

namespace OzonEdu.MerchandiseService.Infrastructure.Swagger
{
    public class SwaggerOptions
    {
        public static void Setup(SwaggerGenOptions options)
        {
            var (name, version) = GetEntryAssemblyInfo();
            options.SwaggerDoc("v1", new OpenApiInfo {Title = name, Version = version});
            options.CustomSchemaIds(x => x.FullName);

            var xmlFileName = $"{name}.xml";
            var xmlFilePath = Path.Combine(AppContext.BaseDirectory, xmlFileName);
            options.IncludeXmlComments(xmlFilePath);
        }
    }
}