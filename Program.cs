using System.Text.Json;
using EFCoreTraining;
using Microsoft.EntityFrameworkCore;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== EF Core Training ===");
    Console.WriteLine("1. Add Product");
    Console.WriteLine("2. List Active Products");
    Console.WriteLine("3. Find Product by ID");
    Console.WriteLine("4. Update Product");
    Console.WriteLine("5. Permanently Delete Product");
    Console.WriteLine("6. Create Category and Assign Products");
    Console.WriteLine("7. Increase Prices Below 1000");
    Console.WriteLine("8. Delete Products with Non-Positive Prices");
    Console.WriteLine("9. List Products with Categories");
    Console.WriteLine("10. Increase Prices in a Category");
    Console.WriteLine("11. Run Sample Price Transaction");
    Console.WriteLine("12. Find Products Above a Price (SQL)");
    Console.WriteLine("13. Rename Category (SQL)");
    Console.WriteLine("14. Soft Delete Product");
    Console.WriteLine("15. List All Products, Including Deleted");
    Console.WriteLine("16. Read Categories with Split Query");
    Console.WriteLine("17. Read Categories with Filtered Include");
    Console.WriteLine("18. Simulate Concurrency Conflict");
    Console.WriteLine("19. Insert Product with JSONB Metadata");
    Console.WriteLine("20. Search Products by JSONB Containment");
    Console.WriteLine("0. Exit");

    int choice = ReadInt("Choice: ");
    if (choice == 0)
    {
        break;
    }

    try
    {
        switch (choice)
        {
            case 1:
                Create_Product(ReadText("Product name: "), ReadDecimal("Price: "));
                break;
            case 2:
                Read_All();
                break;
            case 3:
                Read_Product(ReadInt("Product ID: "));
                break;
            case 4:
                Update_Product(ReadInt("Product ID: "), ReadText("New name: "), ReadDecimal("New price: "));
                break;
            case 5:
                Delete_Product(ReadInt("Product ID to permanently delete: "));
                break;
            case 6:
                Category_Add(ReadText("Category name: "));
                break;
            case 7:
                Update_Bulk();
                break;
            case 8:
                Delete_Bulk();
                break;
            case 9:
                Read_Products_Fast();
                break;
            case 10:
                Increase_Category_Prices(ReadInt("Category ID: "), ReadDecimal("Increase percentage: "));
                break;
            case 11:
                Transfer_Stock_Or_Process_Order();
                break;
            case 12:
                Read_Products_Raw_Sql(ReadDecimal("Minimum price: "));
                break;
            case 13:
                Update_Category_Name_Raw_Sql(ReadInt("Category ID: "), ReadText("New category name: "));
                break;
            case 14:
                Soft_Delete_Product(ReadInt("Product ID to soft delete: "));
                break;
            case 15:
                Read_All_Including_Deleted();
                break;
            case 16:
                Read_Categories_With_Split_Query();
                break;
            case 17:
                Read_Categories_With_Filtered_Include();
                break;
            case 18:
                Simulate_Concurrency_Conflict(ReadInt("Product ID for concurrency demo: "));
                break;
            case 19:
                Add_Product_With_Jsonb(
                    ReadText("Product name: "),
                    ReadDecimal("Price: "),
                    ReadJson("Enter JSON metadata: "));
                break;
            case 20:
                Search_Products_By_Jsonb(
                    ReadJson("Enter JSON containment filter: "));
                break;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Operation failed: {ex.Message}");
    }
}

string ReadText(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? value = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value.Trim();
        }
        Console.WriteLine("Value cannot be empty.");
    }
}

int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out int value))
        {
            return value;
        }
        Console.WriteLine("Please enter a valid integer.");
    }
}

