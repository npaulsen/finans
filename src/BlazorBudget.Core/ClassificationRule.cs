namespace BlazorBudget.Core;

public class ClassificationRule
{
    public TransactionFilter Filter { get; set; }
    public string TargetCategory { get; set; }

    public ClassificationRule(TransactionFilter filter, string targetCategory)
    {
        Filter = filter;
        TargetCategory = targetCategory;
    }
}
