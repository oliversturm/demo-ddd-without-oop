import { describe, it, expect } from 'vitest';
import { openAccount, withdraw } from '../js-fp3/domain/account.js';
import { createMoney } from '../js-fp3/domain/money.js';

describe('account domain', () => {
  it('opening an account with a negative balance returns openingBalanceMustBeNonNegative', () => {
    const result = openAccount('id-1', createMoney(-1));

    expect(result.error.type).toBe('OpeningBalanceMustBeNonNegative');
  });

  it('withdrawing a zero amount returns amountMustBePositive', () => {
    const account = openAccount('id-1', createMoney(100)).value;

    const result = withdraw(account, createMoney(0));

    expect(result.error.type).toBe('AmountMustBePositive');
  });

  it('withdrawing a negative amount returns amountMustBePositive', () => {
    const account = openAccount('id-1', createMoney(100)).value;

    const result = withdraw(account, createMoney(-10));

    expect(result.error.type).toBe('AmountMustBePositive');
  });

  it('withdrawing more than the balance returns insufficientBalance', () => {
    const account = openAccount('id-1', createMoney(100)).value;

    const result = withdraw(account, createMoney(101));

    expect(result.error.type).toBe('InsufficientBalance');
  });

  it('insufficientBalance error reports the current balance and attempted amount', () => {
    const account = openAccount('id-1', createMoney(100)).value;

    const error = withdraw(account, createMoney(150)).error;

    expect(error.type).toBe('InsufficientBalance');
    expect(error.balance.amount).toBe(100);
    expect(error.amount.amount).toBe(150);
  });

  it('successive withdrawals are each applied to the running balance', () => {
    const account = openAccount('id-1', createMoney(300)).value;

    const updatedAccount1 = withdraw(account, createMoney(100)).value;
    const updatedAccount2 = withdraw(updatedAccount1, createMoney(100)).value;

    expect(updatedAccount2.balance.amount).toBe(100);
  });
});
