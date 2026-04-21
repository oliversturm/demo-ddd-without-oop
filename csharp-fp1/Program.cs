using CsharpFp1.Application;
using CsharpFp1.Domain;
using CsharpFp1.Infrastructure;

namespace CsharpFp1;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("[csharp-fp1] Starting withdraw money demo...");

        var repository = new InMemoryAccountRepository();

        var accountId = Guid.NewGuid();
        Console.WriteLine($"[csharp-fp1] Seeding account {accountId} with opening balance 200.00");
        repository.Save(new Account(new AccountId(accountId), new Money(200m)));

        decimal amount = 100m;
        Console.WriteLine($"[csharp-fp1] Executing withdrawal {amount:0.00} from account {accountId}");
        AccountApplicationService.WithdrawMoney(repository, accountId, amount);

        Console.WriteLine("[csharp-fp1] Demo completed.");
    }
}
