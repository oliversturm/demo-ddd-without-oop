using CsharpFp2.Domain;
using CsharpFp2.Infrastructure;

namespace CsharpFp2.Application;

// delegate type is optional, but nice to illustrate
public delegate WithdrawResult WithdrawMoney(Guid accountId, decimal amount);

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

            switch (AccountDomain.Withdraw(account, new Money(amount)))
            {
                case WithdrawResult.Success s:
                {
                    Console.WriteLine(
                        $"[App] Withdrawal applied. New balance: {s.Account.Balance.Amount:0.00}"
                    );
                    saveAccount(s.Account);
                    Console.WriteLine("[App] Account persisted.");
                    return s;
                }

                case var other:
                {
                    Console.Error.WriteLine($"[App Error] Withdrawal failed. Error: {other}");
                    return other;
                }
            }
        };
    }
}
