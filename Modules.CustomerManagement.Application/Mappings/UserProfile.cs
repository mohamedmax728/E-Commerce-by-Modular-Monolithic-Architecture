using AutoMapper;
using EmployeeManagementSystem.Application.Dtos.User;
using Modules.CustomerManagement.Application.Dtos.User;
using Modules.CustomerManagement.Domain.Entities;

namespace Modules.CustomerManagement.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserCreateDto, User>();
            CreateMap<User, UserLoginDto>();
        }
    }
}
