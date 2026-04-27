using System.Collections.Immutable;
using CsharpEs.Domain;
using CsharpEs.Infrastructure;
using CsharpEs.Library;

public abstract record DemoError
{
    public sealed record Account(AccountError Error) : DemoError;

    public sealed record ReadModel(ReadModelError Error) : DemoError;
}

public class Program
{
    private static Guid accountId;
    private static ImmutableList<AccountCommand> demoCommands = ImmutableList<AccountCommand>.Empty;

    static Program()
    {
        accountId = Guid.NewGuid();
        demoCommands = demoCommands.AddRange([
            // bring this in to see an error right from the start
            // new AccountCommand.OpenAccount(accountId, new Money(-200m)),
            new AccountCommand.OpenAccount(accountId, new Money(200m)),
            new AccountCommand.WithdrawMoney(accountId, new Money(100m)),
            new AccountCommand.WithdrawMoney(accountId, new Money(200m)),
            // or maybe an exception in between?
            // new AccountCommand.UnhandledTestCommand(),
            new AccountCommand.DepositMoney(accountId, new Money(500m)),
        ]);
    }

    static ImmutableList<AccountEvent> eventStore = ImmutableList<AccountEvent>.Empty;

    static Result<AccountState?, DemoError> ApplyEvent(
        AccountBalanceReadModel balanceProjection,
        AccountState? state,
        AccountEvent @event
    )
    {
        eventStore = eventStore.Add(@event);
        Logging.Output(
            "ae",
            $"Applying event {Logging.Format(@event)} to state {Logging.Format(state)}"
        );
        return AccountDecider
            .Evolve(state, @event)
            .MapError(e => (DemoError)new DemoError.Account(e))
            .Bind(newState =>
                balanceProjection
                    .Project(@event)
                    .MapError(e => (DemoError)new DemoError.ReadModel(e))
                    .Log("ae ad", "Account details")
                    .Map(_ => newState)
            )
            .Log("ae", $"Applied and projected {Logging.Format(@event)}");
    }

    public static void Main(string[] args)
    {
        Logging.Output("csharp-es", "Starting money handling demo...");

        AccountBalanceReadModelModule
            .Create()
            .MapError(e => (DemoError)new DemoError.ReadModel(e))
            .Bind(readModel =>
                demoCommands
                    .Aggregate(
                        Result<AccountState?, DemoError>.Ok(null),
                        (stateResult, command) =>
                            stateResult
                                .Bind(state =>
                                    AccountDecider
                                        .Decide(state, command)
                                        .MapError(e => (DemoError)new DemoError.Account(e))
                                        .Bind(@event => ApplyEvent(readModel, state, @event))
                                )
                                .Log("loop", "Intermediate state")
                    )
                    .Bind(s =>
                        readModel
                            .Query()
                            .Log("query", "Read model data")
                            .MapError(e => (DemoError)new DemoError.ReadModel(e))
                    )
            )
            .Log("csharp-es done", "Demo completed.");
    }
}
