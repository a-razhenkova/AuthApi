﻿using Business;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApi.V1
{
    [AuthorizeUser(UserRoles.Administrator)]
    [Route("api/v1/[controller]")]
    public class HealthController : JsonApiControllerBase
    {
        private readonly IHealthChecker _healthChecker;
        private readonly IHostEnvironment _environment;

        public HealthController(IHealthChecker healthChecker,
                               IHostEnvironment environment)
        {
            _healthChecker = healthChecker;
            _environment = environment;
        }

        /// <summary>
        /// Provides a simple heartbeat endpoint to verify the service is running.
        /// </summary>
        [AllowAnonymous]
        [HttpHead("heartbeat"), SkipLog]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult HeartbeatAsync()
        {
            return Ok();
        }

        /// <summary>
        /// Retrieves information about the machine and build.
        /// </summary>
        /// <returns>Deploy details.</returns>
        [HttpGet, SkipLog]
        [ProducesResponseType(typeof(DeployInfoModel), StatusCodes.Status200OK)]
        public IActionResult LoadDeployInfo()
        {
            var deployInfo = new DeployInfoModel()
            {
                Version = WebApiAssembly.GetVersion(),
                Environment = _environment.EnvironmentName,
                MachineName = Environment.MachineName,
                MachineTimestamp = DateTime.Now
            };

            return Ok(deployInfo);
        }

        /// <summary>
        /// Checks the health of the application and its dependencies.
        /// </summary>
        [HttpGet("checks"), SkipLog]
        public async Task<IActionResult> CheckHealthAsync()
        {
            HealthReport healthReport = await _healthChecker.CheckHealthAsync();
            return Ok(healthReport);
        }
    }
}