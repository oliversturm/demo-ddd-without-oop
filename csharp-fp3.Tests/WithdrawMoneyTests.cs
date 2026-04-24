using CsharpFp3.Application;
using CsharpFp3.Domain;
using CsharpFp3.Infrastructure;
using CsharpFp3.Library;

namespace CsharpFp3.Tests;

/// Tests covering the application-layer use case surface:
/// what the WithdrawMoney feature does from the caller's perspective.
public class WithdrawMoneyTests
{
    private static (
        Func<Guid, decimal, Result<Account, AccountError>> withdraw,
        Repository repo
    ) BuildHandler()
    {
        var repo = InMemoryAccountRepository.Create();
        var withdraw = AccountApplication.CreateWithdrawMoney(repo).Value;
        return (withdraw, repo);
    }

    [Fact]
    public void Withdrawing_from_an_account_reduces_its_balance_by_the_withdrawn_amount()
    {
        var (withdraw, repo) = BuildHandler();
        var accountId = Guid.NewGuid();
        repo.SaveAccount(AccountDomain.Open(accountId, new Money(200m)).Value);

        withdraw(accountId, 75m);

        var account = repo.LoadAccount(accountId).Value;
        Assert.Equal(125m, account.Balance.Amount);
    }

    [Fact]
    public void Withdrawing_the_entire_balance_leaves_the_account_at_zero()
    {
        var (withdraw, repo) = BuildHandler();
        var accountId = Guid.NewGuid();
        repo.SaveAccount(AccountDomain.Open(accountId, new Money(100m)).Value);

        withdraw(accountId, 100m);

        var account = repo.LoadAccount(accountId).Value;
        Assert.Equal(0m, account.Balance.Amount);
    }

    [Fact]
    public void Withdrawing_from_a_non_existent_account_returns_AccountNotFound()
    {
        var (withdraw, _) = BuildHandler();

        var result = withdraw(Guid.NewGuid(), 50m);

        Assert.IsType<AccountError.AccountNotFound>(result.Error);
    }

    [Fact]
    public void Withdrawing_more_than_the_available_balance_returns_InsufficientBalance()
    {
        var (withdraw, repo) = BuildHandler();
        var accountId = Guid.NewGuid();
        repo.SaveAccount(AccountDomain.Open(accountId, new Money(50m)).Value);

        var result = withdraw(accountId, 100m);

        Assert.IsType<AccountError.InsufficientBalance>(result.Error);
    }

    [Fact]
    public void AccountNotFound_error_reports_the_requested_account_id()
    {
        var (withdraw, _) = BuildHandler();
        var accountId = Guid.NewGuid();

        var error = Assert.IsType<AccountError.AccountNotFound>(
            withdraw(accountId, 50m).Error
        );

        Assert.Equal(accountId, error.AccountId);
    }

    [Fact]
    public void InsufficientBalance_error_reports_the_current_balance_and_attempted_amount()
    {
        var (withdraw, repo) = BuildHandler();
        var accountId = Guid.NewGuid();
        repo.SaveAccount(AccountDomain.Open(accountId, new Money(50m)).Value);

        var error = Assert.IsType<AccountError.InsufficientBalance>(
            withdraw(accountId, 120m).Error
        );

        Assert.Equal(50m, error.Balance.Amount);
        Assert.Equal(120m, error.Amount.Amount);
    }

    [Fact]
    public void After_a_failed_withdrawal_the_balance_is_unchanged()
    {
        var (withdraw, repo) = BuildHandler();
        var accountId = Guid.NewGuid();
        repo.SaveAccount(AccountDomain.Open(accountId, new Money(50m)).Value);

        withdraw(accountId, 999m);

        var account = repo.LoadAccount(accountId).Value;
        Assert.Equal(50m, account.Balance.Amount);
    }
}
