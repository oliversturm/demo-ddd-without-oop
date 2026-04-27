import { tap, tapError, pipe } from './result.js';
import { formatMoney as formatMoneyFromDomain } from '../domain/money.js';

export { formatMoneyFromDomain as formatMoney };

export const logReturn = (message, r) => {
  console.log(message);
  return r;
};

export const output = (src, message) => (x) => {
  const value = typeof x === 'object' ? JSON.stringify(x) : x;
  console.log(`[${src}] ${message} | ${value}`);
};

export const outputWith = (src, renderText) => (x) => {
  console.log(`[${src}] ${renderText(x)}`);
};

export const outputError = (src, error) => {
  const value = typeof error === 'object' ? JSON.stringify(error) : error;
  console.error(`\x1b[1;31m[${src} ERROR]\x1b[0m ${value}`);
};

export const logSuccess = (src, message) => tap(output(src, message));

export const logSuccessWith = (src, renderText) => tap(outputWith(src, renderText));

export const log = (src, message) => (result) =>
  pipe(result, tap(output(src, message)), tapError((e) => outputError(src, e)));

export const logWith = (src, renderText) => (result) =>
  pipe(result, tap((x) => output(src, renderText(x))(x)), tapError((e) => outputError(src, e)));
