namespace CsharpFp1.Domain;

/// Conventional DDD base type for aggregates
public abstract class AggregateRoot<TId>
{
    public TId Id { get; protected set; } = default!;
}
