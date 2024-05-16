using Shared.Utilities.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Utilities.Models.Entities
{
    [Table("Payment", Schema = "PaymentProcessing")]
    public class Payment : AuditModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public Order Order { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Provider { get; set; }
        public PaymentStatusEnum Status { get; set; }
        public string SessionId { get; set; }
    }
}
