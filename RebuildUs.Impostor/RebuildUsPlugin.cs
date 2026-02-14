using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;
using RebuildUs.Impostor.Services;

namespace RebuildUs.Impostor;

/// <summary>
///     The metadata information of your plugin, this is required.
/// </summary>
[ImpostorPlugin("gg.impostor.rebuildus")]
public class RebuildUsPlugin(ILogger<RebuildUsPlugin> logger, IGameCodeManager manager, IDiscordService discordService) : PluginBase
{
    /// <summary>
    ///     This is called when your plugin is enabled by the server.
    /// </summary>
    /// <returns></returns>
    public override async ValueTask EnableAsync()
    {
        logger.LogInformation("RebuildUs plugin is being enabled.");
        var sum = manager.FourCharCodes + manager.SixCharCodes;
        logger.LogInformation(
            "RebuildUs.Codes: loaded {FourCharCodes} 4-char codes and {SixCharCodes} 6-char codes [{Total} total] from {Path}!",
            manager.FourCharCodes, manager.SixCharCodes, sum, manager.Path);

        await discordService.StartAsync();
    }

    /// <summary>
    ///     This is called when your plugin is disabled by the server.
    ///     Most likely because it is shutting down, this is the place to clean up any managed resources.
    /// </summary>
    /// <returns></returns>
    public override async ValueTask DisableAsync()
    {
        logger.LogInformation("RebuildUs plugin is being disabled.");
        await discordService.StopAsync();
    }
}