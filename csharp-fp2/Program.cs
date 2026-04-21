using CsharpFp2.Application;
using CsharpFp2.Domain;
using CsharpFp2.Infrastructure;

namespace CsharpFp2;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("[csharp-fp2] Starting withdraw money demo...");

        var repository = new InMemoryAccountRepository();

        var accountId = Guid.NewGuid();
        Console.WriteLine($"[csharp-fp2] Seeding account {accountId} with opening balance 200.00");
        repository.Save(new Account(accountId, new Money(200m)));

        decimal amount = 100m;
        Console.WriteLine(
            $"[csharp-fp2] Executing withdrawal {amount:0.00} from account {accountId}"
        );
        AccountApplicationService.WithdrawMoney(repository, accountId, amount);

        Console.WriteLine("[csharp-fp2] Demo completed.");
    }
}
