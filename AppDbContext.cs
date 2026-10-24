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

            modelBuilder.Entity<Product>()
                .Property(p => p.JsonMetadata)
                .HasColumnType("jsonb")
                .HasDefaultValueSql("'{}'::jsonb");

            // Global Query Filter: ?? ???? ?? ????? ??? Products ??? ??? ?????? ????? ?? ???
            /*
             ????? ???: ?? ??? ???? ?? ???? ?? ??? ?? ?????? ctx.Products.ToList()? EF Core ??? ???? ?? SQL ??? ??? ?? ????? ?? ???:
            WHERE "p"."IsDeleted" = FALSE
             */
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => !p.IsDeleted);

            // ????? Soft Delete ?? ????? ??? ????
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => !p.IsDeleted);
            /*-------------------------------------------------------------------------*/
            // ?. ????? Index ??? ???? Name
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .HasDatabaseName("IX_Products_Name");

            // ?. ????? Composite Index (?????? ??????) ??? CategoryId ? Price
            // ???? ??????  ?? ????? ????? ?? ????: WHERE CategoryId = 1 AND Price > 100
            modelBuilder.Entity<Product>()
                .HasIndex(p => new { p.categoryId, p.Price })
                .HasDatabaseName("IX_Products_CategoryId_Price");

            // ???????? Concurrency Token ?? ??????? ?? xmin ?? PostgreSQL:
            modelBuilder.Entity<Product>()
                .Property<uint>("xmin")
                .HasColumnType("xid")
                .ValueGeneratedOnAddOrUpdate()
                .IsConcurrencyToken();
        }
    }
}




