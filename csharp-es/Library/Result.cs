namespace CsharpEs.Library;

// disable match exhaustion warnings - C# thinks we could have other
// derived instances of Result<T,E> but there's no way to say
// "don't allow creation of further derived types" while leaving
// the type publicly visible.
#pragma warning disable CS8509

public abstract record Result<T, E>
{
    public static implicit operator Result<T, E>(T value) => new ResultOk<T, E>(value);

    public static implicit operator Result<T, E>(E error) => new ResultFail<T, E>(error);
}

public sealed record ResultOk<T, E>(T Value) : Result<T, E>;

public sealed record ResultFail<T, E>(E Error) : Result<T, E>;

public static class ResultModule
{
    public static T Ok<T>(T v) => v;

    public static Result<T?, E> OkNone<T, E>()
        where T : class? => new ResultOk<T?, E>(null);

    public static E Fail<E>(E e) => e;
}

public static class ResultExtensions
{
    extension<TIn, E>(Result<TIn, E> result)
    {
        public bool IsSuccess => result is ResultOk<TIn, E>;
        public bool IsFailure => !result.IsSuccess;

        public Result<TOut, E> Bind<TOut>(Func<TIn, Result<TOut, E>> binder) =>
            result switch
            {
                ResultOk<TIn, E>(var v) => binder(v),
                ResultFail<TIn, E>(var e) => new ResultFail<TOut, E>(e),
            };

        public Result<TOut, E> Map<TOut>(Func<TIn, TOut> mapper) =>
            result switch
            {
                ResultOk<TIn, E>(var v) => new ResultOk<TOut, E>(mapper(v)),
                ResultFail<TIn, E>(var e) => new ResultFail<TOut, E>(e),
            };

        public Result<TIn, EOut> MapError<EOut>(Func<E, EOut> mapper) =>
            result switch
            {
                ResultOk<TIn, E>(var v) => new ResultOk<TIn, EOut>(v),
                ResultFail<TIn, E>(var e) => new ResultFail<TIn, EOut>(mapper(e)),
            };

        public Result<TIn, E> Tap(Action<TIn> action)
        {
            if (result is ResultOk<TIn, E>(var v))
                action(v);
            return result;
        }

        public Result<TIn, E> TapError(Action<E> action)
        {
            if (result is ResultFail<TIn, E>(var e))
                action(e);
            return result;
        }

        public Result<TIn, E> Log(string src, string msg) =>
            Tap(result, Logging.OutputDelegate<TIn>(src, msg))
                .TapError(m => Logging.OutputError(src, m));

        public Result<TIn, E> Log(string src, Func<TIn, string> renderText) =>
            Tap(result, x => Logging.OutputDelegate<TIn>(src, renderText(x))(x))
                .TapError(m => Logging.OutputError(src, m));

        public Result<TOut, E> Select<TOut>(Func<TIn, TOut> mapper) => Map(result, mapper);

        public Result<TOut, E> SelectMany<TIntermediate, TOut>(
            Func<TIn, Result<TIntermediate, E>> binder,
            Func<TIn, TIntermediate, TOut> projector
        ) => Bind(result, x => binder(x).Map(y => projector(x, y)));

        public TResult Match<TResult>(Func<TIn, TResult> onSuccess, Func<E, TResult> onFailure) =>
            result switch
            {
                ResultOk<TIn, E>(var v) => onSuccess(v),
                ResultFail<TIn, E>(var e) => onFailure(e),
            };

        public void Switch(Action<TIn> onSuccess, Action<E> onFailure)
        {
            switch (result)
            {
                case ResultOk<TIn, E>(var v):
                    onSuccess(v);
                    break;
                case ResultFail<TIn, E>(var e):
                    onFailure(e);
                    break;
            }
        }
    }

    extension<TIn, E>(Result<TIn, E>)
    {
        public static Result<TIn, E> Catch(Func<TIn> f, Func<Exception, E> exceptionMapper)
        {
            try
            {
                return new ResultOk<TIn, E>(f());
            }
            catch (Exception e)
            {
                return new ResultFail<TIn, E>(exceptionMapper(e));
            }
        }

        public static Result<TIn, E> Catch(
            Func<Result<TIn, E>> f,
            Func<Exception, E> exceptionMapper
        )
        {
            try
            {
                return f();
            }
            catch (Exception e)
            {
                return new ResultFail<TIn, E>(exceptionMapper(e));
            }
        }
    }
}
