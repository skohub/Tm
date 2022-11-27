namespace Data.Models.Products
{
    public class ItemRest
    {
        required public int ItemID { get; init; }
        required public string i_n { get; init; }
        required public string name { get; init; }
        required public StoreType StoreType { get; init; }
        required public int summ { get; init; }    
        required public decimal price { get; init; }
    }
}