string ReadJson(string prompt)
{
    while (true)
    {
        string json = ReadText(prompt);
        try
        {
            using var document = JsonDocument.Parse(json);
            return json;
        }
        catch (JsonException)
        {
            Console.WriteLine("Invalid JSON. Please enter a valid JSON object.");
        }
    }
}
decimal ReadDecimal(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (decimal.TryParse(Console.ReadLine(), out decimal value))
        {
            return value;
        }
        Console.WriteLine("Please enter a valid number.");
    }
}
void Create_Product(string name, decimal price)
{
    using (var ctx = new AppDbContext())
    {
        var product = new Product
        {
            Name = name,
            Price = price,
            Tags = new List<string> { "apple", "smartphone", "5g", "flagship" }
        };

        ctx.Products.Add(product);
        ctx.SaveChanges();
        Console.WriteLine($"Product added with ID: {product.Id}");
    }
}
List<Product> Read_All()
{
    using (var ctx = new AppDbContext())
    {
        var products = ctx.Products.AsNoTracking().ToList();
        foreach (var p in products)
        {
            Console.WriteLine($"{p.Name}-{p.Price}-{p.Id}");
        }
        return products;
    }
}
Product Read_Product(int id)
{
    using (var ctx = new AppDbContext())
    {
        var product = ctx.Products
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
        Console.WriteLine($"Product Name: {product?.Name}, Product Price: {product?.Price}");
        return product!;
    }
}

void Update_Product(int id, string name, decimal price)
{
    using (var ctx = new AppDbContext())
    {
        var product = ctx.Products.FirstOrDefault(x => x.Id == id);
        if (product is not null)
        {
            product.Name = name;
            product.Price = price;
            product.Tags = new List<string> { "apple", "smartphone", "5g", "flagship" };
            ctx.SaveChanges();
            Console.WriteLine($"Product Updated - Product Name: {product?.Name}, Product Price: {product?.Price}");
        }
    }
}
void Delete_Product(int id)
{
    using (var ctx = new AppDbContext())
    {
        var product = ctx.Products.FirstOrDefault(x => x.Id == id);
        if (product is not null)
        {
            ctx.Products.Remove(product);
            ctx.SaveChanges();
            Console.WriteLine("Product deleted successfully!");
        }
    }
}

void Category_Add(string name)
{
    using (var ctx = new AppDbContext())
    {
        var products = ctx.Products.Where(x => x.Id < 10).ToList();

        if (!products.Any())
        {
            Console.WriteLine("No products found to assign!");
            return;
        }

        var electronics = new Category
        {
            Name = name,
            Products = products
        };

        ctx.Categories.Add(electronics);
        ctx.SaveChanges();
        Console.WriteLine($"Category '{name}' created and {products.Count} products assigned successfully!");

    }
}

void Update_Bulk()
{
    using (var ctx = new AppDbContext())
    {
        // Directly translates to:
        // UPDATE "Products" SET "Price" = "Price" * 1.1 WHERE "Price" < 1000;
        int affectedRows = ctx.Products
            .Where(p => p.Price < 1000)
            .ExecuteUpdate(s => s.SetProperty(p => p.Price, p => p.Price * 1.1m));

        Console.WriteLine($"Bulk Updated: {affectedRows} products updated.");
    }
}

void Delete_Bulk()
{
    using (var ctx = new AppDbContext())
    {
        // Directly translates to:
        // DELETE FROM "Products" WHERE "Price" <= 0;
        int deletedRows = ctx.Products
            .Where(p => p.Price <= 0)
            .ExecuteDelete();

        Console.WriteLine($"Bulk Deleted: {deletedRows} products removed.");
    }
}

void Read_Products_Fast()
{
    using var ctx = new AppDbContext();

    var products = ctx.Products
        .AsNoTracking()
        .Include(p => p.Category)
        .ToList();

    Console.WriteLine($"--- Fast Read (Total: {products.Count}) ---");
    foreach (var p in products)
    {
        var catName = p.Category?.Name ?? "No Category";
        Console.WriteLine($"ID: {p.Id} | Name: {p.Name} | Price: {p.Price:C} | Category: {catName}");
    }
}

