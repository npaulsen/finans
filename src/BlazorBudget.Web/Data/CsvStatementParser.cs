using System.Globalization;
using BlazorBudget.Core;

namespace BlazorBudget.Web.Data;

record ColumnOffsets(int Date, int Amount, int Type, int Recipient, int Reference);

// TODO replace by smarter import
class CsvStatementParser : IStatementParser
{
    private readonly int _numHeaderRows;
    private readonly ColumnOffsets _columnOffsets;
    private readonly CultureInfo _culture;

    public CsvStatementParser(int numHeaderRows, ColumnOffsets columnOffsets, CultureInfo culture)
    {
        _numHeaderRows = numHeaderRows;
        _columnOffsets = columnOffsets;
        _culture = culture;
    }

    protected Transaction ParseStatementLine(string line)
    {
        var vals = line.Split(';');
        var date = DateOnly.Parse(vals[_columnOffsets.Date], _culture);
        var amount = decimal.Parse(vals[_columnOffsets.Amount], _culture);
        var type = vals[_columnOffsets.Type];
        var recipient = vals[_columnOffsets.Recipient];
        var reference = vals[_columnOffsets.Reference];
        return new Transaction(date, amount, type, reference, recipient);
    }

    public List<Transaction> ParseStatementFile(IEnumerable<string> lines)
    {
        if (lines.Count() <= _numHeaderRows)
        {
            // _logger.LogWarning("No transactions found in file.");
            return new();
        }
        return lines
            .Skip(_numHeaderRows)
            .Select(ParseStatementLine)
            .ToList();
    }
}