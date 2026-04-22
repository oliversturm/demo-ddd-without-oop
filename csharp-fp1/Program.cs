using CsharpFp1.Application;
using CsharpFp1.Domain;
using CsharpFp1.Infrastructure;

Console.WriteLine("[csharp-fp1] Starting withdraw money demo...");

var (loadAccount, saveAccount) = InMemoryAccount.Create();
var withdrawMoney = AccountApplication.CreateWithdrawMoney(loadAccount, saveAccount);

var accountId = Guid.NewGuid();
Console.WriteLine($"[csharp-fp1] Seeding account {accountId} with opening balance 200.00");
saveAccount(new Account(accountId, new Money(200m)));

decimal amount = 100m;
Console.WriteLine($"[csharp-fp1] Executing withdrawal {amount:0.00} from account {accountId}");
withdrawMoney(accountId, amount);

Console.WriteLine("[csharp-fp1] Demo completed.");
