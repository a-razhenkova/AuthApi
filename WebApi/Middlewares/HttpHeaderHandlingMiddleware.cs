using Infrastructure;

namespace WebApi
{
    public class HttpHeaderHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpHeaderHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            await _next(httpContext);

            httpContext.Response.Headers[HttpHeaders.RequestId] = httpContext.GetTraceId();
            httpContext.Response.Headers[HttpHeaders.CorrelationId] = httpContext.GetCorrelationId();
        }
    }
}