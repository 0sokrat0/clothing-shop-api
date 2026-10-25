using ClothingShop.Domain;

namespace ClothingShop.Domain.Entities;

public class ClothingSize
{
    public int Id { get; set; }
    public string Letter { get; set; } = string.Empty;
    public int Number { get; set; }
    public int Quantity { get; set; }

    public int ClothingId { get; set; }
    public Clothing Clothing { get; set; } = null!;

    public void Reserve(int quantity)
    {
        if (!Clothing.IsAvailable)
            throw new DomainException($"Товар «{Clothing.Name}» снят с продажи");
        if (quantity > Quantity)
            throw new DomainException($"Размера {Letter} товара «{Clothing.Name}» осталось {Quantity} шт.");

        Quantity -= quantity;
        Clothing.PurchaseCount += quantity;
    }
}
