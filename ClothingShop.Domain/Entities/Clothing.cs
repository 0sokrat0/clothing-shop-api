using ClothingShop.Domain.Enums;

namespace ClothingShop.Domain.Entities;

public class Clothing
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public int PurchaseCount { get; set; }
    public ClothingType Type { get; set; }
    public bool IsAvailable { get; set; } = true;
    public List<ClothingSize> Sizes { get; set; } = [];
}
