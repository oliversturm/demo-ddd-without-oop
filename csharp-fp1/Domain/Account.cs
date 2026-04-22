namespace CsharpFp1.Domain;

public sealed record Account(Guid Id, Money Balance);

public static class AccountDomain
{
    // Choosing a "clean" FP approach here of instantiating the Account
    // Depending on needs, code to prevent the Account type from
    //   being instantiated without this helper needs to be added
    //   to the record type.
    public static Account Open(Guid id, Money openingBalance)
    {
        if (openingBalance.Amount < 0)
            throw new ArgumentOutOfRangeException(nameof(openingBalance));

        return new Account(id, openingBalance);
    }

    public static Account Withdraw(Account account, Money amount)
    {
        if (amount.Amount <= 0)
            throw new InvalidOperationException("Withdrawal amount must be positive.");

        if (account.Balance.Amount < amount.Amount)
            throw new InsufficientBalanceException(account.Balance, amount);

        return account with
        {
            Balance = account.Balance.Subtract(amount),
        };
    }
}
