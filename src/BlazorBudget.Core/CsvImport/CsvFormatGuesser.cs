using System.Globalization;

namespace BlazorBudget.Core.CsvImport;

public class CsvFormatGuesser
{

    public static (CsvLineSplitter Splitter, int HeaderOffsetLines) Guess(List<string> linesPreview)
    {
        var splitter = new CsvLineSplitter(";");
        int headerOffsetLines;

        var minimumMeaningfulLineLength = 30;
        var lastContentLine = linesPreview.Count - 1;
        while (lastContentLine > 0 && linesPreview[lastContentLine].Length < minimumMeaningfulLineLength)
        {
            lastContentLine--;
        }

        // Guess the separator
        var contentLine = linesPreview[lastContentLine];
        foreach (var sep in CsvLineSplitter.PossibleSeparators)
        {
            var testSplitter = new CsvLineSplitter(sep.Separator);
            var splitData = testSplitter
                .Split(contentLine)
                .Select(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            if (splitData.Count >= 5)
            {
                splitter = testSplitter;
                break;
            }
            // TODO could also check if the min number of non-empty cells in multiple lines is higher...
        }
        if (lastContentLine == 0)
        {
            headerOffsetLines = 0;
        }
        else
        {
            headerOffsetLines = lastContentLine - 1;
            while (headerOffsetLines > 0 &&
                linesPreview[headerOffsetLines - 1].Length > minimumMeaningfulLineLength &&
                splitter
                    .Split(linesPreview[headerOffsetLines - 1])
                    .Select(s => !string.IsNullOrWhiteSpace(s))
                    .Count() >= 5)
            {
                headerOffsetLines--;
            }
        }
        return (splitter, headerOffsetLines);
    }

    public static ColumnMappings GuessColumns(string[] headers, string[][] exampleRows)
    {
        var mappings = new ColumnMappings
        {
            Amount = FindBestHeaderMatching("Betrag"),
            Date = FindBestHeaderMatching("Valuta", "Wertstellung"),
            Recipient = FindBestHeaderMatching("Auftraggeber"),
            Reference = FindBestHeaderMatching("Verwendungszweck"),
            Type = FindBestHeaderMatching("Buchungstext")
        };
        if (exampleRows.Any())
        {
            if (mappings.Amount is not null)
            {
                var raw = exampleRows[0][mappings.Amount.Value]!;
                mappings.Culture = ColumnMappings.PossibleCultureFormats
                    .Select(CultureInfo.CreateSpecificCulture)
                    .FirstOrDefault(ci => decimal.TryParse(raw, ci, out _));
            }
            if (mappings.Date is not null)
            {
                var raw = exampleRows[0][mappings.Date.Value]!;
                mappings.DateFormat = ColumnMappings.PossibleDateStringFormats
                    .FirstOrDefault(format => DateOnly.TryParseExact(raw, format, out _));
            }
        }

        return mappings;

        int? FindBestHeaderMatching(params string[] matches)
        {
            var skipped = headers
                .TakeWhile(h => !matches.Any(match => h.Contains(match, StringComparison.OrdinalIgnoreCase)))
                .Count();
            return skipped == headers.Length? null : skipped;
        }
    }
}