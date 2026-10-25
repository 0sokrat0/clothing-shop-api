using ClothingShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<User> Users { get; }
    DbSet<Clothing> Clothes { get; }
    DbSet<ClothingSize> ClothingSizes { get; }
    DbSet<Order> Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
