namespace Modules.ShoppingCart.Application.Dtos.ShoppingCart
{
    public class ShoppingCartUpdateDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}
