namespace BlazorBudget.Core;

public class ClassificationRule(TransactionFilter filter, string targetCategory)
{
    public TransactionFilter Filter { get; set; } = filter;
    public string TargetCategory { get; set; } = targetCategory;
}
