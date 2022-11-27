using System.Collections.Generic;
using System.Linq;

namespace WcSync.Model.Entities
{
    public class DbProduct 
    {
        required public int Id { get; init; }

        required public string Name { get; init; }

        required public IList<Store> Availability { get; init; }

        public string GetStockStatus()
        {
            var available = Availability
                .Where(a => a.Type == StoreType.Shop || a.Type == StoreType.Warehouse)
                .Any(a => a.Quantity > 0);

            return available ? Consts.AvailableStatus : Consts.UnavailableStatus;
        }

        public string GetAvailability()
        {
            return string.Join(",", Availability
                .Where(a => a.Type == StoreType.Shop)
                .Where(a => a.Quantity > 0)
                .Select(a => a.Name));
        }
    }
}