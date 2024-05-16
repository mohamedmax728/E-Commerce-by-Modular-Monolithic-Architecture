using Microsoft.EntityFrameworkCore.Storage;
using Modules.CustomerManagement.Domain.Entities;
using Shared.Utilities.Models;
using Shared.Utilities.Models.Entities;

namespace EasyRepository.EFCore.Generic;

/// <summary>
/// Abstraction of Unit Of Work pattern
/// </summary>
public interface IUnitOfWork
{
    public Lazy<IRepository<Order>> Orders { get; }
    public Lazy<IRepository<User>> Users { get; }
    public Lazy<IRepository<UserPayment>> UserPayments { get; }
    public Lazy<IRepository<Cart>> Carts { get; }
    public Lazy<IRepository<Payment>> Payments { get; }
    public Lazy<IRepository<Product>> Products { get; }
    public Lazy<IRepository<CartProduct>> CartProducts { get; }
    int SaveChanges();
    Task<int> SaveChangesAsync();
    IDbContextTransaction BeginTransaction();
    IRepository<TEntity> GetRepo<TEntity>() where TEntity : class, new();
}