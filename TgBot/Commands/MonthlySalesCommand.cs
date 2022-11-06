using Telegram.Bot;
using Tm.TgBot.Commands.Common;
using Tm.Data.Interfaces;

namespace Tm.TgBot.Commands;

public class MonthlySalesCommand : ICommand
{
    private const string _lineBreak = "\r\n";
    public string Name => "/monthlysales";

    private readonly ISalesReportsRepository _salesReportsRepository;

    public MonthlySalesCommand(ISalesReportsRepository salesReportsRepository)
    {
        _salesReportsRepository = salesReportsRepository;
    }

    public async Task<CommandResult> ExecuteAsync(string[] arguments, ITelegramBotClient client)
    {
        var dateArgument = arguments.Skip(1).FirstOrDefault();
        
        if (!DateTime.TryParse(dateArgument, out var date))
        {
            date = DateTime.Today;
        }

        var header = $"Продажи за месяц на {date}";
        var sales = await _salesReportsRepository.GetMonthlySales(date.Year, date.Month);
        var lines = sales.Select(x => $"*{x.Key}:* {x.Value:N0}р.");

        return new CommandResult
        {
            Text = $"{header}{_lineBreak}{string.Join(_lineBreak, lines)}",
        };
    }
}
