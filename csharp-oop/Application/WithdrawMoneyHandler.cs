using CsharpOop.Contracts;
using CsharpOop.Domain;

namespace CsharpOop.Applications;

/// Application-layer handler orchestrating the use case
public sealed class WithdrawMoneyHandler
{
    private readonly IAccountRepository _repository;

    public WithdrawMoneyHandler(IAccountRepository repository)
    {
        _repository = repository;
    }

    public void Handle(WithdrawMoneyCommand command)
    {
        Console.WriteLine(
            $"[App] Handling WithdrawMoneyCommand for account {command.AccountId}, amount {command.Amount:0.00}"
        );

        var accountId = new AccountId(command.AccountId);
        var amount = new Money(command.Amount);

        var account =
            _repository.GetById(accountId)
            ?? throw new InvalidOperationException("Account not found.");

        Console.WriteLine($"[App] Account loaded. Current balance: {account.Balance.Amount:0.00}");
        Console.WriteLine($"[App] Executing withdrawal of {amount.Amount:0.00}...");

        account.Withdraw(amount);

        Console.WriteLine($"[App] Withdrawal applied. New balance: {account.Balance.Amount:0.00}");

        _repository.Save(account);
        Console.WriteLine("[App] Account persisted.");
    }
}
