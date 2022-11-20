namespace Data.Models.Sales
{
    public class SalesSummary
    {
        public string Store { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
