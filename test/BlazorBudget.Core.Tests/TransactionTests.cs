namespace BlazorBudget.Core.Tests;

public class TransactionTests
{
    [Fact]
    public void Transactions_are_equal_if_same_values()
    {
        var t1 = new Transaction(new(), 34.987m, "some type", "some ref", "some recipient");
        var t2 = new Transaction(new(), 34.987m, "some type", "some ref", "some recipient");

        Assert.Equal(t1, t2);
    }
}