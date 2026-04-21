using CsharpFp2.Domain;

namespace CsharpFp2.Infrastructure;

/// Simple in-memory repository for demonstration
public class InMemoryAccountRepository : IAccountRepository
{
    private readonly Dictionary<Guid, Account> _accounts = new();

    public Account? GetById(Guid id)
    {
        var found = _accounts.TryGetValue(id, out var account);
        Console.WriteLine(found ? $"[Repo] Loaded account {id}" : $"[Repo] Account {id} not found");

        return found ? account : null;
    }

    public void Save(Account account)
    {
        _accounts[account.Id] = account;
        Console.WriteLine(
            $"[Repo] Saved account {account.Id} with balance {account.Balance.Amount:0.00}"
        );
    }
}
