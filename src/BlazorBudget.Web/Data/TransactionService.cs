using System.Globalization;
using BlazorBudget.Core;

namespace BlazorBudget.Web.Data;

public partial class TransactionService
{
    /// <summary>
    /// Pure in memory state right now.
    /// </summary>
    private List<Transaction> _allTransactions;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ILogger<TransactionService> logger)
    {
        _logger = logger;
        _allTransactions = new();
    }

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
        _allTransactions = new();
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
        
        // TODO concept for importing and internal storage.
        // Hardcoded defaults
        var defaultStatements = new StatementInfo[] {
            // new ("Example_ING_no_balance", IngStatementParser.For(withBalance: false, withNotes: true)),
            // new ("Example_ING_balance", IngStatementParser.For(withBalance: true, withNotes: true)),
            new ("230715", IngStatementParser.For(withBalance: true, withNotes: false)),
            new ("230414", IngStatementParser.For(withBalance: true, withNotes: true)),
        };
        const string directory = "/data/input";
        var files = Directory.EnumerateFiles(directory, "*.csv");

        foreach (var statement in defaultStatements)
        {
            try
            {
                var theFile = files.Single(file => file.Contains(statement.FileMatch));
                await LoadFromFileAsync(theFile, statement.Parser);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to load statement {statement}", statement);
            }
        }
    }

    private async Task LoadFromFileAsync(string fileName, IStatementParser parser)
    {
        try
        {
            var lines = await File.ReadAllLinesAsync(fileName);
            _logger.LogInformation("Loading csv file {file} containing {numLines} lines", fileName, lines.Count());
            var parsedStatements = parser.ParseStatementFile(lines);
            await ImportAsync(parsedStatements);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "Failed to load from file {file}", fileName);
        }
    }
}
