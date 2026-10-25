using ClothingShop.Domain.Enums;

namespace ClothingShop.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.New;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public List<OrderItem> Items { get; set; } = [];
}
