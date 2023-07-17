using System.Globalization;
using BlazorBudget.Core;

namespace BlazorBudget.Web.Data;

public partial class TransactionService
{
    private Transaction[] _allTransactions;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(ILogger<TransactionService> logger)
    {
        _logger = logger;
        _allTransactions = LoadTransactions().ToArray();
    }

    private IEnumerable<Transaction> LoadTransactions()
    {
        const string directory = "/data/input";
        var cultureInfo = CultureInfo.CreateSpecificCulture("de-DE");
        var files = Directory.EnumerateFiles(directory, "*.csv");

        // TODO concept for importing and internal storage.
        var knownStatements = new StatementInfo[] {
            new ("Example_ING_no_balance", IngStatementParser.For(withBalance: false, withNotes: true)),
            new ("Example_ING_balance", IngStatementParser.For(withBalance: true, withNotes: true)),
            new ("230715", IngStatementParser.For(withBalance: true, withNotes: false)),
            new ("230414", IngStatementParser.For(withBalance: true, withNotes: true)),
        };

        var allTransactions = knownStatements
            .SelectMany(TryLoadTransactionsFromStatement);

        var dedupedTransactions = Dedup(allTransactions);

        return dedupedTransactions.OrderByDescending(t => t.Date);

        IEnumerable<Transaction> TryLoadTransactionsFromStatement(StatementInfo statement)
        {
            try
            {
                var theFile = files.Single(file => file.Contains(statement.FileMatch));
                var lines = File.ReadLines(theFile);
                _logger.LogInformation("Found csv file, {file} containing {numLines} lines", theFile, lines.Count());
                return statement.Parser.ParseStatementFile(lines);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Failed to load statement {statement}", statement);
                return Enumerable.Empty<Transaction>();
            }
        }
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
        _allTransactions = LoadTransactions().ToArray();
        return Task.FromResult(_allTransactions);
    }
}
