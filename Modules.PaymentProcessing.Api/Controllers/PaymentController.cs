using EmployeeManagementSystem.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.PaymentProcessing.Application.Dtos;
using Modules.PaymentProcessing.Application.Interfaces;
using PagedList;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Modules.PaymentProcessing.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        //[HttpPost("Add")]
        //public ActionResult<ServiceResponse> Add([FromBody] PaymentCreateDto addDto)
        //{
        //    return Ok(_paymentService.Create(addDto));
        //}

        [HttpGet("Get/{id}")]
        public async Task<ActionResult<PaymentDetailsDto>> GetById(int id)
        {
            return Ok(await _paymentService.Get<PaymentDetailsDto>(id));
        }
        [HttpGet("GetMany")]
        public async Task<ActionResult<IPagedList<PaymentListDto>>> GetMany()
        {
            return Ok(await _paymentService.GetMany<PaymentListDto>());
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ServiceResponse>> Delete(int id)
        {
            return Ok(await _paymentService.Delete(id));
        }
    }
}
