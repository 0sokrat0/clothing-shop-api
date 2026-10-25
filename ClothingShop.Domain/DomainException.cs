namespace ClothingShop.Domain;

// Нарушение бизнес-правила: API отвечает 400
public class DomainException(string message) : Exception(message);
