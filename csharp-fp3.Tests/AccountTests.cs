using CsharpFp3.Domain;
using CsharpFp3.Library;

namespace CsharpFp3.Tests;

/// Tests covering domain-layer invariants on the Account aggregate directly,
/// independent of the application layer.
public class AccountTests
{
    [Fact]
    public void Opening_an_account_with_a_negative_balance_returns_OpeningBalanceMustBeNonNegative()
    {
        var result = AccountDomain.Open(Guid.NewGuid(), new Money(-1m));

        Assert.True(result.Match(_ => false, e => e is AccountError.OpeningBalanceMustBeNonNegative));
    }

    [Fact]
    public void Withdrawing_a_zero_amount_returns_AmountMustBePositive()
    {
        var accountResult = AccountDomain.Open(Guid.NewGuid(), new Money(100m));
        var account = accountResult.Match(v => v, _ => throw new InvalidOperationException("Expected success"));

        var result = AccountDomain.Withdraw(account, new Money(0m));

        Assert.True(result.Match(_ => false, e => e is AccountError.AmountMustBePositive));
    }

    [Fact]
    public void Withdrawing_a_negative_amount_returns_AmountMustBePositive()
    {
        var accountResult = AccountDomain.Open(Guid.NewGuid(), new Money(100m));
        var account = accountResult.Match(v => v, _ => throw new InvalidOperationException("Expected success"));

        var result = AccountDomain.Withdraw(account, new Money(-10m));

        Assert.True(result.Match(_ => false, e => e is AccountError.AmountMustBePositive));
    }

    [Fact]
    public void Withdrawing_more_than_the_balance_returns_InsufficientBalance()
    {
        var accountResult = AccountDomain.Open(Guid.NewGuid(), new Money(100m));
        var account = accountResult.Match(v => v, _ => throw new InvalidOperationException("Expected success"));

        var result = AccountDomain.Withdraw(account, new Money(101m));

        Assert.True(result.Match(_ => false, e => e is AccountError.InsufficientBalance));
    }

    [Fact]
    public void InsufficientBalance_error_reports_the_current_balance_and_attempted_amount()
    {
        var accountResult = AccountDomain.Open(Guid.NewGuid(), new Money(100m));
        var account = accountResult.Match(v => v, _ => throw new InvalidOperationException("Expected success"));

        var result = AccountDomain.Withdraw(account, new Money(150m));
        var error = result.Match(
            _ => throw new InvalidOperationException("Expected failure"),
            e => e is AccountError.InsufficientBalance ins ? ins : throw new InvalidOperationException("Expected InsufficientBalance")
        );

        Assert.Equal(100m, error.Balance.Amount);
        Assert.Equal(150m, error.Amount.Amount);
    }

    [Fact]
    public void Successive_withdrawals_are_each_applied_to_the_running_balance()
    {
        var accountResult = AccountDomain.Open(Guid.NewGuid(), new Money(300m));
        var account = accountResult.Match(v => v, _ => throw new InvalidOperationException("Expected success"));

        var result1 = AccountDomain.Withdraw(account, new Money(100m));
        var updatedAccount1 = result1.Match(v => v, _ => throw new InvalidOperationException("Expected success"));

        var result2 = AccountDomain.Withdraw(updatedAccount1, new Money(100m));
        var updatedAccount2 = result2.Match(v => v, _ => throw new InvalidOperationException("Expected success"));

        Assert.Equal(100m, updatedAccount2.Balance.Amount);
    }
}
