using ClothingShop.Domain.Enums;

namespace ClothingShop.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Customer;
    public List<Order> Orders { get; set; } = [];
}
