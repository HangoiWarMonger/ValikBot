using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot.Infrastructure.YouTube;

/// <summary>
/// Service that refreshes YouTube cookies on startup using yt-dlp.
/// </summary>
public sealed class YouTubeCookieUpdater : IHostedService
{
    private readonly YoutubeApiOptions _options;
    private readonly ILogger<YouTubeCookieUpdater> _logger;

    public YouTubeCookieUpdater(IOptions<YoutubeApiOptions> options, ILogger<YouTubeCookieUpdater> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var cookiesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies.txt");

        var startInfo = new ProcessStartInfo
        {
            FileName = _options.YtDlpPath,
            Arguments = $"--cookies {cookiesPath} --username {_options.Login} --password {_options.Password} https://www.youtube.com",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process is null)
            {
                _logger.LogError("Не удалось запустить yt-dlp для обновления cookie.");
                return;
            }

            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode == 0)
            {
                _logger.LogInformation("Cookie YouTube обновлены.");
            }
            else
            {
                var error = await process.StandardError.ReadToEndAsync();
                _logger.LogError("yt-dlp завершился с кодом {Code}: {Error}", process.ExitCode, error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении cookie YouTube.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
