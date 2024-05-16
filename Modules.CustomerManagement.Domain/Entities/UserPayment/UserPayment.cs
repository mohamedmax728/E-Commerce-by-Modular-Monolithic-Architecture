using Modules.CustomerManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
namespace Modules.CustomerManagement.Domain.Entities
{
    [Table("UserPayment", Schema = "CustomerManagement")]
    public class UserPayment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public PaymentProviderEnum PaymentProvider { get; set; }
        public DateOnly ExpireDate { get; set; }
        public int AccountNumber { get; set; }
    }
}
