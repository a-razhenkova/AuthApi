using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System.Text.Json;

namespace WebApi.Tests
{
    public static class HttpRequestMock
    {
        private static ExceptionHandlingMiddleware MockExceptionHandlingMiddleware(Func<HttpContext, Task> request)
            => new(request.Invoke, LoggerMock.GetLogger<ExceptionHandlingMiddleware>(),
                new HostingEnvironment { EnvironmentName = Environments.Development });

        public static async Task<(int StatusCode, string Body)> MockRequest(Func<HttpContext, Task> request)
        {
            HttpContext httpContext = new DefaultHttpContext();

            var tempResponseBody = new MemoryStream();
            httpContext.Response.Body = tempResponseBody;

            await MockExceptionHandlingMiddleware(request).InvokeAsync(httpContext);

            string responseMsg = await httpContext.Response.Body.ReadToEndAsync();
            return (httpContext.Response.StatusCode, responseMsg);
        }

        public static void MockResponse(this IActionResult actionResult, HttpContext httpContext)
        {
            Assert.True(actionResult is OkResult);
            OkResult okResult = (OkResult)actionResult;

            httpContext.Response.StatusCode = okResult.StatusCode;
        }

        public static async Task MockResponse<TResponse>(this IActionResult actionResult, HttpContext httpContext)
        {
            Assert.True(actionResult is ObjectResult);
            ObjectResult objectResult = (ObjectResult)actionResult;

            Assert.NotNull(objectResult.StatusCode);
            httpContext.Response.StatusCode = objectResult.StatusCode.GetValueOrDefault();

            Assert.NotNull(objectResult.Value);
            await httpContext.Response.WriteAsJsonAsync((TResponse)objectResult.Value);
        }

        public static TResponse DeserializeResponse<TResponse>(this string body)
        {
            return JsonSerializer.Deserialize<TResponse>(body, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            }) ?? throw new InvalidOperationException();
        }
    }
}