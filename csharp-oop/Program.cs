using CsharpOop.Applications;
using CsharpOop.Contracts;
using CsharpOop.Domain;
using CsharpOop.Infrastructure;

namespace CsharpOop;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("[csharp-oop] Starting withdraw money demo...");

        var repository = new InMemoryAccountRepository();
        var handler = new WithdrawMoneyHandler(repository);

        var accountId = Guid.NewGuid();
        Console.WriteLine($"[csharp-oop] Seeding account {accountId} with opening balance 200.00");
        repository.Save(new Account(new AccountId(accountId), new Money(200m)));

        var command = new WithdrawMoneyCommand { AccountId = accountId, Amount = 100m };
        Console.WriteLine(
            $"[csharp-oop] Dispatching command: withdraw {command.Amount:0.00} from account {command.AccountId}"
        );

        handler.Handle(command);

        Console.WriteLine("[csharp-oop] Demo completed.");
    }
}
