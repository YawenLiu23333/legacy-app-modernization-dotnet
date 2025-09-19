namespace ModernizedApp.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Products.Any()) return;
        db.Products.AddRange(new[] {
            new Product { Name = "Widget A", Price = 9.99m },
            new Product { Name = "Widget B", Price = 19.99m },
            new Product { Name = "Widget C", Price = 29.99m }
        });
        await db.SaveChangesAsync();
    }
}
