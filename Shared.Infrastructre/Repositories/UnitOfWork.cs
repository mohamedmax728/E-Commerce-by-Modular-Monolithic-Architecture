using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Modules.CustomerManagement.Domain.Entities;
using Shared.Infrastructre.Context;
using Shared.Utilities.Models;
using Shared.Utilities.Models.Entities;
using System.Security.Claims;

namespace EasyRepository.EFCore.Generic;

/// <summary>
/// Implementation of Unit of work pattern
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    public Lazy<IRepository<Order>> Orders { get; private set; }
    public Lazy<IRepository<User>> Users { get; private set; }
    public Lazy<IRepository<UserPayment>> UserPayments { get; private set; }
    public Lazy<IRepository<Cart>> Carts { get; private set; }
    public Lazy<IRepository<Payment>> Payments { get; private set; }
    public Lazy<IRepository<Product>> Products { get; private set; }
    public Lazy<IRepository<CartProduct>> CartProducts { get; private set; }

    public UnitOfWork(AppDbContext context)
    {
        this.context = context;
        Orders = new Lazy<IRepository<Order>>(() => new Repository<Order>(context));
        Users = new Lazy<IRepository<User>>(() => new Repository<User>(context));
        UserPayments = new Lazy<IRepository<UserPayment>>(() => new Repository<UserPayment>(context));
        Carts = new Lazy<IRepository<Cart>>(() => new Repository<Cart>(context));
        CartProducts = new Lazy<IRepository<CartProduct>>(() => new Repository<CartProduct>(context));
        Payments = new Lazy<IRepository<Payment>>(() => new Repository<Payment>(context));
        Products = new Lazy<IRepository<Product>>(() => new Repository<Product>(context));
    }
    private readonly AppDbContext context;

    public int SaveChanges()
    {
        HandleAuditModel();
        return context.SaveChanges();
    }

    public async Task<int> SaveChangesAsync()
    {
        HandleAuditModel();
        return await context.SaveChangesAsync();
    }

    private void HandleAuditModel()
    {
        IEnumerable<EntityEntry<AuditModel>> changes = context.ChangeTracker.Entries<AuditModel>()
                     .Where(t => t.State == EntityState.Added || t.State == EntityState.Modified);

        foreach (EntityEntry<AuditModel> entity in changes)
        {
            if (entity.State == EntityState.Added)
            {
                entity.Entity.CreationDate = DateTime.Now;
                entity.Entity.CreatedByUserId =
                    int.Parse(context.GetService<IHttpContextAccessor>()?.HttpContext?
                                    .User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                entity.Entity.CreatedBy
                    = context.GetService<IHttpContextAccessor>()?.HttpContext?
                                    .User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

            }
            if (entity.State == EntityState.Modified)
            {
                entity.Entity.ModifiedDate = DateTime.Now;
                entity.Entity.ModifiedByUserId = int.Parse(context.GetService<IHttpContextAccessor>()?.HttpContext?
                    .User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                entity.Entity.ModifiedBy
                    = context.GetService<IHttpContextAccessor>()?.HttpContext?
                                    .User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            }
        }
    }

    public IDbContextTransaction BeginTransaction()
    {
        return context.Database.BeginTransaction();
    }
    public IRepository<TEntity> GetRepo<TEntity>() where TEntity : class, new()
    {
        //var assembly = this.GetType().Assembly;

        //var repo = assembly.GetTypes().FirstOrDefault(s => s.IsClass && s.IsSubclassOf(typeof(Repository<TEntity>)));
        //return repo == null ? null : (Repository<TEntity>)Activator.CreateInstance(repo, context);
        var x =
            GetType().GetProperties().Any(s => ReferenceEquals(s.PropertyType, typeof(Lazy<IRepository<TEntity>>)));

        return x ? (Repository<TEntity>)Activator.CreateInstance(typeof(Repository<TEntity>), context) : null;
    }

}