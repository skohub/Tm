using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models.Products;

namespace Data.Interfaces
{
    public interface IProductsRepository
    {
        Task<IList<Product>> GetProductsAsync(int organizationId);
    }
}