using EmployeeManagementSystem.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Modules.ProductCatalog.Application.Dtos;
using Modules.ProductCatalog.Application.Interfaces;
using PagedList.Core;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Modules.ProductCatalog.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<ActionResult<ServiceResponse>> Add(ProductCreateDto productCreateDto)
        {
            var response = await _service.Create(productCreateDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet("Get/{id}")]
        public async Task<ActionResult<ServiceResponse<ProductDetailsDto>>> Get(int id)
        {
            var response = await _service.Get<ProductDetailsDto>(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet("GetMany")]
        public async Task<ActionResult<ServiceResponse<IPagedList<ProductDetailsDto>>>> GetMany()
        {
            var response = await _service.GetMany<ProductListDto>();
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("Update")]
        public async Task<ActionResult<ServiceResponse>> Update(ProductUpdateDto ProductUpdateDto)
        {
            var response = await _service.Update(ProductUpdateDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ServiceResponse>> Delete(int id)
        {
            var response = await _service.Delete(id);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
