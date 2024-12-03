using System.Text.RegularExpressions;
using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Types;
using Bot.Domain.Validation;

namespace Bot.Application.Common.Services;

public partial class TrackSourceResolver : ITrackSourceResolver
{
    public TrackSource GetTrackSource(string url)
    {
        Guard.Against.NullOrWhiteSpace(url, nameof(url));

        if (YoutubeVideoUrlRegex().IsMatch(url))
        {
            return TrackSource.Youtube;
        }

        return TrackSource.Undefined;
    }

    [GeneratedRegex(ValidationConstant.YouTubeVideoUrlRegex)]
    private static partial Regex YoutubeVideoUrlRegex();
}