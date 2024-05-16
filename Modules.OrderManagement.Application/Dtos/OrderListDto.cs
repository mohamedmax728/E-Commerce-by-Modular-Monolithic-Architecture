using Shared.Utilities.Models;

namespace Modules.OrderManagement.Application.Dtos
{
    public class OrderListDto : AuditModelVM
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
