using CsharpFp1.Domain;
using CsharpFp1.Infrastructure;

namespace CsharpFp1.Application;

// delegate type is optional, but nice to illustrate
public delegate void WithdrawMoney(Guid accountId, decimal amount);

public static class AccountApplication
{
    public static WithdrawMoney CreateWithdrawMoney(
        LoadAccount loadAccount,
        SaveAccount saveAccount
    )
    {
        return (accountId, amount) =>
        {
            var account =
                loadAccount(accountId) ?? throw new InvalidOperationException("Account not found.");

            Console.WriteLine(
                $"[App] Account loaded. Current balance: {account.Balance.Amount:0.00}"
            );
            Console.WriteLine($"[App] Executing withdrawal of {amount:0.00}...");

            var modifiedAccount = AccountDomain.Withdraw(account, new Money(amount));

            Console.WriteLine(
                $"[App] Withdrawal applied. New balance: {modifiedAccount.Balance.Amount:0.00}"
            );

            saveAccount(modifiedAccount);

            Console.WriteLine("[App] Account persisted.");
        };
    }
}
