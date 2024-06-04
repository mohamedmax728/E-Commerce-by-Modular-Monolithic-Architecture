using Microsoft.EntityFrameworkCore;
using Modules.CustomerManagement.Domain.Entities;
using Shared.Utilities.Models.Entities;

namespace Shared.Infrastructre.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {

        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<UserPayment> UserPayments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CartProduct> CartProducts { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost,1433;Database=ModularMonoECommerce;Integrated Security=True;TrustServerCertificate=Yes;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasIndex(s => s.CartId).IsUnique();

            modelBuilder.Entity<Payment>()
                .HasIndex(s => s.OrderId).IsUnique();

            modelBuilder.Entity<Order>()
                .HasOne(s => s.Cart)
                .WithOne(s => s.Order)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(s => s.Order)
                .WithOne(s => s.Payment)
                .OnDelete(DeleteBehavior.Restrict);

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,2)");
            }
        }
    }
}
