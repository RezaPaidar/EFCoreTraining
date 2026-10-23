using EFCoreTraining;
using System.Xml.Schema;

Console.WriteLine("Hello, World!");

using (var ctx = new AppDbContext())
{
    bool created = ctx.Database.EnsureCreated();
    if (created)
    {
        Console.WriteLine("DB successfully created!");
    }
    else
    {
        Console.WriteLine("DB is already exists.");
    }
}

//Create_Product();
Read_All();
Category_Add("Electronics");
//Read_Product(1);
//Update_Product(1, "TV", 2500.99m);
//Delete_Product(1);

void Create_Product()
{
    using (var ctx = new  AppDbContext())
    {
        var product = new Product {
            Name = "Mobile",
            Price = 1500
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
        var products = ctx.Products.ToList();
        foreach(var p in products)
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
        var product = ctx.Products.FirstOrDefault(x => x.Id == id);
        Console.WriteLine($"Product Name: {product?.Name}, Product Price: {product?.Price}");
        return product!;
    }
}

void Update_Product(int id, string name, decimal price)
{
    using (var ctx = new AppDbContext())
    {
        var product = ctx.Products.FirstOrDefault(x => x.Id == id);
        if(product is not null)
        {
            product.Name = name;
            product.Price = price;
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