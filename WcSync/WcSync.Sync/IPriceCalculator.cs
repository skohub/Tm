using Tm.WcSync.Model.Entities;

namespace Tm.WcSync.Sync
{
    public interface IPriceCalculator
    {
        (decimal? price, decimal? salePrice) GetPrice(DbProduct product);
    }
}