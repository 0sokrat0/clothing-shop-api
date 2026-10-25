using ClothingShop.Domain.Entities;

namespace ClothingShop.Application.Dtos;

public record SizeDto(int Id, string Letter, int Number, int Quantity)
{
    public static SizeDto From(ClothingSize s) => new(s.Id, s.Letter, s.Number, s.Quantity);
}

public record ClothingDto(
    int Id, string Name, decimal Price, string Description,
    int PurchaseCount, string Type, bool IsAvailable, List<SizeDto> Sizes)
{
    public static ClothingDto From(Clothing c) => new(
        c.Id, c.Name, c.Price, c.Description, c.PurchaseCount,
        c.Type.ToString(), c.IsAvailable, c.Sizes.OrderBy(s => s.Id).Select(SizeDto.From).ToList());
}

public record UpdateQuantityRequest(int Quantity);
