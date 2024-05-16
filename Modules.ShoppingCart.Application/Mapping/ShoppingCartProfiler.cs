using AutoMapper;
using Modules.ShoppingCart.Application.Dtos.CartProduct;
using Modules.ShoppingCart.Application.Dtos.ShoppingCart;
using Shared.Utilities.Models;
using Shared.Utilities.Models.Entities;
using Shared.Utilities.Models.Enums;

namespace Modules.ShoppingCart.Application.Mapping
{
    public class ShoppingCartProfiler : Profile
    {
        public ShoppingCartProfiler()
        {
            CreateMap<Cart, ShoppingCartListDto>();
            CreateMap<Cart, ShoppingCartDetailsDto>();
            CreateMap<AuditModel, AuditModelVM>();
            CreateMap<ShoppingCartCreateDto, Cart>()
                .ForMember(des => des.Status, opt => opt.MapFrom(src => CartStatusEnum.Pending));
            CreateMap<ShoppingCartUpdateDto, Cart>();
            CreateMap<CartProduct, CartProductDetailsDto>()
                .ForMember(des => des.ProductName, opt => opt.MapFrom(src => src.Product.Title))
                .ForMember(des => des.Amount, opt => opt.MapFrom(src => src.Quantity * src.Product.Price));
        }
    }
}
