namespace Modules.PaymentProcessing.Application.Dtos
{
    public class PaymentCreateDto
    {
        public int OrderId { get; set; }
        public int CartId { get; set; }
        public decimal Amount { get; set; }
        public string Provider { get; set; }
        public string? SessionId { get; set; }
    }
}
