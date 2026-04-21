using CsharpFp2.Domain;

namespace CsharpFp2.Application;

public static class AccountApplicationService
{
    public static void WithdrawMoney(IAccountRepository repository, Guid accountId, decimal amount)
    {
        var account =
            repository.GetById(accountId)
            ?? throw new InvalidOperationException("Account not found.");

        account.Withdraw(new Money(amount));

        repository.Save(account);
    }
}
