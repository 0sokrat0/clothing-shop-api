namespace ClothingShop.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public int ClothingSizeId { get; set; }
    public ClothingSize ClothingSize { get; set; } = null!;
}
