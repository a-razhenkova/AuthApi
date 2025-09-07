using Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.V1
{
    [Route("api/v1/[controller]")]
    public class MfaController : JsonApiControllerBase
    {
        private readonly IAuthenticator _authenticator;

        public MfaController(IAuthenticator authenticator)
        {
            _authenticator = authenticator;
        }

        /// <summary>
        /// Generates a OTP for the user based on the provided credentials.
        /// </summary>
        /// <param name="userCredentials">User authentication credentials.</param>
        /// <returns>The user's external ID associated with the generated OTP.</returns>
        [AllowAnonymous]
        [HttpPost("otp")]
        [ProducesResponseType(typeof(SimpleResponseModel<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOtpAsync(UserCredentialsModel userCredentials)
        {
            string userExternalId = await _authenticator.CreateAndSendOtpAsync(userCredentials.Username, userCredentials.Password);
            return Ok(new SimpleResponseModel<string>(userExternalId));
        }
    }
}