using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace OzonEdu.MerchandiseService.Platform.Middlewares
{
    public class RouteHeadersLoggingMiddleware
    {
        private const string GrpcContentType = "application/grpc";
        private readonly ILogger<RouteHeadersLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        private readonly JsonSerializerOptions _serializeOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public RouteHeadersLoggingMiddleware(RequestDelegate next, ILogger<RouteHeadersLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var contentType = context.Request.ContentType;
            if (!string.IsNullOrEmpty(contentType) && contentType.Contains(GrpcContentType))
            {
                // Ignore gRPC request
                await _next(context);
                return;
            }

            var path = context.Request.Path.HasValue
                ? context.Request.Path.Value
                : string.Empty;

            var requestHeaders = GetHeaders(context.Request.Headers);

            await _next(context);

            var responseHeaders = GetHeaders(context.Response.Headers);

            try
            {
                var model = new Model
                {
                    Path = path,
                    RequestHeaders = requestHeaders,
                    ResponseHeaders = responseHeaders
                };
                var json = JsonSerializer.Serialize(model, _serializeOptions);
                _logger.LogInformation(json);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not log request/response headers");
            }
        }

        private static IEnumerable<string> GetHeaders(IHeaderDictionary headerDictionary)
        {
            var headers = headerDictionary is null
                ? Enumerable.Empty<string>()
                : headerDictionary.Select(x => $"{x.Key}: {x.Value}");

            return headers;
        }

        public class Model
        {
            public string Path { get; set; }
            public IEnumerable<string> RequestHeaders { get; set; }
            public IEnumerable<string> ResponseHeaders { get; set; }
        }
    }
}