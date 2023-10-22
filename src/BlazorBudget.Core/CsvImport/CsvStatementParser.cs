using System.Globalization;

namespace BlazorBudget.Core.CsvImport;

public record ColumnOffsets(int Date, int Amount, int Type, int Recipient, int Reference);

public class ColumnMappings {
    public int? Date { get; set; }
    public int? Amount { get; set; }
    public int? Type { get; set; }
    public int? Recipient { get; set; }
    public int? Reference { get; set; }

    public CultureInfo Culture { get; set; }
    
    // TODO maybe simpler to assume it is always fitting with the culture of the number format...
    public string DateFormat { get; set; }

    
    public static IEnumerable<string> PossibleDateStringFormats => new[] {"dd.MM.yyyy","dd.MM.yy"};
    public static IEnumerable<string> PossibleCultureFormats => new[] {"de-DE", "en-US", "en-UK", ""};

    public bool IsMissingAnyData =>
        Date is null || Amount is null || Type is null || Recipient is null ||
        Reference is null || Culture is null || DateFormat is null;

    public ColumnOffsets GetColumnOffsets() => new(Date!.Value, Amount!.Value, Type!.Value, Recipient!.Value, Reference!.Value);
}

public class CsvStatementParser
{
    private readonly int _headerOffset;
    private readonly ColumnOffsets _columnOffsets;
    private readonly CultureInfo _culture;
    private readonly CsvLineSplitter _splitter;
    private readonly string _dateFormat;

    public CsvStatementParser(CsvLineSplitter splitter, int headerRowOffset, ColumnOffsets columnOffsets, CultureInfo culture, string dateFormat)
    {
        _headerOffset = headerRowOffset;
        _columnOffsets = columnOffsets;
        _culture = culture;
        _splitter = splitter;
        _dateFormat = dateFormat;
    }

    public CsvStatementParser(CsvLineSplitter splitter, int headerRowOffset, ColumnMappings mappings)
        : this(splitter, headerRowOffset, mappings.GetColumnOffsets(), mappings.Culture!, mappings.DateFormat!)
    {

    }

    protected Transaction ParseStatementLine(string line)
    {
        var vals = _splitter.Split(line);
        var date = DateOnly.ParseExact(vals[_columnOffsets.Date], _dateFormat);
        var amount = decimal.Parse(vals[_columnOffsets.Amount], NumberStyles.Currency ,_culture);
        var type = vals[_columnOffsets.Type];
        var recipient = vals[_columnOffsets.Recipient];
        var reference = vals[_columnOffsets.Reference];
        return new(date, amount, type, reference, recipient);
    }

    public async Task<CsvParsingResult> ParseCsvFromStreamAsync(StreamReader reader)
    {
        var parsed = new List<Transaction>();
        var failsDuringParsing = 0;
        string? firstLineFailingParsing = null;

        for (var lineIndex = 0; ; lineIndex++)
        {
            var line = await reader.ReadLineAsync();
            if (line is null) break;
            if (lineIndex <= _headerOffset) continue;

            try
            {
                parsed.Add(ParseStatementLine(line));
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error parsing a line: {e.Message}");
                failsDuringParsing++;
                firstLineFailingParsing ??= line;
            }
        }
        return new CsvParsingResult(parsed, failsDuringParsing, firstLineFailingParsing);
    }

    // TODO remove and use the async stream-based one always.
    public CsvParsingResult ParseCsv(string[] lines)
    {
        var parsed = new List<Transaction>();
        var failsDuringParsing = 0;
        string? firstLineFailingParsing = null;

        foreach (var line in lines.Skip(_headerOffset + 1))
        {
            try
            {
                parsed.Add(ParseStatementLine(line));
            }
            catch
            {
                failsDuringParsing++;
                firstLineFailingParsing ??= line;
            }
        }
        return new CsvParsingResult(parsed, failsDuringParsing, firstLineFailingParsing);
    }
}
