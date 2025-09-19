namespace ModernizedApp.Data;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
