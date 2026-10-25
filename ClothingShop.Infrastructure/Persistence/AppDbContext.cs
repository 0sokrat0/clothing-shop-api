using ClothingShop.Application.Abstractions;
using ClothingShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Clothing> Clothes => Set<Clothing>();
    public DbSet<ClothingSize> ClothingSizes => Set<ClothingSize>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>(e =>
        {
            e.HasIndex(u => u.Login).IsUnique();
            e.Property(u => u.Login).HasMaxLength(50);
            e.Property(u => u.Phone).HasMaxLength(20);
            e.Property(u => u.Role).HasConversion<string>();
        });

        b.Entity<Clothing>(e =>
        {
            e.Property(c => c.Name).HasMaxLength(100);
            e.Property(c => c.Price).HasPrecision(10, 2);
            e.Property(c => c.Type).HasConversion<string>();
        });

        b.Entity<ClothingSize>(e =>
        {
            e.Property(s => s.Letter).HasMaxLength(5);
            e.HasIndex(s => new { s.ClothingId, s.Letter }).IsUnique();
            e.HasOne(s => s.Clothing).WithMany(c => c.Sizes).HasForeignKey(s => s.ClothingId);
        });

        b.Entity<Order>(e =>
        {
            e.Property(o => o.Status).HasConversion<string>();
            e.HasOne(o => o.User).WithMany(u => u.Orders).HasForeignKey(o => o.UserId);
        });

        b.Entity<OrderItem>(e =>
        {
            e.HasOne(i => i.Order).WithMany(o => o.Items).HasForeignKey(i => i.OrderId);
            // Размер нельзя удалить, пока он есть в заказах
            e.HasOne(i => i.ClothingSize).WithMany().HasForeignKey(i => i.ClothingSizeId).OnDelete(DeleteBehavior.Restrict);
        });
    }
}
