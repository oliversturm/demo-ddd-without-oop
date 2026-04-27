import { describe, it, expect, vi } from 'vitest';
import {
  ok,
  fail,
  catchResult,
  bind,
  map,
  tap,
  tapError,
  mapError,
  match,
  switch_,
  pipe,
} from '../js-fp3/library/result.js';

describe('ok / fail construction', () => {
  it('ok is success', () => {
    const result = ok(42);

    expect(result.isSuccess).toBe(true);
    expect(result.isFailure).toBe(false);
  });

  it('fail is failure', () => {
    const result = fail('oops');

    expect(result.isFailure).toBe(true);
    expect(result.isSuccess).toBe(false);
  });

  it('ok exposes value', () => {
    const result = ok(42);

    expect(result.value).toBe(42);
  });

  it('fail exposes error', () => {
    const result = fail('oops');

    expect(result.error).toBe('oops');
  });
});

describe('match', () => {
  it('calls onSuccess for ok', () => {
    const result = ok(10);

    const output = match((v) => `value:${v}`, (e) => `error:${e}`)(result);

    expect(output).toBe('value:10');
  });

  it('calls onFailure for fail', () => {
    const result = fail('bad');

    const output = match((v) => `value:${v}`, (e) => `error:${e}`)(result);

    expect(output).toBe('error:bad');
  });
});

describe('switch_', () => {
  it('calls onSuccess for ok', () => {
    const result = ok(7);
    let called = false;

    switch_(() => { called = true; }, () => {})(result);

    expect(called).toBe(true);
  });

  it('calls onFailure for fail', () => {
    const result = fail('err');
    let called = false;

    switch_(() => {}, () => { called = true; })(result);

    expect(called).toBe(true);
  });
});

describe('catchResult', () => {
  it('returns ok when no exception is thrown', () => {
    const result = catchResult(() => 99, (ex) => ex.message);

    expect(result.isSuccess).toBe(true);
    expect(result.value).toBe(99);
  });

  it('returns fail when an exception is thrown', () => {
    const result = catchResult(
      () => { throw new Error('boom'); },
      (ex) => ex.message
    );

    expect(result.isFailure).toBe(true);
    expect(result.error).toBe('boom');
  });
});

describe('bind', () => {
  it('chains to the next result on success', () => {
    const result = bind((v) => ok(`got ${v}`))(ok(5));

    expect(result.value).toBe('got 5');
  });

  it('short-circuits on failure without calling the binder', () => {
    const binder = vi.fn((v) => ok(`got ${v}`));
    const result = bind(binder)(fail('nope'));

    expect(binder).not.toHaveBeenCalled();
    expect(result.error).toBe('nope');
  });

  it('propagates the inner failure when the binder returns fail', () => {
    const result = bind(() => fail('inner fail'))(ok(5));

    expect(result.error).toBe('inner fail');
  });
});

describe('map', () => {
  it('transforms the value on success', () => {
    const result = map((v) => v * 2)(ok(3));

    expect(result.value).toBe(6);
  });

  it('does not call the mapper on failure', () => {
    const mapper = vi.fn((v) => v * 2);
    const result = map(mapper)(fail('err'));

    expect(mapper).not.toHaveBeenCalled();
    expect(result.error).toBe('err');
  });
});

describe('tap', () => {
  it('calls the action and returns the original result on success', () => {
    let seen = -1;
    const result = tap((v) => { seen = v; })(ok(8));

    expect(seen).toBe(8);
    expect(result.value).toBe(8);
  });

  it('does not call the action on failure', () => {
    const action = vi.fn();
    tap(action)(fail('err'));

    expect(action).not.toHaveBeenCalled();
  });
});

describe('tapError', () => {
  it('calls the action and returns the original result on failure', () => {
    let seen = '';
    const result = tapError((e) => { seen = e; })(fail('bad'));

    expect(seen).toBe('bad');
    expect(result.error).toBe('bad');
  });

  it('does not call the action on success', () => {
    const action = vi.fn();
    tapError(action)(ok(1));

    expect(action).not.toHaveBeenCalled();
  });
});

describe('mapError', () => {
  it('transforms the error on failure', () => {
    const result = mapError((e) => e.length)(fail('oops'));

    expect(result.error).toBe(4);
  });

  it('does not call the mapper on success', () => {
    const mapper = vi.fn((e) => e.length);
    const result = mapError(mapper)(ok(1));

    expect(mapper).not.toHaveBeenCalled();
    expect(result.value).toBe(1);
  });
});

describe('pipe', () => {
  it('applies functions left to right starting from the initial result', () => {
    const result = pipe(ok(2), map((v) => v + 1), map((v) => v * 3));

    expect(result.value).toBe(9);
  });

  it('short-circuits on the first failure', () => {
    const mapper = vi.fn((v) => v * 2);
    const result = pipe(fail('stop'), map(mapper));

    expect(mapper).not.toHaveBeenCalled();
    expect(result.error).toBe('stop');
  });
});
