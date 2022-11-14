using System.Threading.Tasks;
using Tm.WcSync.Model;

namespace Tm.WcSync.Sync;
public class SyncService : ISyncService
{
    private readonly IProductService _productService;

    public SyncService(IProductService productService)
    {
        _productService = productService;
    }

    public async Task RunAsync(string command)
    {
        switch (command) 
        {
            case "list":
                await _productService.ListProductsDicrepancies();
                break;
            case "update":
                await _productService.UpdateAllProductsAsync();
                break;
            default:
                await _productService.UpdateAllProductsAsync();
                break;
        }
    }
}