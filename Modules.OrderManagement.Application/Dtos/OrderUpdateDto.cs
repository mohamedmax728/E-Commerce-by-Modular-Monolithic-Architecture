namespace Modules.OrderManagement.Application.Dtos
{
    public class OrderUpdateDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
}
