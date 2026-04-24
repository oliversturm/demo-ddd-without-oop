import { Result } from '../library/result.js';
import { Account, AccountError } from '../domain/account.js';

export type Repository = {
  loadAccount: (id: string) => Result<Account, AccountError>;
  saveAccount: (account: Account) => Result<Account, AccountError>;
};

export type AppErrorRepositoryCreationFailed = {
  type: 'RepositoryCreationFailed';
  message: string;
};

export type AppErrorInnerAccountError = {
  type: 'InnerAccountError';
  innerError: AccountError;
};

export type AppError =
  | AppErrorRepositoryCreationFailed
  | AppErrorInnerAccountError;

export declare const repositoryCreationFailed: (
  message: string
) => AppErrorRepositoryCreationFailed;

export declare const innerAccountError: (
  innerError: AccountError
) => AppErrorInnerAccountError;

export declare const createInMemoryRepository: () => Repository;
