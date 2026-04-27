import { describe, it, expect, vi } from 'vitest';
import { match, when, any } from '../js-fp3/library/patterns.js';

describe('when', () => {
  it('returns a two-element tuple of [predicate, handler]', () => {
    const predicate = () => true;
    const handler = () => 'result';

    const branch = when(predicate, handler);

    expect(branch).toEqual([predicate, handler]);
  });
});

describe('any', () => {
  it('returns true for any value', () => {
    expect(any(0)).toBe(true);
    expect(any(null)).toBe(true);
    expect(any('string')).toBe(true);
    expect(any({ x: 1 })).toBe(true);
  });
});

describe('match', () => {
  it('executes the handler of the first matching branch', () => {
    const result = match(
      when((v) => v === 1, () => 'one'),
      when((v) => v === 2, () => 'two'),
      when(any, () => 'other')
    )(1);

    expect(result).toBe('one');
  });

  it('falls through to the next branch when the first does not match', () => {
    const result = match(
      when((v) => v === 1, () => 'one'),
      when((v) => v === 2, () => 'two'),
      when(any, () => 'other')
    )(2);

    expect(result).toBe('two');
  });

  it('uses the any catch-all when no earlier branch matches', () => {
    const result = match(
      when((v) => v === 1, () => 'one'),
      when(any, () => 'catch-all')
    )(99);

    expect(result).toBe('catch-all');
  });

  it('passes the value to the handler', () => {
    const result = match(
      when(any, (v) => v * 2)
    )(7);

    expect(result).toBe(14);
  });

  it('throws when no branch matches', () => {
    expect(() =>
      match(
        when((v) => v === 1, () => 'one')
      )(99)
    ).toThrow('No matching pattern');
  });

  it('does not call handlers for branches that were not selected', () => {
    const skipped = vi.fn(() => 'skipped');
    const selected = vi.fn(() => 'selected');

    match(
      when((v) => v === 1, selected),
      when(any, skipped)
    )(1);

    expect(selected).toHaveBeenCalledOnce();
    expect(skipped).not.toHaveBeenCalled();
  });
});
