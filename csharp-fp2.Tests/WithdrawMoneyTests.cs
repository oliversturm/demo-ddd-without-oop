using CsharpFp2.Application;
using CsharpFp2.Domain;
using CsharpFp2.Infrastructure;

namespace CsharpFp2.Tests;

/// Tests covering the application-layer use case surface:
/// what the WithdrawMoney feature does from the caller's perspective.
public class WithdrawMoneyTests
{
    private static (
        WithdrawMoney withdraw,
        LoadAccount loadAccount,
        SaveAccount saveAccount
    ) BuildHandler()
    {
        var (loadAccount, saveAccount) = InMemoryAccountRepository.Create();
        var withdraw = AccountApplication.CreateWithdrawMoney(loadAccount, saveAccount);
        return (withdraw, loadAccount, saveAccount);
    }

    [Fact]
    public void Withdrawing_from_an_account_reduces_its_balance_by_the_withdrawn_amount()
    {
        var (withdraw, loadAccount, saveAccount) = BuildHandler();
        var accountId = Guid.NewGuid();
        saveAccount(AccountDomain.Open(accountId, new Money(200m)));

        withdraw(accountId, 75m);

        var account = loadAccount(accountId)!;
        Assert.Equal(125m, account.Balance.Amount);
    }

    [Fact]
    public void Withdrawing_the_entire_balance_leaves_the_account_at_zero()
    {
        var (withdraw, loadAccount, saveAccount) = BuildHandler();
        var accountId = Guid.NewGuid();
        saveAccount(AccountDomain.Open(accountId, new Money(100m)));

        withdraw(accountId, 100m);

        var account = loadAccount(accountId)!;
        Assert.Equal(0m, account.Balance.Amount);
    }

    [Fact]
    public void Withdrawing_from_a_non_existent_account_throws()
    {
        var (withdraw, _, _) = BuildHandler();

        Assert.Throws<InvalidOperationException>(() => withdraw(Guid.NewGuid(), 50m));
    }

    [Fact]
    public void Withdrawing_more_than_the_available_balance_returns_InsufficientBalance()
    {
        var (withdraw, _, saveAccount) = BuildHandler();
        var accountId = Guid.NewGuid();
        saveAccount(AccountDomain.Open(accountId, new Money(50m)));

        var result = withdraw(accountId, 100m);

        Assert.IsType<WithdrawResult.InsufficientBalance>(result);
    }

    [Fact]
    public void After_a_failed_withdrawal_the_balance_is_unchanged()
    {
        var (withdraw, loadAccount, saveAccount) = BuildHandler();
        var accountId = Guid.NewGuid();
        saveAccount(AccountDomain.Open(accountId, new Money(50m)));

        withdraw(accountId, 999m);

        var account = loadAccount(accountId)!;
        Assert.Equal(50m, account.Balance.Amount);
    }
}
