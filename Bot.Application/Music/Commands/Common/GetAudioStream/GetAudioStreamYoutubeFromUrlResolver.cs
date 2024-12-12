using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Types;
using MediatR;

namespace Bot.Application.Music.Commands.Common.GetAudioStream;

public class GetAudioStreamYoutubeFromUrlResolver : IRequestHandler<GetAudioStreamRequest, Stream>
{
    private readonly IFactory<ITrackClient, TrackSource> _trackClientFactory;
    private readonly ITrackSourceResolver _trackSourceResolver;

    public GetAudioStreamYoutubeFromUrlResolver(ITrackSourceResolver trackSourceResolver, IFactory<ITrackClient, TrackSource> trackClientFactory)
    {
        _trackSourceResolver = trackSourceResolver;
        _trackClientFactory = trackClientFactory;
    }

    public Task<Stream> Handle(GetAudioStreamRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NullOrWhiteSpace(request.Url, nameof(request.Url));

        var source = _trackSourceResolver.GetTrackSource(request.Url);

        ITrackClient trackClient = _trackClientFactory.Get(source);

        return trackClient.GetAudioStreamAsync(request.Url, cancellationToken);
    }
}