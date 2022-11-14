using System.Threading.Tasks;

namespace Tm.WcSync.Model;
public interface ISyncService
{
    Task RunAsync(string command);
}