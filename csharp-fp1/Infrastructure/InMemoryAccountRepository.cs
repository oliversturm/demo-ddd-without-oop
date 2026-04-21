using CsharpFp1.Domain;

namespace CsharpFp1.Infrastructure;

/// Simple in-memory repository for demonstration
public class InMemoryAccountRepository : IAccountRepository
{
    private readonly Dictionary<AccountId, Account> _accounts = new();

    public Account? GetById(AccountId id)
    {
        var found = _accounts.TryGetValue(id, out var account);
        Console.WriteLine(
            found ? $"[Repo] Loaded account {id.Value}" : $"[Repo] Account {id.Value} not found"
        );

        return found ? account : null;
    }

    public void Save(Account account)
    {
        _accounts[account.Id] = account;
        Console.WriteLine(
            $"[Repo] Saved account {account.Id.Value} with balance {account.Balance.Amount:0.00}"
        );
    }
}
