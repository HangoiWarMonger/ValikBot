using MediatR;

namespace Bot.Application.Music.Commands.Common.GetUrlFromTextRequest;

/// <summary>
/// Запрос на получение списка ссылок на треки из запроса.
/// </summary>
public class GetUrlFromTextRequest : IRequest<string[]>
{
    /// <summary>
    /// Текст-запрос.
    /// </summary>
    public string RequestText { get; init; }

    /// <summary>
    /// Запрос на получение списка ссылок на треки из запроса.
    /// </summary>
    /// <param name="requestText">Текст-запрос.</param>
    public GetUrlFromTextRequest(string requestText)
    {
        RequestText = requestText;
    }
}