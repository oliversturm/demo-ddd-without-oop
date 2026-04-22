namespace CsharpFp1.Domain;

/// Repository abstraction used to load and save accounts
public interface IAccountRepository
{
    Account? GetById(Guid id);
    void Save(Account account);
}
