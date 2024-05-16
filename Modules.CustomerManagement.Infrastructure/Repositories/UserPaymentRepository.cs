using EasyRepository.EFCore.Generic;
using Modules.CustomerManagement.Domain.Entities;
using Shared.Infrastructre.Context;

namespace Modules.CustomerManagement.Infrastructure.Repositories
{
    public class UserPaymentRepository : Repository<UserPayment>, IUserPaymentRepository
    {
        public UserPaymentRepository(AppDbContext context) : base(context)
        {
        }
    }
}
