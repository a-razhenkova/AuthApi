using AutoMapper;
using Business;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.V1
{
    [Route("api/v1/[controller]")]
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
        /// Creates an access token for clients.
        /// </summary>
        [AllowAnonymous, SensitiveData(isResponseSensitive: true)]
        [HttpPost]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateAccessTokenAsync()
        {
            TokenDto token = await _authenticator.CreateAccessTokenAsync(HttpContext.GetAuthorization());
            return Ok(_mapper.Map<TokenModel>(token));
        }

        /// <summary>
        /// Validates the provided access token.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("status"), SkipLog]
        [ProducesResponseType(typeof(TokenValidationResultModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> ValidateAccessTokenAsync()
        {
            TokenValidationResult tokenValidationResult = await _authenticator.ValidateAccessTokenAsync();
            return Ok(_mapper.Map<TokenValidationResultModel>(tokenValidationResult));
        }
    }
}