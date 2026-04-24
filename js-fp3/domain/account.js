import { ok, fail } from '../library/result.js';

export const openingBalanceMustBeNonNegative = { type: 'OpeningBalanceMustBeNonNegative' };

export const amountMustBePositive = { type: 'AmountMustBePositive' };

export const insufficientBalance = (balance, amount) => ({
  type: 'InsufficientBalance',
  balance,
  amount,
});

export const accountNotFound = (accountId) => ({
  type: 'AccountNotFound',
  accountId,
});

export const openAccount = (id, openingBalance) =>
  openingBalance.amount < 0
    ? fail(openingBalanceMustBeNonNegative)
    : ok({ id, balance: openingBalance });

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
