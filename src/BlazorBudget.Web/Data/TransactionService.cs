using BlazorBudget.Core;
using BlazorBudget.Core.CsvImport;

namespace BlazorBudget.Web.Data;

public partial class TransactionService(ILogger<TransactionService> logger)
{
    /// <summary>
    /// Pure in memory state right now.
    /// </summary>
    private List<Transaction> _allTransactions = [];
    private readonly ILogger<TransactionService> _logger = logger;

    private List<Transaction> Dedup(IEnumerable<Transaction> allTransactions)
    {
        var previousCount = allTransactions.Count();
        var deduped = allTransactions.Distinct().ToList();
        if (deduped.Count < previousCount)
        {
            _logger.LogInformation("Filtered out {removedCount} duplicate transactions.", previousCount - deduped.Count);
        }
        return deduped;
    }

    public Task<Transaction[]> GetTransactionsAsync()
    {
        return Task.FromResult(_allTransactions.ToArray());
    }

    public Task ClearAllTransactionsAsync()
    {
        _logger.LogInformation("Clearing all transactions.");
        _allTransactions = [];
        return Task.CompletedTask;
    }

    public Task ImportAsync(List<Transaction> newTransactions)
    {
        _logger.LogInformation("Importing {newTransactionCount} transactions.", newTransactions.Count);
        var deduped = Dedup(_allTransactions.Concat(newTransactions));
        _allTransactions = deduped.OrderBy(t => t.Date).ToList();
        return Task.CompletedTask;
    }

    public async Task LoadPresetsAsync()
    {
        _logger.LogInformation("Loading presets.");
        
        const string directory = "autoimport";
        var files = Directory.EnumerateFiles(directory, "*.csv");

        foreach (var file in files)
        {
            try
            {
                await LoadFromFileAsync(file);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to load statement file {file}", file);
            }
        }
    }

    private async Task LoadFromFileAsync(string fileName)
    {
        try
        {
            var lines = await File.ReadAllLinesAsync(fileName);
            _logger.LogInformation("Loading csv file {file} containing {numLines} lines", fileName, lines.Count());

            // TODO
            var (splitter, headerOffset) = CsvFormatGuesser.Guess(lines.Take(30).ToList());

            var headers = splitter.Split(lines.Skip(headerOffset).First());
            var exampleRows = lines.Skip(headerOffset + 1).Take(3).Select(splitter.Split).ToArray();

            var mappings = CsvFormatGuesser.GuessColumns(headers, exampleRows);
            if (mappings.IsMissingAnyData)
            {
                _logger.LogWarning("Could not guess all column mappings for {file}. Skipping it.", fileName);
                return;
            }

            var parser = new CsvStatementParser(splitter, headerOffset, mappings);
            var parsingResult = parser.ParseCsv(lines);
            _logger.LogInformation("Parsing skipped {fails} lines and read {read} transactions",
            parsingResult.NumFailures, parsingResult.Transactions.Count);
            // var parsedStatements = parser.ParseStatementFile(lines);
            await ImportAsync(parsingResult.Transactions);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to load from file {file}", fileName);
        }
    }
}
