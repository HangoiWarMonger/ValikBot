using Bot.Application.Common.Types;

namespace Bot.Application.Common.Interfaces;

public interface ITrackSourceResolver
{
    TrackSource GetTrackSource(string url);
}