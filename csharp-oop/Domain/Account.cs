namespace CsharpOop.Domain;

/// Domain entity / aggregate root representing a bank account
public sealed class Account : AggregateRoot<AccountId>
{
    public Money Balance { get; private set; }

    // Often present to satisfy serializers / ORMs.
    private Account()
    {
        Balance = new Money(0);
    }

    public Account(AccountId id, Money openingBalance)
    {
        if (openingBalance.Amount < 0)
            throw new ArgumentOutOfRangeException(nameof(openingBalance));

        Id = id;

        Balance = openingBalance;
    }

    // Domain behaviour attached to the entity.
    public void Withdraw(Money amount)
    {
        if (amount.Amount <= 0)
            throw new InvalidOperationException("Withdrawal amount must be positive.");

        if (Balance.Amount - amount.Amount < 0)
            throw new InvalidOperationException("Balance cannot go below zero.");

        Balance = Balance.Subtract(amount);
    }
}
