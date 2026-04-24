import { randomUUID } from 'crypto';
import { catchResult, bind, tap, tapError, mapError, pipe } from './library/result.js';
import { output, outputError } from './library/logging.js';
import { createInMemoryRepository, innerAccountError } from './infrastructure/repository.js';
import { openAccount } from './domain/account.js';
import { createMoney } from './domain/money.js';
import { createWithdrawMoney } from './application/accountApp.js';

const log = (src, msg) => (result) =>
  tap((x) => output(src, msg)(x))(tapError((e) => outputError(src, e))(result));

const logWith = (src, renderText) => (result) =>
  tap((x) => output(src, renderText(x))(x))(tapError((e) => outputError(src, e))(result));

console.log('[js-fp3] Starting withdraw money demo...');

const accountId = randomUUID();

// Try changing this to 250 to see an error being handled
const withdrawalAmount = 100;

pipe(
  catchResult(
    createInMemoryRepository,
    (ex) => ({ type: 'RepositoryCreationFailed', message: ex.message })
  ),
  bind((repo) =>
    pipe(
      createWithdrawMoney(repo),
      bind((withdrawMoney) =>
        pipe(
          openAccount(accountId, createMoney(200)),
          logWith('js-fp3 seed', (account) => `Seeding account ${account.id} with opening balance ${account.balance.amount.toFixed(2)}`),
          bind(repo.saveAccount),
          log('js-fp3 exec', `Executing withdrawal ${withdrawalAmount.toFixed(2)} from account ${accountId}`),
          bind(() => withdrawMoney(accountId, withdrawalAmount)),
          logWith('js-fp3 new balance', (account) => `New balance is ${account.balance.amount.toFixed(2)}`),
          mapError((ae) => innerAccountError(ae))
        )
      )
    )
  ),
  log('js-fp3 done', 'Demo completed.')
);
