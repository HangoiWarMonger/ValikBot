namespace Bot.Domain.ValueObjects;

/// <summary>
/// Базовый класс ValueObject.
/// </summary>
public abstract class BaseValueObject<T> : IEquatable<T> where T : BaseValueObject<T>
{
    /// <summary>
    /// Набор элементов, по которым будет осуществляться сравнение.
    /// </summary>
    protected abstract IEnumerable<object> GetEqualityComponents();

    /// <summary>
    /// Получение Hash Code.
    /// </summary>
    public override int GetHashCode()
    {
        return GetEqualityComponents().Aggregate(0, (current, obj) => HashCode.Combine(current, obj.GetHashCode()));
    }

    /// <summary>
    /// Сравнение объекта с другим объектом.
    /// </summary>
    /// <param name="obj">Объект, с которым производится сравнение.</param>
    public override bool Equals(object? obj)
    {
        return obj is T other && Equals(other);
    }

    /// <summary>
    /// Сравнение объекта с другим объектом этого же типа.
    /// </summary>
    /// <param name="other">Объект, с которым производится сравнение.</param>
    public virtual bool Equals(T? other)
    {
        return other is not null && other.GetEqualityComponents().SequenceEqual(GetEqualityComponents());
    }
}
