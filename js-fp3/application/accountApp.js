import { bind, ok, tap, tapError, pipe } from '../library/result.js';
import { output, outputError } from '../library/logging.js';
import { withdraw } from '../domain/account.js';
import { createMoney } from '../domain/money.js';

const log = (src, msg) => (result) =>
  tap((x) => output(src, msg)(x))(tapError((e) => outputError(src, e))(result));

const logWith = (src, renderText) => (result) =>
  tap((x) => output(src, renderText(x))(x))(tapError((e) => outputError(src, e))(result));

export const createWithdrawMoney = (repo) =>
  ok((accountId, amount) =>
    pipe(
      repo.loadAccount(accountId),
      log('App load', 'Account loaded'),
      log('App exec wdrwl', `[App] Executing withdrawal of ${amount.toFixed(2)}...`),
      bind((account) => withdraw(account, createMoney(amount))),
      logWith('App wdrwl done', (a) => `Withdrawal applied. New balance: ${a.balance.amount.toFixed(2)}`),
      bind(repo.saveAccount),
      log('App acc svd', '[App] Account persisted.')
    )
  );
