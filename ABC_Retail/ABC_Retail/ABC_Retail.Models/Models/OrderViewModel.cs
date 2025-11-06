namespace ABC_Retail.Models
{
    public class OrderViewModel
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
    }
}
