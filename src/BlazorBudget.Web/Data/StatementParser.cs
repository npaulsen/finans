using BlazorBudget.Core;

namespace BlazorBudget.Web.Data;

public interface IStatementParser
{
    public List<Transaction> ParseStatementFile(IEnumerable<string> lines);
}