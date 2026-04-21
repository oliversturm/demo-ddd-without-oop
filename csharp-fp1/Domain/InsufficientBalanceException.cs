namespace CsharpFp1.Domain;

/// Custom domain exception thrown when a withdrawal would cause the balance to go below zero
public sealed class InsufficientBalanceException : InvalidOperationException
{
    public Money CurrentBalance { get; }
    public Money RequestedAmount { get; }

    public InsufficientBalanceException(Money currentBalance, Money requestedAmount)
        : base(
            $"Insufficient balance. Current: {currentBalance.Amount:0.00}, Requested: {requestedAmount.Amount:0.00}"
        )
    {
        CurrentBalance = currentBalance;
        RequestedAmount = requestedAmount;
    }
}
