using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Types;
using Bot.Application.Music.Commands.YouTube.GetAudioStream;
using MediatR;

namespace Bot.Application.Music.Commands.Common.GetAudioStream;

public class GetAudioStreamYoutubeFromUrlResolver : IRequestHandler<GetAudioStreamRequest, Stream>
{
    private readonly ITrackSourceResolver _trackSourceResolver;
    private readonly ISender _sender;

    public GetAudioStreamYoutubeFromUrlResolver(ITrackSourceResolver trackSourceResolver, ISender sender)
    {
        _trackSourceResolver = trackSourceResolver;
        _sender = sender;
    }

    public Task<Stream> Handle(GetAudioStreamRequest request, CancellationToken cancellationToken)
    {
        var source = _trackSourceResolver.GetTrackSource(request.Url);

        var command = source switch
        {
            TrackSource.Youtube => new GetAudioStreamYoutubeFromUrlRequest(request.Url),
            TrackSource.Undefined or _ => throw new InvalidDataException(),
        };

        return _sender.Send(command, cancellationToken);
    }
}