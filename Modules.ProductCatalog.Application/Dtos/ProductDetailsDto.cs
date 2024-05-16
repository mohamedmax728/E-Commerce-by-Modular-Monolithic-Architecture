using Shared.Utilities.Models;

namespace Modules.ProductCatalog.Application.Dtos
{
    public class ProductDetailsDto : AuditModelVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
