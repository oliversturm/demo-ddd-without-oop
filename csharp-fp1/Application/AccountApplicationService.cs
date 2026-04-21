using CsharpFp1.Domain;

namespace CsharpFp1.Application;

public static class AccountApplicationService
{
    public static void WithdrawMoney(IAccountRepository repository, Guid accountId, decimal amount)
    {
        var account =
            repository.GetById(new AccountId(accountId))
            ?? throw new InvalidOperationException("Account not found.");

        account.Withdraw(new Money(amount));

        repository.Save(account);
    }
}
