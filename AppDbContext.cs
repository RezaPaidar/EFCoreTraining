using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCoreTraining
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false)
                .Build();

            string? connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString) || connectionString.Contains("YOUR_PASSWORD"))
            {
                throw new InvalidOperationException(
                    "Set ConnectionStrings:DefaultConnection in appsettings.json or appsettings.Local.json.");
            }

            optionsBuilder.UseNpgsql(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global Query Filter: به ازای هر کوئری روی Products این شرط خودکار اضافه می شود
            /*
             توضیح فنی: از این لحظه به بعد، هر بار که بنویسی ctx.Products.ToList()، EF Core پشت صحنه در SQL این شرط رو تزریق می کنه:
            WHERE "p"."IsDeleted" = FALSE
             */
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => !p.IsDeleted);

            // فیلتر Soft Delete که مرحله قبل زدیم
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => !p.IsDeleted);
            /*-------------------------------------------------------------------------*/
            // ۱. ایجاد Index روی فیلد Name
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .HasDatabaseName("IX_Products_Name");

            // ۲. ایجاد Composite Index (ایندکس ترکیبی) روی CategoryId و Price
            // برای کوئری‌  که مثلاً فیلتر می کنند: WHERE CategoryId = 1 AND Price > 100
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.categoryId, p.Price })
                .HasDatabaseName("IX_Products_CategoryId_Price");

            // پیکربندی Concurrency Token با استفاده از xmin در PostgreSQL:
            modelBuilder.Entity<Product>()
                .Property<uint>("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
        }
    }
}

