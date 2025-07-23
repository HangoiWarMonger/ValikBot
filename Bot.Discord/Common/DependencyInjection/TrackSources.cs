namespace Bot.Discord.Common.DependencyInjection;

/// <summary>
/// Возможные источники треков.
/// </summary>
public enum TrackSources
{
    /// <summary>Источник не определён.</summary>
    Undefined = 0,
    /// <summary>YouTube.</summary>
    Youtube = 1,
    /// <summary>SoundCloud.</summary>
    SoundCLoud = 2
}