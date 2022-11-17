using System.Threading;
using System.Threading.Tasks;
using WcSync.Model;

namespace WcSync.Sync;
public class SyncService : ISyncService
{
    private readonly IProductService _productService;

    public SyncService(IProductService productService)
    {
        _productService = productService;
    }

    public async Task RunAsync(string command, CancellationToken cancellationToken)
    {
        switch (command) 
        {
            case "list":
                await _productService.ListProductsDicrepanciesAsync(cancellationToken);
                break;
            case "update":
                await _productService.UpdateAllProductsAsync(cancellationToken);
                break;
            default:
                await _productService.UpdateAllProductsAsync(cancellationToken);
                break;
        }
    }
}