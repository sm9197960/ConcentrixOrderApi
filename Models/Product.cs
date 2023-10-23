using Microsoft.EntityFrameworkCore;

namespace MyStoreApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        [Precision(16,2)]
        public decimal Price {  get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageFileName {  get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
