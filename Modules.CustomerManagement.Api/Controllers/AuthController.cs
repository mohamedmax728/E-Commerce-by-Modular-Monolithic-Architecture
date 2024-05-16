using EmployeeManagementSystem.Application.Dtos.User;
using EmployeeManagementSystem.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.CustomerManagement.Application.Dtos.User;
using Modules.CustomerManagement.Application.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Modules.CustomerManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // GET: api/<AuthController>
        [HttpPost("Register")]
        public async ActionResult<Task<ServiceResponse<string>>> Register([FromBody] UserRegisterDto registerDto)
        {
            var response = await authService.Register(registerDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddNewUser")]
        public async Task<ActionResult<ServiceResponse<string>>> AddNewUser([FromBody] UserCreateDto userCreateDto)
        {
            var response = await authService.Register(userCreateDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpPost("Login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] UserLoginDto loginDto)
        {
            ServiceResponse<string> response = await authService.Login(loginDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
