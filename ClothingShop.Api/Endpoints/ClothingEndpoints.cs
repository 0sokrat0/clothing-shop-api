using ClothingShop.Application.Dtos;
using ClothingShop.Application.Services;

namespace ClothingShop.Api.Endpoints;

public static class ClothingEndpoints
{
    public static void MapClothingEndpoints(this IEndpointRouteBuilder app)
    {
        var clothes = app.MapGroup("/api/clothes").WithTags("Clothes");

        // Весь каталог или поиск по части названия: /api/clothes?search=shirt
        clothes.MapGet("/", async (string? search, ClothingService service, CancellationToken ct) =>
            Results.Ok(await service.GetAllAsync(search, ct)));

        clothes.MapPost("/{id:int}/remove-from-sale", async (int id, ClothingService service, CancellationToken ct) =>
            Results.Ok(await service.SetAvailabilityAsync(id, false, ct)))
            .RequireAuthorization(Policies.Manager);

        clothes.MapPost("/{id:int}/return-to-sale", async (int id, ClothingService service, CancellationToken ct) =>
            Results.Ok(await service.SetAvailabilityAsync(id, true, ct)))
            .RequireAuthorization(Policies.Manager);

        app.MapPatch("/api/sizes/{id:int}/quantity",
                async (int id, UpdateQuantityRequest request, ClothingService service, CancellationToken ct) =>
                    Results.Ok(await service.UpdateSizeQuantityAsync(id, request.Quantity, ct)))
            .WithTags("Clothes")
            .RequireAuthorization(Policies.Manager);
    }
}
