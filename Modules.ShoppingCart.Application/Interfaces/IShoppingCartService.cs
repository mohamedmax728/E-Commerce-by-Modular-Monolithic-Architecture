using EmployeeManagementSystem.Domain.Shared;
using Modules.ShoppingCart.Application.Dtos.ShoppingCart;
using Shared.Utilities.Models.Entities;
using Utilities.Shared.Services.GenericServices.IServices;

namespace Modules.ShoppingCart.Application.Interfaces
{
    public interface IShoppingCartService : IService<Cart>
    {
        Task<ServiceResponse<ShoppingCartDetailsDto>> Get();
    }
}
