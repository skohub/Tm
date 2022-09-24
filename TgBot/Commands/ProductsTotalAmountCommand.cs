using Telegram.Bot;
using Tm.TgBot.Commands.Common;
using Tm.Data.Interfaces;

namespace Tm.TgBot.Commands;

public class ProductsTotalAmountCommand : ICommand
{
    private const string _lineBreak = "\r\n";

    private readonly ISaleRepository _saleRepository;

    public ProductsTotalAmountCommand(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<CommandResult> ExecuteAsync(string[] arguments, ITelegramBotClient client)
    {
        var dateArgument = arguments.Skip(1).FirstOrDefault();
        
        if (!DateTime.TryParse(dateArgument, out var date))
        {
            date = DateTime.Today;
        }

        var header = $"Остатки товара на {date}";
        var amounts = await _saleRepository.GetProductsTotalAmount(date);
        var lines = amounts.Select(x => $"*{x.Store}:* {x.PurchasingPriceTotal:N0}р.");

        return new CommandResult
        {
            Text = $"{header}{_lineBreak}{string.Join(_lineBreak, lines)}",
        };
    }
}
