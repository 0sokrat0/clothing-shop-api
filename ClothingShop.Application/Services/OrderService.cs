using ClothingShop.Application.Abstractions;
using ClothingShop.Application.Dtos;
using ClothingShop.Application.Exceptions;
using ClothingShop.Domain;
using ClothingShop.Domain.Entities;
using ClothingShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Application.Services;

public class OrderService(IAppDbContext db)
{
    public async Task<OrderDto> CreateAsync(int userId, CreateOrderRequest request, CancellationToken ct)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new DomainException("Заказ должен содержать хотя бы один товар");
        if (request.Items.Any(i => i.Quantity <= 0))
            throw new DomainException("Количество товара должно быть больше нуля");

        // Одинаковые размеры в запросе складываем в одну позицию
        var wanted = request.Items
            .GroupBy(i => i.SizeId)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantity));

        var ids = wanted.Keys.ToList();
        var sizes = await db.ClothingSizes
            .Include(s => s.Clothing)
            .Where(s => ids.Contains(s.Id))
            .ToListAsync(ct);

        var missing = wanted.Keys.Except(sizes.Select(s => s.Id)).ToList();
        if (missing.Count > 0)
            throw new NotFoundException($"Размеры не найдены: {string.Join(", ", missing)}");

        var order = new Order { UserId = userId };
        foreach (var size in sizes)
        {
            size.Reserve(wanted[size.Id]);
            order.Items.Add(new OrderItem { ClothingSize = size, Quantity = wanted[size.Id] });
        }

        // ponytail: без оптимистичной блокировки два одновременных заказа могут продать последний товар дважды; для прода добавить concurrency token на Quantity
        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);

        return await GetDtoAsync(order.Id, ct);
    }

    public async Task<List<OrderDto>> GetByStatusAsync(OrderStatus status, int currentUserId, bool isManager, CancellationToken ct)
    {
        var query = WithDetails().Where(o => o.Status == status);
        // Покупатель видит только свои заказы, менеджер видит все
        if (!isManager)
            query = query.Where(o => o.UserId == currentUserId);

        return await ToDtosAsync(query, ct);
    }

    public async Task<List<OrderDto>> GetByUserAsync(int userId, int currentUserId, bool isManager, CancellationToken ct)
    {
        if (!isManager && userId != currentUserId)
            throw new ForbiddenException("Можно смотреть только свои заказы");
        if (!await db.Users.AnyAsync(u => u.Id == userId, ct))
            throw new NotFoundException($"Пользователь {userId} не найден");

        return await ToDtosAsync(WithDetails().Where(o => o.UserId == userId), ct);
    }

    public async Task<OrderDto> UpdateStatusAsync(int orderId, OrderStatus status, CancellationToken ct)
    {
        if (!Enum.IsDefined(status))
            throw new DomainException("Неизвестный статус заказа");

        var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new NotFoundException($"Заказ {orderId} не найден");

        order.Status = status;
        await db.SaveChangesAsync(ct);
        return await GetDtoAsync(orderId, ct);
    }

    private IQueryable<Order> WithDetails() => db.Orders
        .AsNoTracking()
        .Include(o => o.User)
        .Include(o => o.Items).ThenInclude(i => i.ClothingSize).ThenInclude(s => s.Clothing);

    private async Task<OrderDto> GetDtoAsync(int orderId, CancellationToken ct) =>
        OrderDto.From(await WithDetails().FirstAsync(o => o.Id == orderId, ct));

    private static async Task<List<OrderDto>> ToDtosAsync(IQueryable<Order> query, CancellationToken ct) =>
        (await query.OrderByDescending(o => o.CreatedAt).ToListAsync(ct)).Select(OrderDto.From).ToList();
}
