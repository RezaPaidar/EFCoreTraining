using Microsoft.EntityFrameworkCore;

namespace EFCoreTraining
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = Environment.GetEnvironmentVariable("EFCORETRAINING_CONNECTION_STRING")
                ?? throw new InvalidOperationException(
                    "Set the EFCORETRAINING_CONNECTION_STRING environment variable before connecting to the database.");

            optionsBuilder.UseNpgsql(connectionString);
        }

    }
}
