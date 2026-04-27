import { bind, ok, pipe } from '../library/result.js';
import { log, logWith, formatMoney } from '../library/logging.js';
import { withdraw } from '../domain/account.js';
import { createMoney } from '../domain/money.js';

export const createWithdrawMoney = (repo) =>
  ok((accountId, amount) =>
    pipe(
      repo.loadAccount(accountId),
      log('App load', 'Account loaded'),
      log('App exec wdrwl', `[App] Executing withdrawal of ${amount.toFixed(2)}...`),
      bind((account) => withdraw(account, createMoney(amount))),
      logWith('App wdrwl done', (a) => `Withdrawal applied. New balance: ${formatMoney(a.balance)}`),
      bind(repo.saveAccount),
      log('App acc svd', '[App] Account persisted.')
    )
  );
