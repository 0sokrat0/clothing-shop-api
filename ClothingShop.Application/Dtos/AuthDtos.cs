namespace ClothingShop.Application.Dtos;

public record RegisterRequest(string Login, string Password, string Phone, string Address);

public record LoginRequest(string Login, string Password);

public record AuthResponse(int UserId, string Login, string Role, string Token);
