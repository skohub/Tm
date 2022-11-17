using System.Collections.Generic;
using System.Threading.Tasks;
using WcSync.Model.Entities;

namespace WcSync.Db
{
    public interface IDbProductRepository
    {
        Task<List<DbProduct>> GetRecentlyUpdatedProductsAsync();

        Task<List<DbProduct>> GetProductsAsync();
    }
}