namespace CsharpFp2.Domain;

public sealed record Account(Guid Id, Money Balance);

public abstract record WithdrawResult
{
    public sealed record Success(Account Account) : WithdrawResult;

    public sealed record AmountMustBePositive : WithdrawResult;

    public sealed record InsufficientBalance(Money balance, Money amount) : WithdrawResult;
}

public static class AccountDomain
{
    public static Account Open(Guid id, Money openingBalance)
    {
        // Assuming we don't intend to allow invalid open balances to be passed
        // here, we can view this as a technical (i.e. dev-time) error and
        // stick to an exception.
        if (openingBalance.Amount < 0)
            throw new ArgumentOutOfRangeException(nameof(openingBalance));

        return new Account(id, openingBalance);
    }

    public static WithdrawResult Withdraw(Account account, Money amount)
    {
        if (amount.Amount <= 0)
            return new WithdrawResult.AmountMustBePositive();

        if (account.Balance.Amount < amount.Amount)
            return new WithdrawResult.InsufficientBalance(account.Balance, amount);

        return new WithdrawResult.Success(
            account with
            {
                Balance = account.Balance.Subtract(amount),
            }
        );
    }
}
