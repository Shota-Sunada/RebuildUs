using Impostor.Api.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RebuildUs.Impostor.Models;
using RebuildUs.Impostor.Services;

namespace RebuildUs.Impostor;

public sealed class RebuildUsPluginStartup : IPluginStartup
{
    public void ConfigureHost(IHostBuilder host)
    {
        host.ConfigureAppConfiguration((_, config) => { config.AddJsonFile("rebuildus.discord.json", true, true); });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IGameCodeManager, GameCodeManager>();
        services.AddSingleton<IPlayerMappingService, PlayerMappingService>();
        services.AddSingleton<IDiscordService, DiscordService>();

        services.AddOptions<DiscordConfig>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.GetSection("Discord").Bind(settings);

                    if (bool.TryParse(configuration["disableDiscord"], out var disableDiscord)) settings.DisableDiscord = disableDiscord;

                    if (bool.TryParse(configuration["Discord:AutoMute"], out var autoMute)) settings.AutoMute = autoMute;
                });
    }
}
