using CsharpFp3.Library;

namespace CsharpFp3.Domain;

public sealed record Account(Guid Id, Money Balance);

public abstract record AccountError
{
    public sealed record OpeningBalanceMustBeNonNegative : AccountError;

    public sealed record AmountMustBePositive : AccountError;

    public sealed record InsufficientBalance(Money Balance, Money Amount) : AccountError;

    public sealed record AccountNotFound(Guid AccountId) : AccountError;
}

public static class AccountDomain
{
    public static Result<Account, AccountError> Open(Guid id, Money openingBalance) =>
        openingBalance.Amount < 0m
            ? Result<Account, AccountError>.Fail(new AccountError.OpeningBalanceMustBeNonNegative())
            : Result<Account, AccountError>.Ok(new Account(id, openingBalance));

    public static Result<Account, AccountError> Withdraw(Account account, Money amount) =>
        (account.Balance.Amount, amount.Amount) switch
        {
            (_, <= 0m) => Result<Account, AccountError>.Fail(
                new AccountError.AmountMustBePositive()
            ),

            var (balance, transaction) when balance < transaction => Result<
                Account,
                AccountError
            >.Fail(new AccountError.InsufficientBalance(account.Balance, amount)),

            var (balance, transaction) => Result<Account, AccountError>.Ok(
                account with
                {
                    Balance = new Money(balance - transaction),
                }
            ),
        };
}
