using System.Threading;
using System.Threading.Tasks;

namespace Tm.WcSync.Model
{
    public interface IProductService
    {
        Task UpdateAllProductsAsync(CancellationToken cancellationToken);

        Task ListProductsDicrepanciesAsync(CancellationToken cancellationToken);
    }
}