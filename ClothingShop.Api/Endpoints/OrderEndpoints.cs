using System.Security.Claims;
using ClothingShop.Application.Dtos;
using ClothingShop.Application.Services;
using ClothingShop.Domain.Enums;

namespace ClothingShop.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("/api/orders").WithTags("Orders").RequireAuthorization();

        orders.MapPost("/", async (CreateOrderRequest request, ClaimsPrincipal user, OrderService service, CancellationToken ct) =>
        {
            var order = await service.CreateAsync(user.Id(), request, ct);
            return Results.Created($"/api/orders/{order.Id}", order);
        });

        // /api/orders?status=New
        orders.MapGet("/", async (OrderStatus status, ClaimsPrincipal user, OrderService service, CancellationToken ct) =>
            Results.Ok(await service.GetByStatusAsync(status, user.Id(), user.IsManager(), ct)));

        orders.MapPatch("/{id:int}/status",
                async (int id, UpdateOrderStatusRequest request, OrderService service, CancellationToken ct) =>
                    Results.Ok(await service.UpdateStatusAsync(id, request.Status, ct)))
            .RequireAuthorization(Policies.Manager);

        app.MapGet("/api/users/{userId:int}/orders",
                async (int userId, ClaimsPrincipal user, OrderService service, CancellationToken ct) =>
                    Results.Ok(await service.GetByUserAsync(userId, user.Id(), user.IsManager(), ct)))
            .WithTags("Orders")
            .RequireAuthorization();
    }
}
