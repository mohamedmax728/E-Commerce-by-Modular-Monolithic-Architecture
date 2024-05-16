using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Utilities.Models.Entities
{
    [Table("Product", Schema = "ProductCatalog")]
    public class Product : AuditModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
