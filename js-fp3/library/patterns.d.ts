export type Predicate<T> = (value: T) => boolean;
export type Handler<T, R> = (value: T) => R;
export type Branch<T, R> = [Predicate<T>, Handler<T, R>];

export declare const match: <T, R>(...branches: Branch<T, R>[]) => (value: T) => R;

export declare const when: <T, R>(predicate: Predicate<T>, handler: Handler<T, R>) => Branch<T, R>;

export declare const any: Predicate<any>;
