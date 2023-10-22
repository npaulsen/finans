using System.Text.RegularExpressions;

namespace BlazorBudget.Core.CsvImport;


public class CsvLineSplitter
{
    private readonly string _separator;
    private readonly Regex _regex; 

    public string Separator => _separator;

    public static List<(string Name, string Separator)> PossibleSeparators = new () {
        ("Comma", ","),
        ("Semicolon", ";"),
        ("Tab", "\t"),
        ("Space", " "),
    };

    public CsvLineSplitter(string separator)
    {
        _separator = separator;
        _regex = new ($"{separator}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
    }

    public string[] Split(string line)
        => _regex.Split(line)
            .Select(UnwrapQuotations)
            .Select(NormalizeSpaces)
            .ToArray();

    private string UnwrapQuotations(string cellContent)
    {
        if (cellContent.Length > 1 && cellContent[0] == '"' && cellContent[^1] == '"')
        {
            return cellContent
                // Remove leading & trailing quotations.
                .Substring(1, cellContent.Length - 2)
                // In boxed cells, all quotations are replaced by double quotations.
                .Replace("\"\"", "\"");
        }
        return cellContent;
    }

    /// <summary>
    /// Replaces "non-breaking space" by "normal" one.
    /// </summary>
    private string NormalizeSpaces(string original) => original.Replace('\u00A0', ' ');
}