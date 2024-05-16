using EmployeeManagementSystem.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.OrderManagement.Application.Dtos;
using Modules.OrderManagement.Application.Interfaces;
using PagedList;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Modules.OrderManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("Add")]
        public async Task<ActionResult<ServiceResponse>> Add([FromBody] OrderCreateDto addDto)
        {
            return Ok(await _orderService.Create(addDto));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("Get/{id}")]
        public async Task<ActionResult<ServiceResponse<OrderDetailsDto>>> GetById(int id)
        {
            return Ok(await _orderService.Get<OrderDetailsDto>(id));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("GetMany")]
        public async Task<ActionResult<IPagedList<ServiceResponse<OrderListDto>>>> GetMany()
        {
            return Ok(await _orderService.GetMany<OrderListDto>());
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ServiceResponse>> Delete(int id)
        {
            return Ok(await _orderService.Delete(id));
        }
    }
}
