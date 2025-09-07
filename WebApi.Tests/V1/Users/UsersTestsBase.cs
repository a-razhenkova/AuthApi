using Database.AuthDb;
using Microsoft.AspNetCore.Mvc;
using WebApi.V1;

namespace WebApi.Tests.V1
{
    public abstract class UsersTestsBase
    {
        public static async Task<(int StatusCode, string Body)> LoadUserAsync(string userExternalId, AuthDbContext? authDbContext = null)
            => await HttpRequestMock.MockRequest(async (httpContext) =>
            {
                var controller = ControllerMock.MockUserController(httpContext, authDbContext);
                IActionResult? actionResult = await controller.LoadUserAsync(userExternalId);
                await actionResult.MockResponse<UserModel>(httpContext);
            });
    }
}