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
            AccountDomain.Open(Guid.NewGuid(), new Money(-1m))
        );
    }

    [Fact]
    public void Withdrawing_a_zero_amount_returns_AmountMustBePositive()
    {
        var account = AccountDomain.Open(Guid.NewGuid(), new Money(100m));

        var result = AccountDomain.Withdraw(account, new Money(0m));

        Assert.IsType<WithdrawResult.AmountMustBePositive>(result);
    }

    [Fact]
    public void Withdrawing_a_negative_amount_returns_AmountMustBePositive()
    {
        var account = AccountDomain.Open(Guid.NewGuid(), new Money(100m));

        var result = AccountDomain.Withdraw(account, new Money(-10m));

        Assert.IsType<WithdrawResult.AmountMustBePositive>(result);
    }

    [Fact]
    public void Withdrawing_more_than_the_balance_returns_InsufficientBalance()
    {
        var account = AccountDomain.Open(Guid.NewGuid(), new Money(100m));

        var result = AccountDomain.Withdraw(account, new Money(101m));

        Assert.IsType<WithdrawResult.InsufficientBalance>(result);
    }

    [Fact]
    public void Successive_withdrawals_are_each_applied_to_the_running_balance()
    {
        var account = AccountDomain.Open(Guid.NewGuid(), new Money(300m));

        var result1 = AccountDomain.Withdraw(account, new Money(100m));
        var success1 = Assert.IsType<WithdrawResult.Success>(result1);

        var result2 = AccountDomain.Withdraw(success1.Account, new Money(100m));
        var success2 = Assert.IsType<WithdrawResult.Success>(result2);

        Assert.Equal(100m, success2.Account.Balance.Amount);
    }
}
