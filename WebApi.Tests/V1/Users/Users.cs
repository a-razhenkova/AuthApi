using Database.IdentityDb;
using Database.IdentityDb.DefaultSchema;
using Microsoft.AspNetCore.Http;
using WebApi.Tests;
using WebApi.Tests.V1;
using WebApi.V1;
using Xunit.Priority;

namespace V1
{
    [Collection("UsersTests")]
    public class Users : UsersTestsBase
    {
        [Priority(1)]
        [Theory(DisplayName = "GET api/v1/users")]
        [InlineData(StatusCodes.Status200OK)]
        [InlineData(StatusCodes.Status404NotFound)]
        public async Task Load(int expectedStatusCode)
        {
            // Arrange
            IdentityDbContext authDbContext = DatabaseMock.GetAuthDbContext();

            User user;
            if (expectedStatusCode == StatusCodes.Status200OK)
            {
                user = await FakeUser.CreateAndSave(authDbContext);
            }
            else
            {
                user = FakeUser.Create();
            }

            // Act
            var response = await LoadUserAsync(user.ExternalId, authDbContext);

            // Assert
            Assert.Equal(expectedStatusCode, response.StatusCode);
            Assert.NotEmpty(response.Body);

            if (expectedStatusCode == StatusCodes.Status200OK)
            {
                UserModel userModel = response.Body.DeserializeResponse<UserModel>();
                UserAssert.Equal(user, userModel);
            }
            else
            {
                response.Body.DeserializeResponse<ExceptionModel>();
            }
        }
    }
}