using CsharpEs.Library;
using static CsharpEs.Library.ResultModule;

namespace CsharpEs.Domain;

public sealed record AccountState(Guid Id);

public abstract record AccountCommand
{
    public sealed record OpenAccount(Guid AccountId, Money OpeningBalance) : AccountCommand;

    public sealed record WithdrawMoney(Guid AccountId, Money Amount) : AccountCommand;

    public sealed record DepositMoney(Guid AccountId, Money Amount) : AccountCommand;

    public sealed record UnhandledTestCommand() : AccountCommand;
}

public abstract record AccountEvent(Guid AccountId)
{
    public sealed record AccountOpened(Guid AccountId, Money OpeningBalance)
        : AccountEvent(AccountId);

    public sealed record MoneyWithdrawn(Guid AccountId, Money Amount) : AccountEvent(AccountId);

    public sealed record MoneyDeposited(Guid AccountId, Money Amount) : AccountEvent(AccountId);
}

public abstract record AccountError
{
    public sealed record AccountNotFound : AccountError;

    public sealed record AccountOpenAlready : AccountError;

    public sealed record OpeningBalanceMustBeNonNegative : AccountError;

    public sealed record InnerException(string Message) : AccountError;
}

public static class AccountDecider
{
    public static Result<AccountEvent, AccountError> Decide(
        AccountState? state,
        AccountCommand command
    ) =>
        Result<AccountEvent, AccountError>.Catch(
            () =>
                (state, command) switch
                {
                    // if this is a new account, check for valid opening balance
                    (null, AccountCommand.OpenAccount c) when c.OpeningBalance.Amount < 0m => Fail(
                        new AccountError.OpeningBalanceMustBeNonNegative()
                    ),

                    // still a new account, now we can open it
                    (null, AccountCommand.OpenAccount c) => Ok(
                        new AccountEvent.AccountOpened(c.AccountId, c.OpeningBalance)
                    ),

                    // if we have an account already, you can't open it
                    (not null, AccountCommand.OpenAccount c) => Fail(
                        new AccountError.AccountOpenAlready()
                    ),

                    (null, _) => Fail(new AccountError.AccountNotFound()),

                    (_, AccountCommand.WithdrawMoney c) => Ok(
                        new AccountEvent.MoneyWithdrawn(c.AccountId, c.Amount)
                    ),

                    (_, AccountCommand.DepositMoney c) => Ok(
                        new AccountEvent.MoneyDeposited(c.AccountId, c.Amount)
                    ),

                    _ => throw new InvalidOperationException("Unknown command."),
                },
            e => new AccountError.InnerException(e.Message)
        );

    public static Result<AccountState?, AccountError> Evolve(
        AccountState? state,
        AccountEvent @event
    ) =>
        (state, @event) switch
        {
            (null, AccountEvent.AccountOpened e) => Ok(new AccountState(e.AccountId)),

            _ => Ok(state),
        };
}
