using EmployeeManagementSystem.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.ShoppingCart.Application.Dtos.ShoppingCart;
using Modules.ShoppingCart.Application.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Modules.ShoppingCart.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _service;

        public ShoppingCartController(IShoppingCartService service)
        {
            _service = service;
        }
        [Authorize(Roles = "Customer")]
        [HttpPost("Add")]
        public async Task<ActionResult<ServiceResponse>> Add([FromBody] ShoppingCartCreateDto addDto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }
            return Ok(await _service.Create(addDto));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetCartToAdmin/{id}")]
        public ActionResult<ServiceResponse<ShoppingCartDetailsDto>> GetCartToAdmin(int id)
        {
            return Ok(_service.Get<ShoppingCartDetailsDto>(id));
        }
        [Authorize(Roles = "Customer")]
        [HttpGet("GetCartOfCustomer")]
        public async Task<ActionResult<ServiceResponse<ShoppingCartDetailsDto>>> GetCartOfCustomer()
        {
            return Ok(await _service.Get());
        }
        //[HttpGet("GetMany")]
        //public ActionResult<ServiceResponse<IPagedList<ShoppingCartListDto>>> GetMany()
        //{
        //    return Ok(_service.GetMany<ShoppingCartListDto>());
        //}
        [HttpDelete("Delete/{productId}")]
        public async Task<ActionResult<ServiceResponse>> Delete(int productId)
        {
            return Ok(await _service.Delete(productId));
        }
    }
}
