using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using identity.api.Interfaces.Identity;
using identity.api.Models.Identity;
using identity.api.Models.oAuth;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace identity.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityUserService _identityUserService;

        public IdentityController(IIdentityUserService identityUserService)
        {
            _identityUserService = identityUserService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] ApplicationUserDTO user)
        {
            try
            {
                IdentityResult result = await _identityUserService.Register(user);

                if (result.Succeeded)
                    return Ok();
                return Conflict(result.Errors);
            }
            catch (Exception ex)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpPost("connect/token")]
        public async Task<IActionResult> Token([FromBody] TokenRequestDTO tokenRequest)
        {
            try
            {
                TokenResponse result = await _identityUserService.Token(tokenRequest);
                if (!result.IsError)
                    return Ok(result.Json);
                return Conflict(result.Error);
            }
            catch (Exception ex)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpPost("connect/refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDTO tokenRequest)
        {
            try
            {
                TokenResponse result = await _identityUserService.RefreshToken(tokenRequest);
                if (!result.IsError)
                    return Ok(result.Json);
                return Conflict(result.Error);
            }
            catch (Exception ex)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpPost("connect/revoketoken")]
        public async Task<IActionResult> RevokeToken([FromBody] TokenRequestDTO tokenRequest)
        {
            try
            {
                TokenRevocationResponse result = await _identityUserService.RevokeToken(tokenRequest);
                if (!result.IsError)
                    return Ok();
                return Conflict(result.Error);
            }
            catch (Exception ex)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpGet("registertest")]
        public async Task<IActionResult> RegisterTest()
        {
            try
            {
                var user = new ApplicationUserDTO()
                {
                    Email = "faruk.redzic@klika.ba",
                    Password = "password",
                    Roles = new List<string>() { "admin" },
                    UserName = "username"
                };

                IdentityResult result = await _identityUserService.Register(user);
                if (result.Succeeded)
                    return Ok();
                return Conflict(result.Errors);
            }
            catch (Exception ex)
            {
                //TODO: add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
