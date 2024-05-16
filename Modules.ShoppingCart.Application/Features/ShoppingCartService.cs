using AutoMapper;
using EasyRepository.EFCore.Generic;
using EmployeeManagementSystem.Domain.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Modules.ShoppingCart.Application.Dtos.ShoppingCart;
using Modules.ShoppingCart.Application.Interfaces;
using PagedList;
using Shared.Utilities.Models.Entities;
using System.Security.Claims;
using Utilities.Shared.Services.GenericServices.Services;

namespace Modules.ShoppingCart.Application.Features
{
    public class ShoppingCartService : Service<Cart>, IShoppingCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ShoppingCartService(IUnitOfWork unitOfWork, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, mapper)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public override async Task<ServiceResponse<IPagedList<TListDto>>> GetMany<TListDto>()
        {
            var currentUserRole = _httpContextAccessor.HttpContext.User.Claims
                .First(p => p.Type == ClaimTypes.Role).Value;
            if (currentUserRole == "Admin")
            {
                return await base.GetMany<TListDto>();
            }
            var currentUserId = int.Parse(_httpContextAccessor.HttpContext.User.Claims
                .First(p => p.Type == ClaimTypes.NameIdentifier).Value);
            return await GetMany<TListDto>(s => s.CreatedByUserId == currentUserId);
        }
        public override async Task<ServiceResponse<TDetailsDto>> Get<TDetailsDto>(object Id)
        {
            var currentUserRole = _httpContextAccessor.HttpContext.User.Claims
                .First(p => p.Type == ClaimTypes.Role).Value;
            if (currentUserRole == "Admin")
            {
                return await Get<TDetailsDto>(Id, include: s => s.Include(x => x.CartProducts));
            }
            var currentUserId = int.Parse(_httpContextAccessor.HttpContext.User.Claims
                .First(p => p.Type == ClaimTypes.NameIdentifier).Value);
            return await Get<TDetailsDto>(s => s.CreatedByUserId == currentUserId && (int)Id == s.Id,
                include: s => s.Include(x => x.CartProducts).ThenInclude(x => x.Product));
        }
        public async Task<ServiceResponse<ShoppingCartDetailsDto>> Get()
        {

            var currentUserId = int.Parse(_httpContextAccessor.HttpContext.User.Claims
                .First(p => p.Type == ClaimTypes.NameIdentifier).Value);
            return await Get<ShoppingCartDetailsDto>(s => s.CreatedByUserId == currentUserId
                && s.Status == Shared.Utilities.Models.Enums.CartStatusEnum.Pending,
                include: s => s.Include(x => x.CartProducts).ThenInclude(x => x.Product));
        }
        public override async Task<ServiceResponse> Create<TAddDto>(TAddDto dto)
        {
            var addDto = dto as ShoppingCartCreateDto;
            var currentUserId = int.Parse(_httpContextAccessor.HttpContext.User.Claims
                .First(p => p.Type == ClaimTypes.NameIdentifier).Value);
            var currentCart = await _unitOfWork.Carts.Value
                .GetAsync(s => s.CreatedByUserId == currentUserId && s.Status == Shared.Utilities.Models.Enums.CartStatusEnum.Pending);

            if (currentCart is null)
            {
                var cart = new Cart
                {
                    Status = Shared.Utilities.Models.Enums.CartStatusEnum.Pending,
                    CartProducts = [new CartProduct
                    {
                        ProductId = addDto.ProductId,
                        Quantity = addDto.Quantity,
                    }]
                };

                await _unitOfWork.Carts.Value.AddAsync(cart);
            }
            else
            {
                var hasSpecficProductInCart =
                    await _unitOfWork.CartProducts.Value
                    .GetAsync(s => s.ProductId == addDto.ProductId && currentCart.Id == s.CartId);

                if (hasSpecficProductInCart is null)
                {
                    await _unitOfWork.CartProducts.Value.AddAsync(new CartProduct
                    {
                        CartId = currentCart.Id,
                        ProductId = addDto.ProductId,
                        Quantity = addDto.Quantity,
                    });
                }
                else
                {
                    hasSpecficProductInCart.Quantity = addDto.Quantity;
                }
            }
            int res = await _unitOfWork.SaveChangesAsync();
            return new ServiceResponse
            {
                StatusCode = res > 0 ? StatusCodes.Status200OK : StatusCodes.Status400BadRequest,
                Success = res > 0 ? true : false,
                Message = res > 0 ? "Added Sucessfully To Cart" : "Failed"
            };
        }
        public override async Task<ServiceResponse> Delete(object productId)
        {
            var cartProduct = await _unitOfWork.CartProducts.Value.GetAsync(s => productId != null
                && s.ProductId == (int)productId);
            if (cartProduct is null)
            {
                return new ServiceResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Success = false,
                    Message = "Fail To Delete"
                };
            }
            await _unitOfWork.CartProducts.Value.Delete(cartProduct);
            int res = await _unitOfWork.SaveChangesAsync();
            return new ServiceResponse
            {
                StatusCode = res > 0 ? StatusCodes.Status200OK : StatusCodes.Status500InternalServerError,
                Success = res > 0 ? true : false,
                Message = res > 0 ? "Deleted Successfully!!" : "Fail To Delete"
            };
        }
    }
}
