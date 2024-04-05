namespace BlazorBudget.Core.CsvImport;

public class CsvParsingResult(List<Transaction> transactions, int numFailures, string? firstFailingLine)
{

    // TODO make immutable.
    public List<Transaction> Transactions { get; init; } = transactions;
    public int NumFailures { get; init; } = numFailures;
    public string? FirstFailingLine { get; init; } = firstFailingLine;

}