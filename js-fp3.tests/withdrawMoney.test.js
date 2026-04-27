import { describe, it, expect } from 'vitest';
import { createWithdrawMoney } from '../js-fp3/application/accountApp.js';
import { openAccount } from '../js-fp3/domain/account.js';
import { createMoney } from '../js-fp3/domain/money.js';
import { createInMemoryRepository } from '../js-fp3/infrastructure/repository.js';

const buildHandler = () => {
  const repo = createInMemoryRepository();
  const withdraw = createWithdrawMoney(repo).value;
  return { withdraw, repo };
};

describe('withdrawMoney application use case', () => {
  it('withdrawing from an account reduces its balance by the withdrawn amount', () => {
    const { withdraw, repo } = buildHandler();
    const accountId = 'acc-1';
    repo.saveAccount(openAccount(accountId, createMoney(200)).value);

    withdraw(accountId, 75);

    const account = repo.loadAccount(accountId).value;
    expect(account.balance.amount).toBe(125);
  });

  it('withdrawing the entire balance leaves the account at zero', () => {
    const { withdraw, repo } = buildHandler();
    const accountId = 'acc-2';
    repo.saveAccount(openAccount(accountId, createMoney(100)).value);

    withdraw(accountId, 100);

    const account = repo.loadAccount(accountId).value;
    expect(account.balance.amount).toBe(0);
  });

  it('withdrawing from a non-existent account returns accountNotFound', () => {
    const { withdraw } = buildHandler();

    const result = withdraw('no-such-id', 50);

    expect(result.error.type).toBe('AccountNotFound');
  });

  it('withdrawing more than the available balance returns insufficientBalance', () => {
    const { withdraw, repo } = buildHandler();
    const accountId = 'acc-3';
    repo.saveAccount(openAccount(accountId, createMoney(50)).value);

    const result = withdraw(accountId, 100);

    expect(result.error.type).toBe('InsufficientBalance');
  });

  it('accountNotFound error reports the requested account id', () => {
    const { withdraw } = buildHandler();
    const accountId = 'missing-acc';

    const error = withdraw(accountId, 50).error;

    expect(error.type).toBe('AccountNotFound');
    expect(error.accountId).toBe(accountId);
  });

  it('insufficientBalance error reports the current balance and attempted amount', () => {
    const { withdraw, repo } = buildHandler();
    const accountId = 'acc-4';
    repo.saveAccount(openAccount(accountId, createMoney(50)).value);

    const error = withdraw(accountId, 120).error;

    expect(error.type).toBe('InsufficientBalance');
    expect(error.balance.amount).toBe(50);
    expect(error.amount.amount).toBe(120);
  });

  it('after a failed withdrawal the balance is unchanged', () => {
    const { withdraw, repo } = buildHandler();
    const accountId = 'acc-5';
    repo.saveAccount(openAccount(accountId, createMoney(50)).value);

    withdraw(accountId, 999);

    const account = repo.loadAccount(accountId).value;
    expect(account.balance.amount).toBe(50);
  });
});
