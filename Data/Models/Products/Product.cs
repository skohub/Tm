namespace Data.Models.Products
{
    public class Product
    {
        required public int ProductId { get; init; }
        required public string ProductName { get; init; }
        required public string StoreName { get; init; }
        required public StoreType StoreType { get; init; }
        required public int Quantity { get; init; }    
        public decimal Price {get;  init; }
    }
}