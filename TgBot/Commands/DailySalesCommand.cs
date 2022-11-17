using Telegram.Bot;
using TgBot.Commands.Common;
using Data.Interfaces;

namespace TgBot.Commands;

public class DailySalesCommand : ICommand
{
    private const string _lineBreak = "\r\n";
    public string Name => "/dailysales";

    private readonly ISalesReportsRepository _salesReportsRepository;

    public DailySalesCommand(ISalesReportsRepository salesReportsRepository)
    {
        _salesReportsRepository = salesReportsRepository;
    }

    public async Task<CommandResult> ExecuteAsync(string[] arguments, ITelegramBotClient client)
    {
        var date = DateTime.Today;
        var sales = await _salesReportsRepository.GetSaleSummaries(date);
        if (!sales.Any())
        {
            return new CommandResult { Text = "Сегодня продаж еще не было." };
        }

        var salesText = sales
            .GroupBy(x => x.Store)
            .Select(x => $"*{x.Key}:*{_lineBreak}{string.Join(_lineBreak, x.Select(x => $"{x.Product}: {x.Price:N0}р."))}{_lineBreak}")
            .Aggregate((result, item) => string.Join(_lineBreak, new[] { result, item }));

        return new CommandResult
        {
            Text = salesText,
        };
    }
}
