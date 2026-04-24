import { Result } from '../library/result.js';
import { Money } from './money.js';

export type Account = {
  id: string;
  balance: Money;
};

export type OpeningBalanceMustBeNonNegative = {
  type: 'OpeningBalanceMustBeNonNegative';
};

export type AmountMustBePositive = {
  type: 'AmountMustBePositive';
};

export type InsufficientBalance = {
  type: 'InsufficientBalance';
  balance: Money;
  amount: Money;
};

export type AccountNotFound = {
  type: 'AccountNotFound';
  accountId: string;
};

export type AccountError =
  | OpeningBalanceMustBeNonNegative
  | AmountMustBePositive
  | InsufficientBalance
  | AccountNotFound;

export declare const openingBalanceMustBeNonNegative: OpeningBalanceMustBeNonNegative;

export declare const amountMustBePositive: AmountMustBePositive;

export declare const insufficientBalance: (
  balance: Money,
  amount: Money
) => InsufficientBalance;

export declare const accountNotFound: (accountId: string) => AccountNotFound;

export declare const openAccount: (
  id: string,
  openingBalance: Money
) => Result<Account, AccountError>;

export declare const withdraw: (
  account: Account,
  amount: Money
) => Result<Account, AccountError>;
