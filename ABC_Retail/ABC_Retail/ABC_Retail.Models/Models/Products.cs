using System.ComponentModel.DataAnnotations;

namespace ABC_Retail.Models
{
    public class Products
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double Price { get; set; }

        public string Category { get; set; } 
        public int StockQuantity { get; set; } 
        public string ImageUrl { get; set; }
    }
}
