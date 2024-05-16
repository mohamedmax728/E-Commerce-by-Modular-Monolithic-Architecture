using Shared.Utilities.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Utilities.Models.Entities
{
    [Table("Cart", Schema = "ShoppingCart")]
    public class Cart : AuditModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public CartStatusEnum Status { get; set; }
        public virtual ICollection<CartProduct> CartProducts { get; set; }
        public virtual Order Order { get; set; }
    }
}
