using Shared.Utilities.Models;
using Shared.Utilities.Models.Enums;

namespace Modules.PaymentProcessing.Application.Dtos
{
    public class PaymentDetailsDto : AuditModelVM
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Provider { get; set; }
        public PaymentStatusEnum Status { get; set; }
    }
}
