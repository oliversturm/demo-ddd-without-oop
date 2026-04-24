import { bind, ok, tap, tapError, pipe } from '../library/result.js';
import { output, outputError } from '../library/logging.js';
import { withdraw } from '../domain/account.js';
import { createMoney } from '../domain/money.js';

/**
 * @typedef {import('../infrastructure/repository.js').Repository} Repository
 * @typedef {import('../domain/account.js').Account} Account
 * @typedef {import('../domain/account.js').AccountError} AccountError
 * @typedef {import('../infrastructure/repository.js').AppError} AppError
 * @typedef {import('../library/result.js').Result<Account, AccountError>} AccountResult
 * @typedef {(accountId: string, amount: number) => AccountResult} WithdrawMoneyFn
 * @typedef {import('../library/result.js').Result<WithdrawMoneyFn, AppError>} CreateWithdrawMoneyResult
 */

/**
 * @param {string} src
 * @param {string} msg
 * @returns {<T, E>(result: import('../library/result.js').Result<T, E>) => import('../library/result.js').Result<T, E>}
 */
const log = (src, msg) => (result) =>
  tap((x) => output(src, msg)(x))(tapError((e) => outputError(src, e))(result));

/**
 * @param {string} src
 * @param {(x: T) => string} renderText
 * @returns {<T, E>(result: import('../library/result.js').Result<T, E>) => import('../library/result.js').Result<T, E>}
 */
const logWith = (src, renderText) => (result) =>
  tap((x) => output(src, renderText(x))(x))(tapError((e) => outputError(src, e))(result));

/**
 * @param {Repository} repo
 * @returns {CreateWithdrawMoneyResult}
 */
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
