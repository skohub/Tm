using System.Collections.Generic;
using Tm.WcSync.Model.Entities;

namespace Tm.WcSync.Db
{
    public interface IDbProductRepository
    {
        List<DbProduct> GetRecentlyUpdatedProducts();

        List<DbProduct> GetProducts();
    }
}