using System.ComponentModel.DataAnnotations;

namespace ABC_Retail.Models
{
    public class Order
    {
       // [BindNever] // This attribute is defined in Microsoft.AspNetCore.Mvc.ModelBinding

        public string Id { get; set; }   // auto-generated

        [Required]
        public string CustomerId { get; set; }

        [Required]
        public string ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public double TotalPrice { get; set; }
    }
}
