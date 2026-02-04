using Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.V1
{
    [Route("api/v1/[controller]")]
    public class OtpController : JsonApiControllerBase
    {
        private readonly IOtpHandler _otpHandler;

        public OtpController(IOtpHandler otpHandler)
        {
            _otpHandler = otpHandler;
        }

        /// <summary>
        /// Generates a OTP for the user based on the provided credentials.
        /// </summary>
        /// <param name="userCredentials">User authentication credentials.</param>
        /// <returns>The user's external ID associated with the generated OTP.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(SimpleResponseModel<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAndSendOtpAsync(UserCredentialsModel userCredentials)
        {
            string userExternalId = await _otpHandler.CreateAndSendOtpAsync(userCredentials.Username, userCredentials.Password);
            return Ok(new SimpleResponseModel<string>(userExternalId));
        }
    }
}