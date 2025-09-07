using AutoMapper;
using Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.V2
{
    /// <summary>
    /// Controller responsible for handling authentication-related endpoints.
    /// </summary>
    [Authorize]
    [ApiController, Route("api/v2/[controller]")]
    public class TokensController : JsonApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAuthenticator _authenticator;

        public TokensController(IMapper mapper,
                               IAuthenticator authenticator)
        {
            _mapper = mapper;
            _authenticator = authenticator;
        }

        /// <summary>
        /// Creates an access token for users.
        /// </summary>
        /// <param name="userCredentials">User authentication credentials.</param>
        [AllowAnonymous, SensitiveData(isResponseSensitive: true)]
        [HttpPost]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAccessTokenAsync(V1.UserCredentialsModel userCredentials)
        {
            TokenDto token = await _authenticator.CreateAccessTokenAsync(userCredentials.Username, userCredentials.Password);
            return Ok(_mapper.Map<TokenModel>(token));
        }

        /// <summary>
        /// Refreshes the access token for a user.
        /// </summary>
        [AllowAnonymous, SensitiveData(isResponseSensitive: true)]
        [HttpPut]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshAccessTokenAsync()
        {
            TokenDto token = await _authenticator.RefreshAccessTokenAsync();
            return Ok(_mapper.Map<TokenModel>(token));
        }
    }
}