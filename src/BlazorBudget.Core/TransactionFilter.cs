namespace BlazorBudget.Core;

public class TransactionFilter
{
    public string TextFilter { get; set; } = string.Empty;

    public bool MatchRecipient { get; set; } = true;
    public bool MatchReference { get; set; } = true;

    public bool Matches(Transaction transaction)
    {
        if (MatchRecipient && transaction.Recipient.Contains(TextFilter, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        
        if (MatchReference && transaction.Reference.Contains(TextFilter, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
        
        return false;
    }

    public override string ToString()
    {
        return $"[{nameof(TransactionFilter)}:'{TextFilter}']";
    }
}
