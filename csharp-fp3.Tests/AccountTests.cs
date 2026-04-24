using CsharpFp3.Domain;

namespace CsharpFp3.Tests;

/// Tests covering domain-layer invariants on the Account aggregate directly,
/// independent of the application layer.
public class AccountTests
{
    [Fact]
    public void Opening_an_account_with_a_negative_balance_returns_OpeningBalanceMustBeNonNegative()
    {
        var result = AccountDomain.Open(Guid.NewGuid(), new Money(-1m));

        Assert.IsType<AccountError.OpeningBalanceMustBeNonNegative>(result.Error);
    }

    [Fact]
    public void Withdrawing_a_zero_amount_returns_AmountMustBePositive()
    {
        var account = AccountDomain.Open(Guid.NewGuid(), new Money(100m)).Value;

        var result = AccountDomain.Withdraw(account, new Money(0m));

        Assert.IsType<AccountError.AmountMustBePositive>(result.Error);
    }

    [Fact]
    public void Withdrawing_a_negative_amount_returns_AmountMustBePositive()
    {
        var account = AccountDomain.Open(Guid.NewGuid(), new Money(100m)).Value;

        var result = AccountDomain.Withdraw(account, new Money(-10m));

        Assert.IsType<AccountError.AmountMustBePositive>(result.Error);
    }

    [Fact]
    public void Withdrawing_more_than_the_balance_returns_InsufficientBalance()
    {
        var account = AccountDomain.Open(Guid.NewGuid(), new Money(100m)).Value;

        var result = AccountDomain.Withdraw(account, new Money(101m));

        Assert.IsType<AccountError.InsufficientBalance>(result.Error);
    }

    [Fact]
    public void InsufficientBalance_error_reports_the_current_balance_and_attempted_amount()
    {
        var account = AccountDomain.Open(Guid.NewGuid(), new Money(100m)).Value;

        var error = Assert.IsType<AccountError.InsufficientBalance>(
            AccountDomain.Withdraw(account, new Money(150m)).Error
        );

        Assert.Equal(100m, error.Balance.Amount);
        Assert.Equal(150m, error.Amount.Amount);
    }

    [Fact]
    public void Successive_withdrawals_are_each_applied_to_the_running_balance()
    {
        var account = AccountDomain.Open(Guid.NewGuid(), new Money(300m)).Value;

        var result1 = AccountDomain.Withdraw(account, new Money(100m));
        var updatedAccount1 = result1.Value;

        var result2 = AccountDomain.Withdraw(updatedAccount1, new Money(100m));
        var updatedAccount2 = result2.Value;

        Assert.Equal(100m, updatedAccount2.Balance.Amount);
    }
}
