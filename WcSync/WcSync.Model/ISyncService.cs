using System.Threading;
using System.Threading.Tasks;

namespace WcSync.Model
{
    public interface ISyncService
    {
        Task UpdateAllProductsAsync(CancellationToken cancellationToken);
    }
}