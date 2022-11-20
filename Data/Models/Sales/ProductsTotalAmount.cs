namespace Data.Models.Sales
{
    public class ProductsTotalAmount
    {
        public string Store { get; set; } = string.Empty;
        public decimal SellingPriceTotal { get; set; }
        public decimal PurchasingPriceTotal { get; set; }
    }
}