void Increase_Category_Prices(int categoryId, decimal percentage)
{
    using var ctx = new AppDbContext();

    decimal factor = 1 + (percentage / 100m);

    int rows = ctx.Products
        .Where(p => p.categoryId == categoryId)
        .ExecuteUpdate(setter => setter.SetProperty(p => p.Price, p => p.Price * factor));

    Console.WriteLine($"[Bulk Update] Successfully increased prices for Category {categoryId}. Affected rows: {rows}");
}

void Transfer_Stock_Or_Process_Order()
{
    using var ctx = new AppDbContext();

    // 1. Begin the transaction
    using var transaction = ctx.Database.BeginTransaction();

    try
    {
        // Step A: Modify product
        var product = ctx.Products.First();
        product.Price += 100;
        ctx.SaveChanges();

        // Step B: Bulk update another set of records
        ctx.Products
           .Where(p => p.Id == 2)
           .ExecuteUpdate(s => s.SetProperty(p => p.Price, p => p.Price - 50));

        // Step C: If everything succeeded, commit!
        transaction.Commit();
        Console.WriteLine("[Transaction] Successfully committed all operations!");
    }
    catch (Exception ex)
    {
        // Step D: Rollback changes in PostgreSQL if any failure occurs
        transaction.Rollback();
        Console.WriteLine($"[Transaction Failed] Rolled back! Reason: {ex.Message}");
    }
}


void Read_Products_Raw_Sql(decimal minPrice)
{
    using var ctx = new AppDbContext();
    // Interpolated FormattableString values are parameterized to prevent SQL injection.
    var expensiveProducts = ctx.Database
        .SqlQuery<ProductPriceDto>($"SELECT \"Name\", \"Price\" FROM \"Products\" WHERE \"Price\" > {minPrice}")
        .ToList();

    Console.WriteLine($"--- Products with Price > {minPrice} (SqlQuery) ---");
    foreach (var item in expensiveProducts)
    {
        Console.WriteLine($"Name: {item.Name} | Price: {item.Price:C}");
    }
}
void Update_Category_Name_Raw_Sql(int categoryId, string newCategoryName)
{
    using var ctx = new AppDbContext();

    // Values are sent as parameters through Npgsql.
    int rowsAffected = ctx.Database.ExecuteSql(
        $"UPDATE \"Categories\" SET \"Name\" = {newCategoryName} WHERE \"Id\" = {categoryId}"
    );

    if (rowsAffected > 0)
    {
        Console.WriteLine($"[ExecuteSql] Successfully updated category {categoryId} to '{newCategoryName}'. Rows affected: {rowsAffected}");
    }
    else
    {
        Console.WriteLine($"[ExecuteSql] No category found with ID: {categoryId}");
    }
}
void Soft_Delete_Product(int id)
{
    using var ctx = new AppDbContext();

    // Find the product.
    var product = ctx.Products.FirstOrDefault(p => p.Id == id);
    if (product is not null)
    {
        product.IsDeleted = true;
        ctx.SaveChanges();
        Console.WriteLine($"[Soft Delete] Product ID {id} ('{product.Name}') marked as deleted.");
    }
    else
    {
        Console.WriteLine($"[Soft Delete] Product with ID {id} not found.");
    }
}
void Read_All_Including_Deleted()
{
    using var ctx = new AppDbContext();
    var allProducts = ctx.Products
        .IgnoreQueryFilters()
        .AsNoTracking()
        .ToList();

    Console.WriteLine($"--- All Products Including Soft-Deleted (Total: {allProducts.Count}) ---");
    foreach (var p in allProducts)
    {
        var status = p.IsDeleted ? "[DELETED]" : "[ACTIVE]";
        Console.WriteLine($"{status} ID: {p.Id} | Name: {p.Name} | Price: {p.Price:C}");
    }
}

