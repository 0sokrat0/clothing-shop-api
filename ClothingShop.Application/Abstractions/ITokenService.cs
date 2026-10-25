using ClothingShop.Domain.Entities;

namespace ClothingShop.Application.Abstractions;

public interface ITokenService
{
    string CreateToken(User user);
}
