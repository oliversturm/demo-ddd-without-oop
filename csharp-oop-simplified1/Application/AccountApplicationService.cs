using CsharpOopSimplified1.Domain;

namespace CsharpOopSimplified1.Application;

public static class AccountApplicationService
{
    public static void WithdrawMoney(IAccountRepository repository, Guid accountId, decimal amount)
    {
        var account =
            repository.GetById(new AccountId(accountId))
            ?? throw new InvalidOperationException("Account not found.");

        Console.WriteLine($"[App] Account loaded. Current balance: {account.Balance.Amount:0.00}");
        Console.WriteLine($"[App] Executing withdrawal of {amount:0.00}...");

        account.Withdraw(new Money(amount));

        Console.WriteLine($"[App] Withdrawal applied. New balance: {account.Balance.Amount:0.00}");

        repository.Save(account);

        Console.WriteLine("[App] Account persisted.");
    }
}
