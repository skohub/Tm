using Telegram.Bot;
using Tm.TgBot.Commands.Common;
using Tm.Data.Interfaces;

namespace Tm.TgBot.Commands;

public class SalesSummaryCommand : ICommand
{
    private const string _lineBreak = "\r\n";

    private readonly ISaleRepository _saleRepository;

    public SalesSummaryCommand(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<CommandResult> ExecuteAsync(string[] arguments, ITelegramBotClient client)
    {
        var date = DateTime.Today;
        var sales = await _saleRepository.GetSaleSummaries(date);
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
