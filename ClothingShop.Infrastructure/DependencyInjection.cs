using ClothingShop.Application.Abstractions;
using ClothingShop.Infrastructure.Persistence;
using ClothingShop.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClothingShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var jwt = config.GetSection("Jwt").Get<JwtOptions>() ?? throw new InvalidOperationException("Нет секции Jwt");

        return services
            .AddDbContext<AppDbContext>(o => o.UseSqlite(config.GetConnectionString("Default")))
            .AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>())
            .AddSingleton<IPasswordHasher, PasswordHasher>()
            .AddSingleton(jwt)
            .AddSingleton<ITokenService, JwtTokenService>();
    }
}
