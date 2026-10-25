using ClothingShop.Application.Abstractions;
using ClothingShop.Domain.Entities;
using ClothingShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordHasher hasher, string managerPassword)
    {
        await db.Database.EnsureCreatedAsync();
        if (await db.Users.AnyAsync())
            return;

        db.Users.Add(new User
        {
            Login = "manager",
            PasswordHash = hasher.Hash(managerPassword),
            Phone = "+70000000000",
            Address = "Склад",
            Role = UserRole.Manager
        });

        db.Clothes.AddRange(
            TShirt("Basic White T-Shirt", 990, "Белая футболка из хлопка"),
            TShirt("Basic Black T-Shirt", 990, "Чёрная футболка из хлопка"),
            TShirt("Oversize Print T-Shirt", 1590, "Оверсайз футболка с принтом"),
            new Clothing
            {
                Name = "Classic Hoodie", Price = 3490, Description = "Худи с капюшоном", Type = ClothingType.Hoodie,
                Sizes = Sizes()
            });

        await db.SaveChangesAsync();
    }

    private static Clothing TShirt(string name, decimal price, string description) => new()
    {
        Name = name, Price = price, Description = description, Type = ClothingType.TShirt, Sizes = Sizes()
    };

    private static List<ClothingSize> Sizes() =>
    [
        new() { Letter = "S", Number = 44, Quantity = 10 },
        new() { Letter = "M", Number = 48, Quantity = 15 },
        new() { Letter = "L", Number = 52, Quantity = 10 },
        new() { Letter = "XL", Number = 56, Quantity = 5 }
    ];
}
