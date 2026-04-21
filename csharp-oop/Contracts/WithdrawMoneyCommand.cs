namespace CsharpOop.Contracts;

/// Command DTO representing the requested use case
public sealed class WithdrawMoneyCommand
{
    public Guid AccountId { get; init; }
    public decimal Amount { get; init; }
}
