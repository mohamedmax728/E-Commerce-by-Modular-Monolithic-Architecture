using EmployeeManagementSystem.Application.Dtos.User;

namespace Modules.CustomerManagement.Application.Dtos.User
{
    public class UserCreateDto : UserRegisterDto
    {
        public bool IsAdmin { get; set; }
    }
}
