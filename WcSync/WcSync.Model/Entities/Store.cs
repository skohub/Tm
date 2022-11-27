namespace WcSync.Model.Entities
{
    public class Store 
    {
        required public string Name { get; init; }

        required public int Quantity { get; init; }

        required public decimal Price { get; init; }

        required public StoreType Type { get; init; }
    }
}