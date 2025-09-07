using Infrastructure;
using Serilog.Context;
using System.Diagnostics;
using System.Security.Claims;

namespace WebApi
{
    public class HttpMessageLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HttpMessageLoggingMiddleware> _logger;

        public HttpMessageLoggingMiddleware(RequestDelegate next,
                                           ILogger<HttpMessageLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.GetEndpoint()?.Metadata?.GetMetadata<SkipLogAttribute>() is not null)
            {
                await _next(httpContext);
            }
            else
            {
                using (LogContext.PushProperty(LoggerContextProperty.ActionType.ToString(), LoggerContext.WebApi))
                using (LogContext.PushProperty(LoggerContextProperty.TraceId.ToString(), httpContext.GetTraceId()))
                using (LogContext.PushProperty(LoggerContextProperty.CorrelationId.ToString(), httpContext.GetCorrelationId()))
                {
                    httpContext.Request.EnableBuffering();
                    string requestBody = await ReadBodyAsync(httpContext.Request.Body);

                    using Stream originalResponseBody = httpContext.Response.Body;
                    using var tempResponseBody = new MemoryStream();
                    httpContext.Response.Body = tempResponseBody;

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    await _next(httpContext);
                    stopwatch.Stop();

                    string responseBody = await ReadBodyAsync(httpContext.Response.Body);
                    await tempResponseBody.CopyToAsync(originalResponseBody);
                    httpContext.Response.Body = originalResponseBody;

                    SensitiveDataAttribute? sensitiveData = httpContext.GetEndpoint()?.Metadata?.GetMetadata<SensitiveDataAttribute>();

                    _logger.LogInformation("Request finished: {@HttpAction}", new HttpAction()
                    {
                        FromIp = httpContext.GetIpAddresses() ?? "unknown",
                        User = httpContext.User.FindFirstValue(TokenClaim.Username.GetDescription()) ?? "unauthorized",
                        Duration = stopwatch.ElapsedMilliseconds,
                        StatusCode = httpContext.Response.StatusCode,
                        Method = httpContext.Request.Method,
                        RequestData = sensitiveData is not null && sensitiveData.IsRequestSensitive ? Constants.SensitiveDataMask : requestBody,
                        ResponseData = sensitiveData is not null && sensitiveData.IsResponseSensitive ? Constants.SensitiveDataMask : responseBody
                    });
                }
            }
        }

        private async Task<string> ReadBodyAsync(Stream body)
        {
            string bodyMsg = string.Empty;

            try
            {
                bodyMsg = await body.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(bodyMsg))
                {
                    bodyMsg = bodyMsg.RemoveJsonFormatting();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            return bodyMsg;
        }
    }
}