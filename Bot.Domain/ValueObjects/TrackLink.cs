using Bot.Domain.Validation;
using FluentValidation;

namespace Bot.Domain.ValueObjects;

/// <summary>
/// Ссылка на музыкальный трек.
/// </summary>
public class TrackLink : BaseValueObject<TrackLink>
{
    /// <summary>
    /// Ссылка.
    /// </summary>
    public string Url { get; init; }

    /// <summary>
    /// Ссылка на музыкальный трек.
    /// </summary>
    internal TrackLink(string url)
    {
        Url = url;

        new TrackLinkValidator().ValidateAndThrow(this);
    }

    /// <summary>
    /// Набор элементов, по которым будет осуществляться сравнение.
    /// </summary>
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Url;
    }
}