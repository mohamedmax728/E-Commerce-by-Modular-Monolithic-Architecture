using Modules.ShoppingCart.Application.Dtos.CartProduct;
using Shared.Utilities.Models;

namespace Modules.ShoppingCart.Application.Dtos.ShoppingCart
{
    public class ShoppingCartDetailsDto : AuditModelVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public List<CartProductDetailsDto> CartProducts { get; set; }
    }
}
