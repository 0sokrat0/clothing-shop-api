using ClothingShop.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClothingShop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services) => services
        .AddScoped<AuthService>()
        .AddScoped<ClothingService>()
        .AddScoped<OrderService>();
}
