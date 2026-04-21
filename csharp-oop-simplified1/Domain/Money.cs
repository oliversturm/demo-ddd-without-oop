namespace CsharpOopSimplified1.Domain;

/// Value object used to represent money and enforce simple invariants.
/// Note that this implementation uses immutable patterns for the data
/// by returning a new instance for each modification. This is an early
/// recommendation for DDD with OO, but not necessarily the common practice
/// in many real-world implementations.
public sealed class Money
{
    // Potentially with a setter - see note above
    public decimal Amount { get; }

    public Money(decimal amount)
    {
        Amount = amount;
    }

    // In many existing DDD/OO codebases you may actually see the use
    // of mutable value types.
    //
    // public void Add(Money other)
    // {
    //     this.Amount += other.Amount;
    // }

    // On the other hand, sometimes these helpers may be left out
    // and operations encoded directly "from the outside":
    // newBalance = new Money(oldBalance.Amount - charge.Amount)
    //
    public Money Add(Money other) => new(Amount + other.Amount);

    public Money Subtract(Money other) => new(Amount - other.Amount);
}
