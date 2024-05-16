using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Utilities.Models.Entities
{
    [Table("CartProduct", Schema = "ShoppingCart")]
    public class CartProduct : AuditModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
