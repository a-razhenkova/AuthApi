using AutoMapper;
using Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.V3
{
    /// <summary>
    /// Controller responsible for handling authentication-related endpoints.
    /// </summary>
    [Authorize]
    [ApiController, Route("api/v3/[controller]")]
    public class TokenController : JsonApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthenticator _authenticator;

        public TokenController(IMapper mapper,
                               IAuthenticator authenticator)
        {
            _mapper = mapper;
            _authenticator = authenticator;
        }

        /// <summary>
        /// Creates an access token for user by the provided OTP.
        /// </summary>
        /// <param name="userCredentials">The user's OTP and related two-factor authentication information.</param>
        [AllowAnonymous, SensitiveData(isResponseSensitive: true)]
        [HttpPost]
        [ProducesResponseType(typeof(V1.TokenModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAccessTokenAsync(V2.UserCredentialsModel userCredentials)
        {
            TokenDto token = await _authenticator.CreateAccessTokenByOtpAsync(userCredentials.UserId, userCredentials.OneTimePassword);
            return Ok(_mapper.Map<V1.TokenModel>(token));
        }
    }
}