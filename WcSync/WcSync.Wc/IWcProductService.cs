using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tm.WcSync.Model.Entities;

namespace Tm.WcSync.Wc 
{
    public interface IWcProductService
    {
        Task UpdateProductAsync(int productId, string stockStatus, string availability, decimal? regularPrice, decimal? salePrice);
        Task UpdateProductsAsync(List<WcProduct> products);
        Task<List<WcProduct>> GetProductsAsync(CancellationToken cancellationToken);
    }
}