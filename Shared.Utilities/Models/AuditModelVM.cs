namespace Shared.Utilities.Models
{
    public class AuditModelVM
    {
        public string? CreatedBy { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public int CreatedByUserId { get; set; }
        public int ModifiedById { get; set; }
    }
}
