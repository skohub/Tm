using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tm.WcSync.Model;

namespace Tm.WcSync.Cli;
public class HostedService : IHostedService
{
    private readonly ISyncService _syncService;
    private readonly string _command;

    public HostedService(ISyncService syncService, string command)
    {
        _syncService = syncService;
        _command = command;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _syncService.RunAsync(_command);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}