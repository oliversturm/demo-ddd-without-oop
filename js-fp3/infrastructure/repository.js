import { ok, fail } from '../library/result.js';
import { logReturn } from '../library/logging.js';
import { accountNotFound } from '../domain/account.js';

/**
 * @typedef {Object} Repository
 * @property {(id: string) => import('../library/result.js').Result<import('../domain/account.js').Account, import('../domain/account.js').AccountError>} loadAccount
 * @property {(account: import('../domain/account.js').Account) => import('../library/result.js').Result<import('../domain/account.js').Account, import('../domain/account.js').AccountError>} saveAccount
 */

/** @typedef {{ type: 'RepositoryCreationFailed', message: string }} AppErrorRepositoryCreationFailed */
/** @typedef {{ type: 'InnerAccountError', innerError: import('../domain/account.js').AccountError }} AppErrorInnerAccountError */

/** @typedef {AppErrorRepositoryCreationFailed | AppErrorInnerAccountError} AppError */

/**
 * @param {string} message
 * @returns {AppErrorRepositoryCreationFailed}
 */
export const repositoryCreationFailed = (message) => ({
  type: 'RepositoryCreationFailed',
  message,
});

/**
 * @param {import('../domain/account.js').AccountError} innerError
 * @returns {AppErrorInnerAccountError}
 */
export const innerAccountError = (innerError) => ({
  type: 'InnerAccountError',
  innerError,
});

/**
 * @returns {Repository}
 */
export const createInMemoryRepository = () => {
  /** @type {Map<string, import('../domain/account.js').Account>} */
  const store = new Map();

  /**
   * @param {string} id
   * @returns {import('../library/result.js').Result<import('../domain/account.js').Account, import('../domain/account.js').AccountError>}
   */
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

  /**
   * @param {import('../domain/account.js').Account} account
   * @returns {import('../library/result.js').Result<import('../domain/account.js').Account, import('../domain/account.js').AccountError>}
   */
  const save = (account) => {
    store.set(account.id, account);
    return logReturn(
      `[Repo] Saved account ${account.id} with balance ${account.balance.amount.toFixed(2)}`,
      ok(account)
    );
  };

  return { loadAccount: getById, saveAccount: save };
};
