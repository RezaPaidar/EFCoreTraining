namespace EFCoreTraining
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public int? categoryId { get; set; }
        public Category? Category { get; set; }
        public List<string> Tags { get; set; } = new();
        public string JsonMetadata { get; set; } = "{}";

    }
}

