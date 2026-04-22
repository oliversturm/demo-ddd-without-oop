namespace CsharpFp1.Domain;

public sealed record Money(decimal Amount)
{
    public Money Add(Money other) => new(Amount + other.Amount);

    public Money Subtract(Money other) => new(Amount - other.Amount);
}
