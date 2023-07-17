using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WcSync.Model.Entities;

namespace WcSync.Wc 
{
    public interface IWcProductService
    {
        Task UpdateProductsAsync(List<WcProduct> products, CancellationToken cancellationToken);
        Task<List<WcProduct>> GetProductsAsync(CancellationToken cancellationToken);
    }
}