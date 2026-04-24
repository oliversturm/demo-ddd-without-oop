/**
 * @template T, E
 * @typedef {Object} Result
 * @property {boolean} isSuccess
 * @property {boolean} isFailure
 * @property {T} [value] - Present when isSuccess is true
 * @property {E} [error] - Present when isFailure is true
 */

/**
 * @template T, E
 * @param {T} value
 * @returns {Result<T, E>}
 */
export const ok = (value) => ({
  isSuccess: true,
  isFailure: false,
  value,
});

/**
 * @template T, E
 * @param {E} error
 * @returns {Result<T, E>}
 */
export const fail = (error) => ({
  isSuccess: false,
  isFailure: true,
  error,
});

/**
 * @template T, E
 * @param {() => T} f
 * @param {(err: Error) => E} exceptionMapper
 * @returns {Result<T, E>}
 */
export const catchResult = (f, exceptionMapper) => {
  try {
    return ok(f());
  } catch (e) {
    return fail(exceptionMapper(e));
  }
};

/**
 * @template TIn, TOut, E
 * @param {(value: TIn) => Result<TOut, E>} binder
 * @returns {(result: Result<TIn, E>) => Result<TOut, E>}
 */
export const bind = (binder) => (result) =>
  result.isSuccess ? binder(result.value) : fail(result.error);

/**
 * @template TIn, TOut, E
 * @param {(value: TIn) => TOut} mapper
 * @returns {(result: Result<TIn, E>) => Result<TOut, E>}
 */
export const map = (mapper) => (result) =>
  result.isSuccess ? ok(mapper(result.value)) : fail(result.error);

/**
 * @template T, E
 * @param {(value: T) => void} action
 * @returns {(result: Result<T, E>) => Result<T, E>}
 */
export const tap = (action) => (result) => {
  if (result.isSuccess) action(result.value);
  return result;
};

/**
 * @template T, E
 * @param {(error: E) => void} action
 * @returns {(result: Result<T, E>) => Result<T, E>}
 */
export const tapError = (action) => (result) => {
  if (result.isFailure) action(result.error);
  return result;
};

/**
 * @template T, EIn, EOut
 * @param {(error: EIn) => EOut} mapper
 * @returns {(result: Result<T, EIn>) => Result<T, EOut>}
 */
export const mapError = (mapper) => (result) =>
  result.isSuccess ? ok(result.value) : fail(mapper(result.error));

/**
 * @template T, E, TResult
 * @param {(value: T) => TResult} onSuccess
 * @param {(error: E) => TResult} onFailure
 * @returns {(result: Result<T, E>) => TResult}
 */
export const match = (onSuccess, onFailure) => (result) =>
  result.isSuccess ? onSuccess(result.value) : onFailure(result.error);

/**
 * @template T, E
 * @param {(value: T) => void} onSuccess
 * @param {(error: E) => void} onFailure
 * @returns {(result: Result<T, E>) => void}
 */
export const switch_ = (onSuccess, onFailure) => (result) => {
  if (result.isSuccess) onSuccess(result.value);
  else onFailure(result.error);
};

/**
 * Pipe a result through a series of functions (left-to-right composition).
 * This allows chaining without extending the prototype.
 * @template T, E
 * @param {Result<T, E>} result
 * @param {...Function} fns
 * @returns {Result<any, any>}
 */
export const pipe = (result, ...fns) =>
  fns.reduce((acc, fn) => fn(acc), result);
