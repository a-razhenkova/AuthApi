using Database.AuthDb;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.V1;

namespace WebApi.Tests
{
    public static class ControllerMock
    {
        public static UsersController MockUserController(HttpContext httpContext, AuthDbContext? authDbContext = null)
            => MockController(new UsersController(MapperMock.GetMapper(), ServiceMock.MockUserService(authDbContext)), httpContext);

        private static TController MockController<TController>(TController controller, HttpContext httpContext)
            where TController : ControllerBase
        {
            controller.ControllerContext.HttpContext = httpContext;
            return controller;
        }
    }
}