using CsharpOopSimplified1.Application;
using CsharpOopSimplified1.Domain;
using CsharpOopSimplified1.Infrastructure;

Console.WriteLine("[csharp-oop-simplified1] Starting withdraw money demo...");

var repository = new InMemoryAccountRepository();

var accountId = Guid.NewGuid();
Console.WriteLine(
    $"[csharp-oop-simplified1] Seeding account {accountId} with opening balance 200.00"
);
repository.Save(new Account(new AccountId(accountId), new Money(200m)));

decimal amount = 100m;
Console.WriteLine(
    $"[csharp-oop-simplified1] Executing withdrawal {amount:0.00} from account {accountId}"
);
AccountApplicationService.WithdrawMoney(repository, accountId, amount);

Console.WriteLine("[csharp-oop-simplified1] Demo completed.");
