using Bot.Domain.Exceptions;
using Bot.Domain.ValueObjects;
using FluentValidation;

namespace Bot.Domain.Validation;

/// <summary>
/// Валидатор ссылки на трек.
/// </summary>
public class TrackLinkValidator : AbstractValidator<TrackLink>
{
    /// <summary>
    /// Валидатор ссылки на трек.
    /// </summary>
    public TrackLinkValidator()
    {
        RuleFor(x => x.Url)
            .NotNull()
            .NotEmpty()
            .Must(x => Uri.IsWellFormedUriString(x, UriKind.Absolute))
            .WithMessage(ExceptionMessage.InvalidUrl());
    }
}