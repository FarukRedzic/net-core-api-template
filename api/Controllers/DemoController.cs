using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using api.model.Dto;
using api.model.Interfaces.Factory;

namespace api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : ControllerBase
    {
        public ITemplateServiceFactory _templateServiceFactory { get; set; }
        public DemoController(ITemplateServiceFactory templateServiceFactory)
        
        {
            _templateServiceFactory = templateServiceFactory;
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Demo()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            return Ok(claimsIdentity);
        }

        [HttpPost("createservice")]
        public async Task<IActionResult> CreateService([FromBody] CreateServiceDTO serviceDto)
        {
            try
            {
                ITemplateService templateService = _templateServiceFactory.GetService(serviceDto.ServiceType);
                await templateService.CreateService();
                return Ok();
            }
            catch (Exception ex)
            {
                //TODO: Add logger
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}