using System.Security.Claims;
using System.Text;
using ClothingShop.Application.Abstractions;
using ClothingShop.Domain.Entities;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ClothingShop.Infrastructure.Security;

public class JwtTokenService(JwtOptions options) : ITokenService
{
    public string CreateToken(User user) => new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        }),
        Issuer = options.Issuer,
        Audience = options.Audience,
        Expires = DateTime.UtcNow.AddHours(options.LifetimeHours),
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)), SecurityAlgorithms.HmacSha256)
    });
}
