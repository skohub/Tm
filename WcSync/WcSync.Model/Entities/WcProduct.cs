namespace WcSync.Model.Entities
{
    public class WcProduct 
    {
        required public ulong Id { get; init; }

        public string? Sku { get; init; }

        public string? Name { get; init; }

        public string? Availability { get; init; }

        public decimal? RegularPrice { get; init; }

        public decimal? SalePrice { get; init; }

        public string? StockStatus { get; init; }

        public bool FixedPrice { get; init; }
    }
}