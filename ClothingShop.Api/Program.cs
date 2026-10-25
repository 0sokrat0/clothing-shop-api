using System.Text;
using System.Text.Json.Serialization;
using ClothingShop.Api;
using ClothingShop.Api.Endpoints;
using ClothingShop.Application;
using ClothingShop.Application.Abstractions;
using ClothingShop.Domain.Enums;
using ClothingShop.Infrastructure;
using ClothingShop.Infrastructure.Persistence;
using ClothingShop.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    // Статусы и роли в JSON строками: "New", "Manager"
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwt.Issuer,
        ValidAudience = jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key))
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.Manager, p => p.RequireRole(nameof(UserRole.Manager)));

builder.Services.AddExceptionHandler<ErrorHandling>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(
        scope.ServiceProvider.GetRequiredService<AppDbContext>(),
        scope.ServiceProvider.GetRequiredService<IPasswordHasher>(),
        builder.Configuration["Seed:ManagerPassword"]!);
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapClothingEndpoints();
app.MapOrderEndpoints();

app.Run();
