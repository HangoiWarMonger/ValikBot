using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Validation;
//using Bot.Domain.Validation;
using MediatR;

namespace Bot.Application.Music.Commands.Common.GetUrlFromTextRequest;

public class GetUrlFromTextHandler : IRequestHandler<GetUrlFromTextRequest, string[]>
{
    private readonly ITrackClient _trackClient;

    public GetUrlFromTextHandler(ITrackClient trackClient)
    {
        _trackClient = trackClient;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<string[]> Handle(GetUrlFromTextRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NullOrWhiteSpace(request.RequestText, nameof(request.RequestText));

        Uri.TryCreate(request.RequestText, UriKind.Absolute, out Uri? uriResult);

        if (uriResult is null)
        {
            var result = await _trackClient.SearchTrackUrlAsync(request.RequestText, cancellationToken);
            ThrowIf.NullOrWhiteSpace(result, nameof(result));

            return [result!];
        }

        var tracksFromLink = await _trackClient.GetTracksFromLink(request.RequestText);

        return tracksFromLink;
    }
}