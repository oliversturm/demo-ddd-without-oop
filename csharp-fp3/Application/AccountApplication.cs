using CsharpFp3.Domain;
using CsharpFp3.Infrastructure;
using CsharpFp3.Library;

namespace CsharpFp3.Application;

using CreateResultType = Result<Func<Guid, decimal, Result<Account, AccountError>>, AppError>;

public abstract record AppError
{
    public sealed record RepositoryCreationFailed(string Message) : AppError;

    public sealed record InnerAccountError(AccountError InnerError) : AppError;
}

public static class AccountApplication
{
    public static CreateResultType CreateWithdrawMoney(Repository repo) =>
        CreateResultType.Ok(
            (accountId, amount) =>
                repo.LoadAccount(accountId)
                    .Log("App load", "Account loaded")
                    .Log("App exec wdrwl", $"[App] Executing withdrawal of {amount:0.00}...")
                    .Bind(account => AccountDomain.Withdraw(account, new Money(amount)))
                    .Log(
                        "App wdrwl done",
                        a => $"Withdrawal applied. New balance: {a.Balance.Amount:0.00}"
                    )
                    .Bind(repo.SaveAccount)
                    .Log("App acc svd", "[App] Account persisted.")
        );
}
