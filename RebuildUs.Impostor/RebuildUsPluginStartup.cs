using Impostor.Api.Plugins;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace RebuildUs.Impostor;

public class RebuildUsPluginStartup : IPluginStartup
{
    public void ConfigureHost(IHostBuilder host) { }

    public void ConfigureServices(IServiceCollection services) =>
        services.AddSingleton<IGameCodeManager, GameCodeManager>();
}