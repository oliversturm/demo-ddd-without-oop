import { Result } from '../library/result.js';
import { Repository, AppError } from '../infrastructure/repository.js';
import { Account, AccountError } from '../domain/account.js';

export type AccountResult = Result<Account, AccountError>;

export type WithdrawMoneyFn = (accountId: string, amount: number) => AccountResult;

export type CreateWithdrawMoneyResult = Result<WithdrawMoneyFn, AppError>;

export declare const createWithdrawMoney: (repo: Repository) => CreateWithdrawMoneyResult;
