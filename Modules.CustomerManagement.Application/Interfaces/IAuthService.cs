using EmployeeManagementSystem.Application.Dtos.User;
using EmployeeManagementSystem.Domain.Shared;

namespace Modules.CustomerManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> Login(UserLoginDto loginDto);
        Task<ServiceResponse<string>> Register(UserRegisterDto registerDto);
        string GetCurrentUserId();
    }
}
