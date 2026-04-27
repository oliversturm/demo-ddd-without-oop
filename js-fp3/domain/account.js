import { ok, fail } from '../library/result.js';
import { match, when, any } from '../library/patterns.js';

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

export const withdraw = (account, amount) =>
  match(
    when(({ amount: a }) => a.amount <= 0, () => fail(amountMustBePositive)),
    when(
      ({ account: acc, amount: amt }) => acc.balance.amount < amt.amount,
      ({ account: acc, amount: amt }) => fail(insufficientBalance(acc.balance, amt))
    ),
    when(any, ({ account: acc, amount: amt }) =>
      ok({
        ...acc,
        balance: { amount: acc.balance.amount - amt.amount },
      })
    )
  )({ account, amount });
