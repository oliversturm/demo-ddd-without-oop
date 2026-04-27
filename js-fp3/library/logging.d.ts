export declare const logReturn: <T>(message: string, r: T) => T;

export declare const output: <T>(src: string, message: string) => (x: T) => void;

export declare const outputWith: <T>(
  src: string,
  renderText: (x: T) => string
) => (x: T) => void;

export declare const outputError: <E>(src: string, error: E) => void;

export declare const logSuccess: <T, E>(src: string, message: string) => (
  result: import('./result.js').Result<T, E>
) => import('./result.js').Result<T, E>;

export declare const logSuccessWith: <T, E>(
  src: string,
  renderText: (x: T) => string
) => (result: import('./result.js').Result<T, E>) => import('./result.js').Result<T, E>;

export declare const formatMoney: (money: { amount: number }) => string;

export declare const log: <T, E>(src: string, message: string) => (
  result: import('./result.js').Result<T, E>
) => import('./result.js').Result<T, E>;

export declare const logWith: <T, E>(
  src: string,
  renderText: (x: T) => string
) => (result: import('./result.js').Result<T, E>) => import('./result.js').Result<T, E>;
