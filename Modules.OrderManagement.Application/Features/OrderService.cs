using AutoMapper;
using EmployeeManagementSystem.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Modules.OrderManagement.Application.Interfaces;
using Modules.PaymentProcessing.Application.Dtos;
using Modules.PaymentProcessing.Application.Interfaces;
using Shared.Utilities.Models.Entities;
using Shared.Utilities.Models.Enums;
using System.Security.Claims;
using Utilities.Shared.Services.GenericServices.Services;

namespace Modules.OrderManagement.Application.Features
{
    public sealed class OrderService : Service<Order>, IOrderService
    {
        private readonly IPaymentService _paymentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderService(EasyRepository.EFCore.Generic.IUnitOfWork unitOfWork,
            IPaymentService paymentService, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, mapper)
        {
            _paymentService = paymentService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<ServiceResponse> Create<TAddDto>(TAddDto dto)
        {
            var currentId = int.Parse(_httpContextAccessor.HttpContext.User.Claims
                .First(p => p.Type == ClaimTypes.NameIdentifier).Value);

            var currentCart = await _unitOfWork.Carts.Value
                .GetAsync(predicate: s => s.CreatedByUserId == currentId && s.Status == CartStatusEnum.Pending,
                x => x.Include(s => s.CartProducts).ThenInclude(s => s.Product));

            if (currentCart is null)
            {
                return new ServiceResponse
                {
                    Message = "You Have To Add Any Product To Cart ,To Checkout",
                    StatusCode = StatusCodes.Status400BadRequest,
                    Success = false,
                };
            }

            var createdOrderOnThisCartIfExists = await _unitOfWork.Orders.Value.GetAsync(s => s.CartId == currentCart.Id);

            if (createdOrderOnThisCartIfExists is null)
            {
                var order = new Order
                {
                    CartId = currentCart.Id
                };
                await _unitOfWork.Orders.Value.AddAsync(order);

                int res = await _unitOfWork.SaveChangesAsync();
                if (res == 0)
                {
                    return new ServiceResponse
                    {
                        Message = "Order Cannot Created",
                        StatusCode = StatusCodes.Status500InternalServerError,
                        Success = false,
                    };
                }
            }

            return await _paymentService.Create(new PaymentCreateDto
            {
                OrderId = createdOrderOnThisCartIfExists.Id,
                Provider = "Card",
                CartId = currentCart.Id,
                Amount = currentCart.CartProducts.Sum(s => s.Quantity * s.Product.Price)
            });
        }
    }
}
