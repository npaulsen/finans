namespace BlazorBudget.Core;

public record Category(string Name) {
    public static Category Uncategorized => new("[Uncategorized]");
}