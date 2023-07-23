namespace BlazorBudget.Core.CsvImport;

public class CsvParsingResult
{
    public CsvParsingResult(List<Transaction> transactions, int numFailures, string? firstFailingLine)
    {
        Transactions = transactions;
        NumFailures = numFailures;
        FirstFailingLine = firstFailingLine;
    }

    // TODO make immutable.
    public List<Transaction> Transactions { get; init; }
    public int NumFailures { get; init; }
    public string? FirstFailingLine { get; init; }


}