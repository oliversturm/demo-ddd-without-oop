export const ok = (value) => ({
  isSuccess: true,
  isFailure: false,
  value,
});

export const fail = (error) => ({
  isSuccess: false,
  isFailure: true,
  error,
});

export const catchResult = (f, exceptionMapper) => {
  try {
    return ok(f());
  } catch (e) {
    return fail(exceptionMapper(e));
  }
};

export const bind = (binder) => (result) =>
  result.isSuccess ? binder(result.value) : fail(result.error);

export const map = (mapper) => (result) =>
  result.isSuccess ? ok(mapper(result.value)) : fail(result.error);

export const tap = (action) => (result) => {
  if (result.isSuccess) action(result.value);
  return result;
};

export const tapError = (action) => (result) => {
  if (result.isFailure) action(result.error);
  return result;
};

export const mapError = (mapper) => (result) =>
  result.isSuccess ? ok(result.value) : fail(mapper(result.error));

export const match = (onSuccess, onFailure) => (result) =>
  result.isSuccess ? onSuccess(result.value) : onFailure(result.error);

export const switch_ = (onSuccess, onFailure) => (result) => {
  if (result.isSuccess) onSuccess(result.value);
  else onFailure(result.error);
};

export const pipe = (result, ...fns) =>
  fns.reduce((acc, fn) => fn(acc), result);
