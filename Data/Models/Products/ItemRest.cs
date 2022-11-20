namespace Data.Models.Products
{
    public class ItemRest
    {
        public int ItemID { get; set; }

        public string i_n { get; set; }

        public string name { get; set; }

        public StoreType StoreType { get; set; }

        public int summ { get; set; }    

        public decimal price { get; set; }
    }
}