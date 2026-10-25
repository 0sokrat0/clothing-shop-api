using ClothingShop.Application.Abstractions;
using ClothingShop.Application.Dtos;
using ClothingShop.Application.Exceptions;
using ClothingShop.Domain;
using ClothingShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClothingShop.Application.Services;

public class AuthService(IAppDbContext db, IPasswordHasher hasher, ITokenService tokens)
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Phone) || string.IsNullOrWhiteSpace(request.Address))
            throw new DomainException("Логин, телефон и адрес обязательны");
        if (request.Password is null || request.Password.Length < 6)
            throw new DomainException("Пароль должен быть не короче 6 символов");

        var login = request.Login.Trim();
        if (await db.Users.AnyAsync(u => u.Login == login, ct))
            throw new DomainException("Логин уже занят");

        // Роль не берётся из запроса: регистрация всегда создаёт покупателя
        var user = new User
        {
            Login = login,
            PasswordHash = hasher.Hash(request.Password),
            Phone = request.Phone.Trim(),
            Address = request.Address.Trim()
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return new AuthResponse(user.Id, user.Login, user.Role.ToString(), tokens.CreateToken(user));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Login == request.Login, ct);
        if (user is null || !hasher.Verify(request.Password ?? "", user.PasswordHash))
            throw new UnauthorizedException("Неверный логин или пароль");

        return new AuthResponse(user.Id, user.Login, user.Role.ToString(), tokens.CreateToken(user));
    }
}
