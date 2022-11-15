using System.Collections.Generic;
using System.Threading.Tasks;
using Tm.WcSync.Model.Entities;

namespace Tm.WcSync.Db
{
    public interface IDbProductRepository
    {
        Task<List<DbProduct>> GetRecentlyUpdatedProductsAsync();

        Task<List<DbProduct>> GetProductsAsync();
    }
}