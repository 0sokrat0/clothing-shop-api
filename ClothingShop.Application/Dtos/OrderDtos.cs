using ClothingShop.Domain.Entities;
using ClothingShop.Domain.Enums;

namespace ClothingShop.Application.Dtos;

public record OrderItemRequest(int SizeId, int Quantity);

public record CreateOrderRequest(List<OrderItemRequest> Items);

public record UpdateOrderStatusRequest(OrderStatus Status);

public record OrderItemDto(int ClothingId, string ClothingName, string SizeLetter, int SizeNumber, int Quantity, decimal Price);

public record OrderDto(int Id, DateTime CreatedAt, string Status, int UserId, string UserLogin, List<OrderItemDto> Items)
{
    public static OrderDto From(Order o) => new(
        o.Id, o.CreatedAt, o.Status.ToString(), o.UserId, o.User.Login,
        o.Items.Select(i => new OrderItemDto(
            i.ClothingSize.ClothingId, i.ClothingSize.Clothing.Name,
            i.ClothingSize.Letter, i.ClothingSize.Number,
            i.Quantity, i.ClothingSize.Clothing.Price)).ToList());
}
