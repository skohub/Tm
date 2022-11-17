using System.Threading;
using System.Threading.Tasks;

namespace WcSync.Model
{
    public interface IProductService
    {
        Task UpdateAllProductsAsync(CancellationToken cancellationToken);

        Task ListProductsDicrepanciesAsync(CancellationToken cancellationToken);
    }
}