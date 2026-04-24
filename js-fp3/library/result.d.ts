export type Success<T> = {
  isSuccess: true;
  isFailure: false;
  value: T;
};

export type Failure<E> = {
  isSuccess: false;
  isFailure: true;
  error: E;
};

export type Result<T, E> = Success<T> | Failure<E>;

export declare const ok: <T, E>(value: T) => Result<T, E>;

export declare const fail: <T, E>(error: E) => Result<T, E>;

export declare const catchResult: <T, E>(
  f: () => T,
  exceptionMapper: (err: Error) => E
) => Result<T, E>;

export declare const bind: <TIn, TOut, E>(
  binder: (value: TIn) => Result<TOut, E>
) => (result: Result<TIn, E>) => Result<TOut, E>;

export declare const map: <TIn, TOut, E>(
  mapper: (value: TIn) => TOut
) => (result: Result<TIn, E>) => Result<TOut, E>;

export declare const tap: <T, E>(
  action: (value: T) => void
) => (result: Result<T, E>) => Result<T, E>;

export declare const tapError: <T, E>(
  action: (error: E) => void
) => (result: Result<T, E>) => Result<T, E>;

export declare const mapError: <T, EIn, EOut>(
  mapper: (error: EIn) => EOut
) => (result: Result<T, EIn>) => Result<T, EOut>;

export declare const match: <T, E, TResult>(
  onSuccess: (value: T) => TResult,
  onFailure: (error: E) => TResult
) => (result: Result<T, E>) => TResult;

export declare const switch_: <T, E>(
  onSuccess: (value: T) => void,
  onFailure: (error: E) => void
) => (result: Result<T, E>) => void;

export declare const pipe: <T, E>(
  result: Result<T, E>,
  ...fns: Array<(arg: Result<any, any>) => Result<any, any>>
) => Result<any, any>;
