using Telegram.Bot;
using TgBot.Commands.Common;
using Data.Interfaces;
using Telegram.Bot.Types.ReplyMarkups;
using System.Globalization;

namespace TgBot.Commands;

public class MonthlySalesCommand : ICommand
{
    private const int _responseButtonCount = 3;
    private const string _lineBreak = "\r\n";
    public string Name => "/monthlysales";

    private readonly ISalesReportsRepository _salesReportsRepository;

    public MonthlySalesCommand(ISalesReportsRepository salesReportsRepository)
    {
        _salesReportsRepository = salesReportsRepository;
    }

    public async Task<CommandResult> ExecuteAsync(string[] arguments, ITelegramBotClient client)
    {
        var monthName = arguments.Where(x => !x.StartsWith('/')).FirstOrDefault()?.ToLowerInvariant();
        if (monthName is null)
        {
            return RequestMonthName("Выбери месяц или впиши вручную");
        }
        if (!CommandConsts.Months.ContainsKey(monthName))
        {
            return RequestMonthName($"Не получилось распознать месяц \"{monthName}\", вот варианты");
        }

        (int year, int month) = GetReportPeriodByMonthName(monthName);
        var sales = await _salesReportsRepository.GetMonthlySales(year, month);
        var header = $"Продажи за {monthName}:";
        var lines = sales.Select(x => $"*{x.Key}:* {x.Value:N0}р.");

        return new CommandResult
        {
            Text = $"{header}{_lineBreak}{_lineBreak}{string.Join(_lineBreak, lines)}",
        };
    }

    private CommandResult RequestMonthName(string text) => new CommandResult
    {
        Text = text,
        ReplyMarkup = new ReplyKeyboardMarkup(GenerateButtons(_responseButtonCount))
        {
            ResizeKeyboard = true,
        }
    };

    private IEnumerable<KeyboardButton> GenerateButtons(int count) => Enumerable
        .Range(0, count)
        .Select(x => -x)
        .Select(DateTime.Now.AddMonths)
        .Reverse()
        .Select(GetMonthName)
        .Select(x => new KeyboardButton(x))
        .ToList();

    private string GetMonthName(DateTime date) =>
        CultureInfo.InvariantCulture.TextInfo.ToTitleCase(
            CommandConsts.Months.First(x => x.Value == date.Month).Key);

    private (int, int) GetReportPeriodByMonthName(string monthName)
    {
        var date = new DateTime(DateTime.Now.Year, CommandConsts.Months[monthName], 1);
        if (date > DateTime.Now)
        {
            date = date.AddYears(-1);
        }

        return (date.Year, date.Month);
    }
}
