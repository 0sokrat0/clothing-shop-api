using ClothingShop.Application.Abstractions;
using ClothingShop.Application.Dtos;
using ClothingShop.Application.Exceptions;
using ClothingShop.Domain;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Application.Services;

public class ClothingService(IAppDbContext db)
{
    public async Task<List<ClothingDto>> GetAllAsync(string? search, CancellationToken ct)
    {
        var query = db.Clothes.Include(c => c.Sizes).AsNoTracking();
        // ponytail: LIKE в SQLite регистронезависим только для латиницы; на PostgreSQL заменить на EF.Functions.ILike
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => EF.Functions.Like(c.Name, $"%{search.Trim()}%"));

        var clothes = await query.OrderBy(c => c.Id).ToListAsync(ct);
        return clothes.Select(ClothingDto.From).ToList();
    }

    public async Task<ClothingDto> SetAvailabilityAsync(int id, bool isAvailable, CancellationToken ct)
    {
        var clothing = await db.Clothes.Include(c => c.Sizes).FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException($"Товар {id} не найден");

        clothing.IsAvailable = isAvailable;
        await db.SaveChangesAsync(ct);
        return ClothingDto.From(clothing);
    }

    public async Task<SizeDto> UpdateSizeQuantityAsync(int sizeId, int quantity, CancellationToken ct)
    {
        if (quantity < 0)
            throw new DomainException("Количество не может быть отрицательным");

        var size = await db.ClothingSizes.FirstOrDefaultAsync(s => s.Id == sizeId, ct)
            ?? throw new NotFoundException($"Размер {sizeId} не найден");

        size.Quantity = quantity;
        await db.SaveChangesAsync(ct);
        return SizeDto.From(size);
    }
}
