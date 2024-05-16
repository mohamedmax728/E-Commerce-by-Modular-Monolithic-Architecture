using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.CustomerManagement.Domain.Entities
{
    [Table("User", Schema = "CustomerManagement")]
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string? CustomerId { get; set; }
        public bool IsAdmin { get; set; }
        public byte[] PasswordHash { get; set; } = new byte[0];
        public byte[] PasswordSalt { get; set; } = new byte[0];
    }
}
