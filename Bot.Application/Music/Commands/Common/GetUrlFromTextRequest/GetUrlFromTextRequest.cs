using MediatR;

namespace Bot.Application.Music.Commands.Common.GetUrlFromTextRequest;

/// <summary>
/// Запрос на получение ссылки из запроса.
/// <remarks>Если запрос уже является ссылкой - вернет ссылку в неизменном виде.</remarks>
/// </summary>
public class GetUrlFromTextRequest : IRequest<string>
{
    /// <summary>
    /// Тест-запрос.
    /// </summary>
    public string RequestText { get; init; }

    public GetUrlFromTextRequest(string requestText)
    {
        RequestText = requestText;
    }
}