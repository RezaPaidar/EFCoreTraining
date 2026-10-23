namespace EFCoreTraining
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int? categoryId { get; set; }
        public Category? Category { get; set; }
    }
}
