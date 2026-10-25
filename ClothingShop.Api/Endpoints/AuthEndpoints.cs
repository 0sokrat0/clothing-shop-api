using ClothingShop.Application.Dtos;
using ClothingShop.Application.Services;

namespace ClothingShop.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register", async (RegisterRequest request, AuthService auth, CancellationToken ct) =>
            Results.Ok(await auth.RegisterAsync(request, ct)));

        group.MapPost("/login", async (LoginRequest request, AuthService auth, CancellationToken ct) =>
            Results.Ok(await auth.LoginAsync(request, ct)));
    }
}
