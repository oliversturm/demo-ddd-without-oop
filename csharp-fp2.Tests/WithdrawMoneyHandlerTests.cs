using CsharpFp2.Applications;
using CsharpFp2.Contracts;
using CsharpFp2.Domain;
using CsharpFp2.Infrastructure;

namespace CsharpFp2.Tests;

/// Tests covering the application-layer use case surface:
/// what the WithdrawMoney feature does from the caller's perspective.
public class WithdrawMoneyHandlerTests
{
    private static (
        WithdrawMoneyHandler handler,
        InMemoryAccountRepository repository
    ) BuildHandler()
    {
        var repository = new InMemoryAccountRepository();
        var handler = new WithdrawMoneyHandler(repository);
        return (handler, repository);
    }

    [Fact]
    public void Withdrawing_from_an_account_reduces_its_balance_by_the_withdrawn_amount()
    {
        var (handler, repository) = BuildHandler();
        var accountId = Guid.NewGuid();
        repository.Save(new Account(new AccountId(accountId), new Money(200m)));

        handler.Handle(new WithdrawMoneyCommand { AccountId = accountId, Amount = 75m });

        var account = repository.GetById(new AccountId(accountId))!;
        Assert.Equal(125m, account.Balance.Amount);
    }

    [Fact]
    public void Withdrawing_the_entire_balance_leaves_the_account_at_zero()
    {
        var (handler, repository) = BuildHandler();
        var accountId = Guid.NewGuid();
        repository.Save(new Account(new AccountId(accountId), new Money(100m)));

        handler.Handle(new WithdrawMoneyCommand { AccountId = accountId, Amount = 100m });

        var account = repository.GetById(new AccountId(accountId))!;
        Assert.Equal(0m, account.Balance.Amount);
    }

    [Fact]
    public void Withdrawing_from_a_non_existent_account_throws()
    {
        var (handler, _) = BuildHandler();
        var command = new WithdrawMoneyCommand { AccountId = Guid.NewGuid(), Amount = 50m };

        Assert.Throws<InvalidOperationException>(() => handler.Handle(command));
    }

    [Fact]
    public void Withdrawing_more_than_the_available_balance_throws()
    {
        var (handler, repository) = BuildHandler();
        var accountId = Guid.NewGuid();
        repository.Save(new Account(new AccountId(accountId), new Money(50m)));

        Assert.Throws<InsufficientBalanceException>(() =>
            handler.Handle(new WithdrawMoneyCommand { AccountId = accountId, Amount = 100m })
        );
    }

    [Fact]
    public void After_a_failed_withdrawal_the_balance_is_unchanged()
    {
        var (handler, repository) = BuildHandler();
        var accountId = Guid.NewGuid();
        repository.Save(new Account(new AccountId(accountId), new Money(50m)));

        try
        {
            handler.Handle(new WithdrawMoneyCommand { AccountId = accountId, Amount = 999m });
        }
        catch (InsufficientBalanceException) { }

        var account = repository.GetById(new AccountId(accountId))!;
        Assert.Equal(50m, account.Balance.Amount);
    }
}
