namespace Modules.ShoppingCart.Application.Dtos.CartProduct
{
    public class CartProductDetailsDto
    {
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
    }
}
