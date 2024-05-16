using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Utilities.Models.Entities
{
    [Table("Order", Schema = "OrderManagement")]
    public class Order : AuditModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        // Navigation Properties
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public Payment Payment { get; set; }
    }
}
