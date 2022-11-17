using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Serilog;
using WcSync.Model;

namespace WcSync.Cli;
public class HostedService : BackgroundService
{
    private readonly IHostApplicationLifetime _host;
    private readonly ISyncService _syncService;
    private readonly ILogger _logger;
    private readonly string _command;

    public HostedService(IHostApplicationLifetime host, ISyncService syncService, 
        ILogger logger, string command) =>
        (_host, _syncService, _logger, _command) = 
        (host, syncService, logger, command);

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _syncService.RunAsync(_command, stoppingToken);    
        }
        catch (OperationCanceledException e)
        {
            _logger.Information(e, "Stopping sync service");
        }
        catch (Exception e)
        {
            _logger.Error(e, "An error occured");
        }
        finally
        {
            Environment.ExitCode = 0;
            _host.StopApplication();
        }
    }

    public async override Task StopAsync(CancellationToken cancellationToken) =>
        await base.StopAsync(cancellationToken);
}