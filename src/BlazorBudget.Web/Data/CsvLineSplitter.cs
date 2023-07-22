using System.Text.RegularExpressions;

namespace BlazorBudget.Web.Data;


class CsvLineSplitter
{
    private readonly string _separator;
    private readonly Regex _regex; 

    public string Separator => _separator;

    public CsvLineSplitter(string separator)
    {
        _separator = separator;
        _regex = new ($"{separator}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
    }

    public string[] Split(string line)
        => _regex.Split(line).Select(UnwrapQuotations).ToArray();

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
}