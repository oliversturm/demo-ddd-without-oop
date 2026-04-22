using CsharpFp2.Domain;

namespace CsharpFp2.Tests;

/// Tests covering domain-layer invariants on the Account aggregate directly,
/// independent of the application layer.
public class AccountTests
{
    [Fact]
    public void Opening_an_account_with_a_negative_balance_throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Account(new AccountId(Guid.NewGuid()), new Money(-1m))
        );
    }

    [Fact]
    public void Withdrawing_a_zero_amount_throws()
    {
        var account = new Account(new AccountId(Guid.NewGuid()), new Money(100m));

        Assert.Throws<InvalidOperationException>(() => account.Withdraw(new Money(0m)));
    }

    [Fact]
    public void Withdrawing_a_negative_amount_throws()
    {
        var account = new Account(new AccountId(Guid.NewGuid()), new Money(100m));

        Assert.Throws<InvalidOperationException>(() => account.Withdraw(new Money(-10m)));
    }

    [Fact]
    public void Withdrawing_more_than_the_balance_throws()
    {
        var account = new Account(new AccountId(Guid.NewGuid()), new Money(100m));

        Assert.Throws<InsufficientBalanceException>(() => account.Withdraw(new Money(101m)));
    }

    [Fact]
    public void Successive_withdrawals_are_each_applied_to_the_running_balance()
    {
        var account = new Account(new AccountId(Guid.NewGuid()), new Money(300m));

        account.Withdraw(new Money(100m));
        account.Withdraw(new Money(100m));

        Assert.Equal(100m, account.Balance.Amount);
    }
}
