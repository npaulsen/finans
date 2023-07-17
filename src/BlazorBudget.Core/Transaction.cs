namespace BlazorBudget.Core;

public record Transaction(DateOnly Date, decimal Amount, string Type, string Reference, string Recipient);

