using System.Text.RegularExpressions;
using Bot.Domain.Exceptions;
using Bot.Domain.ValueObjects;
using FluentValidation;

namespace Bot.Domain.Validation;

/// <summary>
/// Валидатор ссылки на трек.
/// </summary>
public partial class TrackLinkValidator : AbstractValidator<TrackLink>
{
    /// <summary>
    /// Валидатор ссылки на трек.
    /// </summary>
    public TrackLinkValidator()
    {
        RuleFor(x => x.Url)
            .NotNull()
            .NotEmpty()
            .Matches(UrlRegex())
            .WithMessage(ExceptionMessage.InvalidUrl());
    }

    [GeneratedRegex($"{ValidationConstant.YouTubeVideoUrlRegex}")]
    private static partial Regex UrlRegex();
}