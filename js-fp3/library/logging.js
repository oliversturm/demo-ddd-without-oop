import { tap, tapError } from './result.js';

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

export const log = (src, message) => tap(output(src, message));

export const logWith = (src, renderText) => tap(outputWith(src, renderText));
