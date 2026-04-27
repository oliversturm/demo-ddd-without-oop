using System.Collections.Immutable;
using CsharpEs.Domain;
using CsharpEs.Library;

namespace CsharpEs.Infrastructure;

public abstract record ReadModelError
{
    public sealed record AccountDetailsNotFound : ReadModelError;
}

public sealed record AccountBalanceReadModel(
    Func<AccountEvent, Result<AccountDetails, ReadModelError>> Project,
    Func<Result<KeyValuePair<Guid, AccountDetails>[], ReadModelError>> Query
);

[Flags]
public enum AccountFlags
{
    None = 0,
    InOverdraft = 1,
}

public sealed record AccountDetails(Money Balance, AccountFlags Flags);

public static class AccountBalanceReadModelModule
{
    public static Result<AccountBalanceReadModel, ReadModelError> Create()
    {
        // This is our read-model specific "persistent" storage
        var modelData = ImmutableDictionary<Guid, AccountDetails>.Empty;

        Result<AccountDetails, ReadModelError> Project(AccountEvent @event)
        {
            Logging.Output("rm", "Projecting event", @event);
            switch (@event)
            {
                case AccountEvent.AccountOpened e:
                    modelData = modelData.SetItem(
                        e.AccountId,
                        new AccountDetails(e.OpeningBalance, AccountFlags.None)
                    );
                    break;

                case AccountEvent.MoneyWithdrawn e:
                    {
                        if (modelData.TryGetValue(e.AccountId, out var oldDetails))
                        {
                            var newBalance = new Money(oldDetails.Balance.Amount - e.Amount.Amount);
                            modelData = modelData.SetItem(
                                e.AccountId,
                                oldDetails with
                                {
                                    Balance = newBalance,
                                    Flags = CalcAccountFlags(newBalance, oldDetails),
                                }
                            );
                        }
                        else
                            return Result<AccountDetails, ReadModelError>.Fail(
                                new ReadModelError.AccountDetailsNotFound()
                            );
                    }
                    break;

                case AccountEvent.MoneyDeposited e:
                    {
                        if (modelData.TryGetValue(e.AccountId, out var oldDetails))
                        {
                            var newBalance = new Money(oldDetails.Balance.Amount + e.Amount.Amount);
                            modelData = modelData.SetItem(
                                e.AccountId,
                                oldDetails with
                                {
                                    Balance = newBalance,
                                    Flags = CalcAccountFlags(newBalance, oldDetails),
                                }
                            );
                        }
                        else
                            return Result<AccountDetails, ReadModelError>.Fail(
                                new ReadModelError.AccountDetailsNotFound()
                            );
                    }
                    break;
            }

            // Read model-side "project" may not normally return any data, but for demo
            // purposes we return the affected data directly to save us modeling a separate
            // query side.
            return Result<AccountDetails, ReadModelError>.Ok(modelData[@event.AccountId]);
        }

        Result<KeyValuePair<Guid, AccountDetails>[], ReadModelError> Query() =>
            Result<KeyValuePair<Guid, AccountDetails>[], ReadModelError>.Ok(modelData.ToArray());

        return Result<AccountBalanceReadModel, ReadModelError>.Ok(
            new AccountBalanceReadModel(Project, Query)
        );
    }

    private static AccountFlags CalcAccountFlags(Money newBalance, AccountDetails oldDetails)
    {
        return newBalance.Amount < 0
            ? oldDetails.Flags | AccountFlags.InOverdraft
            : oldDetails.Flags & ~AccountFlags.InOverdraft;
    }
}
