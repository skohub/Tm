using System.Threading.Tasks;

namespace Tm.WcSync.Model
{
    public interface IProductService
    {
        Task UpdateAllProductsAsync();

        Task ListProductsDicrepancies();
    }
}