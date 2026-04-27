export const match = (...branches) => (value) => {
  for (const [predicate, handler] of branches) {
    if (predicate(value)) return handler(value);
  }
  throw new Error('No matching pattern');
};

export const when = (predicate, handler) => [predicate, handler];

export const any = () => true;
