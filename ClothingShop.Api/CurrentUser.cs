using System.Security.Claims;
using ClothingShop.Domain.Enums;

namespace ClothingShop.Api;

public static class CurrentUser
{
    public static int Id(this ClaimsPrincipal user) => int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public static bool IsManager(this ClaimsPrincipal user) => user.IsInRole(nameof(UserRole.Manager));
}
