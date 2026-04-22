using CsharpFp2.Domain;

namespace CsharpFp2.Infrastructure;

// If we don't like working with generic delegates directly, we can
// create custom named delegate types.
public delegate Account? LoadAccount(Guid id);

public delegate void SaveAccount(Account accunt);

// If we don't want to use tuples, or you really miss the idea of a combined "interface",
// we can create a named container
//   public sealed record AccountPersistence(LoadAccount Load, SaveAccount Save);

public static class InMemoryAccount
{
    public static (LoadAccount, SaveAccount) Create()
    {
        Dictionary<Guid, Account> store = new();

        Account? GetById(Guid id)
        {
            var found = store.TryGetValue(id, out var account);
            Console.WriteLine(
                found ? $"[Repo] Loaded account {id}" : $"[Repo] Account {id} not found"
            );

            return found ? account : null;
        }

        void Save(Account account)
        {
            store[account.Id] = account;
            Console.WriteLine(
                $"[Repo] Saved account {account.Id} with balance {account.Balance.Amount:0.00}"
            );
        }

        return (GetById, Save);
    }
}
