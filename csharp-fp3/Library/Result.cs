namespace CsharpFp3.Library;

public readonly record struct Result<T, E>
{
    private readonly T _value;

    private readonly E _error;

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T Value =>
        IsSuccess
            ? _value
            : throw new InvalidOperationException("No value present for failed result.");

    public E Error =>
        IsFailure
            ? _error
            : throw new InvalidOperationException("No error present for successful result.");

    private Result(T value)
    {
        _value = value;
        _error = default!;
        IsSuccess = true;
    }

    private Result(E error)
    {
        _value = default!;
        _error = error;
        IsSuccess = false;
    }

    public static Result<T, E> Ok(T value) => new(value);

    public static Result<T, E> Fail(E error) => new(error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<E, TResult> onFailure) =>
        IsSuccess ? onSuccess(_value) : onFailure(_error);

    public void Switch(Action<T> onSuccess, Action<E> onFailure)
    {
        if (IsSuccess)
            onSuccess(_value);
        else
            onFailure(_error);
    }

    public static Result<T, E> Catch(Func<T> f, Func<Exception, E> exceptionMapper)
    {
        try
        {
            var result = f();
            return Result<T, E>.Ok(result);
        }
        catch (Exception e)
        {
            return Result<T, E>.Fail(exceptionMapper(e));
        }
    }
}

public static class ResultExtensions
{
    public static Result<TOut, E> Bind<TIn, TOut, E>(
        this Result<TIn, E> result,
        Func<TIn, Result<TOut, E>> binder
    ) => result.IsSuccess ? binder(result.Value) : Result<TOut, E>.Fail(result.Error);

    public static Result<TOut, E> Map<TIn, TOut, E>(
        this Result<TIn, E> result,
        Func<TIn, TOut> mapper
    ) =>
        result.IsSuccess
            ? Result<TOut, E>.Ok(mapper(result.Value))
            : Result<TOut, E>.Fail(result.Error);

    public static Result<T, E> Tap<T, E>(this Result<T, E> result, Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value);

        return result;
    }

    public static Result<T, E> TapError<T, E>(this Result<T, E> result, Action<E> action)
    {
        if (result.IsFailure)
            action(result.Error);

        return result;
    }

    public static Result<T, EOut> MapError<T, EIn, EOut>(
        this Result<T, EIn> result,
        Func<EIn, EOut> map
    ) =>
        result.IsSuccess
            ? Result<T, EOut>.Ok(result.Value)
            : Result<T, EOut>.Fail(map(result.Error));

    public static Result<T, E> Log<T, E>(this Result<T, E> result, string src, string msg) =>
        Tap(result, Logging.Output<T>(src, msg)).TapError(m => Logging.OutputError(src, m));

    public static Result<T, E> Log<T, E>(
        this Result<T, E> result,
        string src,
        Func<T, string> renderText
    ) =>
        Tap(result, x => Logging.Output<T>(src, renderText(x))(x))
            .TapError(m => Logging.OutputError(src, m));

    public static Result<TOut, E> Select<TIn, TOut, E>(
        this Result<TIn, E> result,
        Func<TIn, TOut> mapper
    ) => result.Map(mapper);

    public static Result<TOut, E> SelectMany<TIn, TIntermediate, TOut, E>(
        this Result<TIn, E> result,
        Func<TIn, Result<TIntermediate, E>> binder,
        Func<TIn, TIntermediate, TOut> projector
    ) => result.Bind(x => binder(x).Map(y => projector(x, y)));
}