void Read_Categories_With_Split_Query()
{
    using var ctx = new AppDbContext();

    // AsSplitQuery sends two separate queries:
    // Query 1: Select all categories
    // Query 2: Select all related products
    var categories = ctx.Categories
        .AsNoTracking()
        .Include(c => c.Products)
        .AsSplitQuery()
        .ToList();

    Console.WriteLine($"\n--- Split Query Result (Categories: {categories.Count}) ---");
    foreach (var cat in categories)
    {
        Console.WriteLine($"Category: {cat.Name} (Products Count: {cat.Products.Count})");
        foreach (var prod in cat.Products.Take(3)) // Show only the first three products for brevity
        {
            Console.WriteLine($"   -> Product: {prod.Name} | {prod.Price:C}");
        }
    }
}
void Read_Categories_With_Filtered_Include()
{
    using var ctx = new AppDbContext();

    var categories = ctx.Categories
        .AsNoTracking()
        .Include(c => c.Products.Where(p => p.Price >= 1000)) // Filter the included child collection
        .ToList();

    Console.WriteLine("\n--- Categories with Expensive Products Only (>= 1000) ---");
    foreach (var cat in categories)
    {
        Console.WriteLine($"Category: {cat.Name} | Expensive Products: {cat.Products.Count}");
    }
}

void Simulate_Concurrency_Conflict(int productId)
{
    Console.WriteLine($"\n--- Simulating Concurrency Conflict for Product ID: {productId} ---");

    // Two separate contexts represent two users or requests.
    using var user1Context = new AppDbContext();
    using var user2Context = new AppDbContext();

    // 1. Both users read the record.
    var productUser1 = user1Context.Products.First(p => p.Id == productId);
    var productUser2 = user2Context.Products.First(p => p.Id == productId);

    // 2. User 1 changes the price and saves successfully.
    productUser1.Price = 999.00m;
    user1Context.SaveChanges();
    Console.WriteLine("[User 1] Updated price to 999.00 and saved successfully.");

    // 3. User 2 tries to save a change based on stale data.
    productUser2.Price = 50.00m;

    try
    {
        Console.WriteLine("[User 2] Attempting to save outdated changes...");
        user2Context.SaveChanges(); // This should trigger a concurrency exception.
        Console.WriteLine("[User 2] Saved successfully (Unexpected!)");
    }
    catch (DbUpdateConcurrencyException ex)
    {
        Console.WriteLine("[CONFLICT DETECTED!] Concurrency violation handled gracefully.");
        Console.WriteLine($"Error Details: {ex.Message}");

        // Conflict resolution strategy:
        // Load the latest database values.
        var entry = ex.Entries.Single();
        var databaseValues = entry.GetDatabaseValues();

        if (databaseValues != null)
        {
            var dbPrice = (decimal)databaseValues["Price"]!;
            Console.WriteLine($"[Resolution] Current price in PostgreSQL database is: {dbPrice:C}");
        }
    }
}

void Add_Product_With_Jsonb(string name, decimal price, string jsonMetadata)
{
    using var ctx = new AppDbContext();

    var product = new Product
    {
        Name = name,
        Price = price,
        Tags = new List<string> { "apple", "smartphone", "5g", "flagship" },
        JsonMetadata = jsonMetadata
    };

    ctx.Products.Add(product);
    ctx.SaveChanges();
    Console.WriteLine($"Product with JSONB metadata added. ID: {product.Id}");
}

void Search_Products_By_Jsonb(string jsonFilter)
{
    using var ctx = new AppDbContext();

    var products = ctx.Products
        .AsNoTracking()
        .Where(p => EF.Functions.JsonContains(p.JsonMetadata, jsonFilter))
        .ToList();

    Console.WriteLine($"Matching products: {products.Count}");
    foreach (var product in products)
    {
        Console.WriteLine($"ID: {product.Id} | Name: {product.Name} | Metadata: {product.JsonMetadata}");
    }
}

