import { tap, tapError } from './result.js';

/**
 * @template T
 * @param {string} message
 * @param {T} r
 * @returns {T}
 */
export const logReturn = (message, r) => {
  console.log(message);
  return r;
};

/**
 * @template T
 * @param {string} src
 * @param {string} message
 * @returns {(x: T) => void}
 */
export const output = (src, message) => (x) => {
  const value = typeof x === 'object' ? JSON.stringify(x) : x;
  console.log(`[${src}] ${message} | ${value}`);
};

/**
 * @template T
 * @param {string} src
 * @param {(x: T) => string} renderText
 * @returns {(x: T) => void}
 */
export const outputWith = (src, renderText) => (x) => {
  console.log(`[${src}] ${renderText(x)}`);
};

/**
 * @template E
 * @param {string} src
 * @param {E} error
 */
export const outputError = (src, error) => {
  const value = typeof error === 'object' ? JSON.stringify(error) : error;
  console.error(`\x1b[1;31m[${src} ERROR]\x1b[0m ${value}`);
};

/**
 * @template T, E
 * @param {string} src
 * @param {string} message
 * @returns {(result: import('./result.js').Result<T, E>) => import('./result.js').Result<T, E>}
 */
export const log = (src, message) => tap(output(src, message));

/**
 * @template T, E
 * @param {string} src
 * @param {(x: T) => string} renderText
 * @returns {(result: import('./result.js').Result<T, E>) => import('./result.js').Result<T, E>}
 */
export const logWith = (src, renderText) => tap(outputWith(src, renderText));
