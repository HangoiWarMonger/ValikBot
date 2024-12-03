using Ardalis.GuardClauses;
using Bot.Application.Common.Dto;
using Bot.Application.Common.Interfaces;
using MediatR;

namespace Bot.Application.Music.Commands.YouTube.GetTrackInfoRequest;

public class GetYouTubeTrackInfoRequestHandler : IRequestHandler<GetYouTubeTrackInfoRequest, TrackInfoDto>
{
    private readonly ITrackClient _trackClient;

    public GetYouTubeTrackInfoRequestHandler(ITrackClient trackClient)
    {
        _trackClient = trackClient;
    }

    public Task<TrackInfoDto> Handle(GetYouTubeTrackInfoRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));

        return _trackClient.GetInfoAsync(request.Url);
    }
}