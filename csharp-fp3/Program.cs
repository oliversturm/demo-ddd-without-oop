using CsharpFp3.Application;
using CsharpFp3.Domain;
using CsharpFp3.Infrastructure;
using CsharpFp3.Library;

Console.WriteLine("[csharp-fp3] Starting withdraw money demo...");

var accountId = Guid.NewGuid();

// try changing this to 250m to see an error being handled
var withdrawalAmount = 100m;

Result<Repository, AppError>
    .Catch(
        InMemoryAccountRepository.Create,
        ex => new AppError.RepositoryCreationFailed(ex.Message)
    )
    .Bind(repo =>
        AccountApplication
            .CreateWithdrawMoney(repo)
            .Bind(withdrawMoney =>
                AccountDomain
                    .Open(accountId, new Money(200m))
                    .Log(
                        "csharp-fp3 seed",
                        account =>
                            $"Seeding account {account.Id} with opening balance {account.Balance}"
                    )
                    .Bind(repo.SaveAccount)
                    .Log(
                        "csharp-fp3 exec",
                        $"Executing withdrawal {withdrawalAmount:0.00} from account {accountId}"
                    )
                    .Bind<Account, Account, AccountError>(account =>
                        withdrawMoney(account.Id, withdrawalAmount)
                    )
                    .Log("csharp-fp3 new balance", account => $"New balance is {account.Balance}")
                    .MapError(ae => (AppError)new AppError.InnerAccountError(ae))
            )
    )
    .Log("csharp-fp3 done", "Demo completed.");
