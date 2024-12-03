using Bot.Application.Common.Dto;
using Bot.Application.Common.Interfaces;
using Bot.Application.Common.Types;
using Bot.Application.Music.Commands.YouTube.GetTrackInfoRequest;
using MediatR;

namespace Bot.Application.Music.Commands.Common.GetTrackInfo;

public class GetTrackInfoRequestResolver : IRequestHandler<GetTrackInfoRequest, TrackInfoDto>
{
    private readonly ITrackSourceResolver _trackSourceResolver;
    private readonly ISender _sender;

    public GetTrackInfoRequestResolver(ITrackSourceResolver trackSourceResolver, ISender sender)
    {
        _trackSourceResolver = trackSourceResolver;
        _sender = sender;
    }

    public Task<TrackInfoDto> Handle(GetTrackInfoRequest request, CancellationToken cancellationToken)
    {
        var source = _trackSourceResolver.GetTrackSource(request.Url);

        var command = source switch
        {
            TrackSource.Youtube => new GetYouTubeTrackInfoRequest(request.Url),
            TrackSource.Undefined or _ => throw new InvalidDataException(),
        };

        return _sender.Send(command, cancellationToken);
    }
}