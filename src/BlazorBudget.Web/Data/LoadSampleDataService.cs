using BlazorBudget.Web.Data;

/// <summary>
/// Temporary thing to populate data with some automated imports.
/// Should be replaced by persisting transactions across restarts.
/// </summary>
public class LoadSampleDataService : BackgroundService
{
    private readonly ILogger<LoadSampleDataService> _logger;
    private readonly TransactionService _transactionService;

    public LoadSampleDataService(ILogger<LoadSampleDataService> logger, TransactionService transactionService)
    {
        _logger = logger;
        _transactionService = transactionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"LoadSampleDataService is starting.");

        await _transactionService.LoadPresetsAsync();

        _logger.LogInformation($"Done.");
    }

}