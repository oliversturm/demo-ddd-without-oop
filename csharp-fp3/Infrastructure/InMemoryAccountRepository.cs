using CsharpFp3.Domain;
using CsharpFp3.Library;
using static CsharpFp3.Library.Logging;
using static CsharpFp3.Library.ResultModule;

namespace CsharpFp3.Infrastructure;

public sealed record Repository(
    Func<Guid, Result<Account, AccountError>> LoadAccount,
    Func<Account, Result<Account, AccountError>> SaveAccount
);

public static class InMemoryAccountRepository
{
    public static Repository Create()
    {
        Dictionary<Guid, Account> store = new();

        Result<Account, AccountError> GetById(Guid id) =>
            store.TryGetValue(id, out var account)
                ? LogReturn($"[Repo] Loaded account {id}", Ok(account))
                : LogReturn(
                    $"[Repo] Account {id} not found",
                    Fail(new AccountError.AccountNotFound(id))
                );

        Result<Account, AccountError> Save(Account account)
        {
            store[account.Id] = account;

            // no failure modes here
            return LogReturn(
                $"[Repo] Saved account {account.Id} with balance {account.Balance.Amount:0.00}",
                Ok(account)
            );
        }

        // What if something goes wrong?
        // throw new Exception("Something went wrong");
        return new Repository(GetById, Save);
    }
}
