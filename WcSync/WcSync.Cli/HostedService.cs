using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Tm.WcSync.Model;

namespace Tm.WcSync.Cli;
public class HostedService : BackgroundService
{
    private readonly ISyncService _syncService;
    private readonly string _command;

    public HostedService(ISyncService syncService, string command) =>
        (_syncService, _command) = (syncService, command);

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _syncService.RunAsync(_command, stoppingToken);    
        }
        catch (OperationCanceledException)
        {
        }
    }

    public async override Task StopAsync(CancellationToken cancellationToken) =>
        await base.StopAsync(cancellationToken);
}