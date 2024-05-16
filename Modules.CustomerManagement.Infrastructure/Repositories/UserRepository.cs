using EasyRepository.EFCore.Generic;
using Modules.CustomerManagement.Domain.Entities;
using Shared.Infrastructre.Context;

namespace Modules.CustomerManagement.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }
}
