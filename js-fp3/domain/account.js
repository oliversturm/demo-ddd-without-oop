import { ok, fail } from '../library/result.js';

/**
 * @typedef {Object} Account
 * @property {string} id
 * @property {import('./money.js').Money} balance
 */

/** @typedef {{ type: 'OpeningBalanceMustBeNonNegative' }} OpeningBalanceMustBeNonNegative */
/** @typedef {{ type: 'AmountMustBePositive' }} AmountMustBePositive */
/** @typedef {{ type: 'InsufficientBalance', balance: import('./money.js').Money, amount: import('./money.js').Money }} InsufficientBalance */
/** @typedef {{ type: 'AccountNotFound', accountId: string }} AccountNotFound */

/** @typedef {OpeningBalanceMustBeNonNegative | AmountMustBePositive | InsufficientBalance | AccountNotFound} AccountError */

/**
 * @type {OpeningBalanceMustBeNonNegative}
 */
export const openingBalanceMustBeNonNegative = { type: 'OpeningBalanceMustBeNonNegative' };

/**
 * @type {AmountMustBePositive}
 */
export const amountMustBePositive = { type: 'AmountMustBePositive' };

/**
 * @param {import('./money.js').Money} balance
 * @param {import('./money.js').Money} amount
 * @returns {InsufficientBalance}
 */
export const insufficientBalance = (balance, amount) => ({
  type: 'InsufficientBalance',
  balance,
  amount,
});

/**
 * @param {string} accountId
 * @returns {AccountNotFound}
 */
export const accountNotFound = (accountId) => ({
  type: 'AccountNotFound',
  accountId,
});

/**
 * @param {string} id
 * @param {import('./money.js').Money} openingBalance
 * @returns {import('../library/result.js').Result<Account, AccountError>}
 */
export const openAccount = (id, openingBalance) =>
  openingBalance.amount < 0
    ? fail(openingBalanceMustBeNonNegative)
    : ok({ id, balance: openingBalance });

/**
 * @param {Account} account
 * @param {import('./money.js').Money} amount
 * @returns {import('../library/result.js').Result<Account, AccountError>}
 */
export const withdraw = (account, amount) => {
  if (amount.amount <= 0) {
    return fail(amountMustBePositive);
  }
  if (account.balance.amount < amount.amount) {
    return fail(insufficientBalance(account.balance, amount));
  }
  return ok({
    ...account,
    balance: { amount: account.balance.amount - amount.amount },
  });
};
