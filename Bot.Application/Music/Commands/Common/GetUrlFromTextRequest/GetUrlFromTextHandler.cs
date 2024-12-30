using Ardalis.GuardClauses;
using Bot.Application.Common.Interfaces;
using Bot.Domain.Validation;
using MediatR;

namespace Bot.Application.Music.Commands.Common.GetUrlFromTextRequest;

public class GetUrlFromTextHandler : IRequestHandler<GetUrlFromTextRequest, string[]>
{
    private readonly IFactory<ITrackClient, string> _trackClientFactory;
    private readonly ISearchService _searchService;

    public GetUrlFromTextHandler(ISearchService searchService, IFactory<ITrackClient, string> trackClientFactory)
    {
        _searchService = searchService;
        _trackClientFactory = trackClientFactory;
    }

    public async Task<string[]> Handle(GetUrlFromTextRequest request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.NullOrWhiteSpace(request.RequestText, nameof(request.RequestText));

        if (!Uri.TryCreate(request.RequestText, UriKind.Absolute, out _))
        {
            var result = await _searchService.SearchTrackUrlAsync(request.RequestText, cancellationToken);
            ThrowIf.NullOrWhiteSpace(result, nameof(result));

            return [result!];
        }

        var trackClient = _trackClientFactory.Get(request.RequestText);

        var tracksFromLink = await trackClient.GetTracksFromLink(request.RequestText);

        return tracksFromLink;
    }
}