import { ok, fail } from '../library/result.js';
import { logReturn, formatMoney } from '../library/logging.js';
import { accountNotFound } from '../domain/account.js';

export const repositoryCreationFailed = (message) => ({
  type: 'RepositoryCreationFailed',
  message,
});

export const innerAccountError = (innerError) => ({
  type: 'InnerAccountError',
  innerError,
});

export const createInMemoryRepository = () => {
  const store = new Map();

  const getById = (id) =>
    store.has(id)
      ? logReturn(
          `[Repo] Loaded account ${id}`,
          ok(store.get(id))
        )
      : logReturn(
          `[Repo] Account ${id} not found`,
          fail(accountNotFound(id))
        );

  const save = (account) => {
    store.set(account.id, account);
    return logReturn(
      `[Repo] Saved account ${account.id} with balance ${formatMoney(account.balance)}`,
      ok(account)
    );
  };

  return { loadAccount: getById, saveAccount: save };
};
