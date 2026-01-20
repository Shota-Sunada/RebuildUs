using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace RebuildUs.Impostor
{
    /// <summary>
    ///     The metadata information of your plugin, this is required.
    /// </summary>
    [ImpostorPlugin("gg.impostor.rebuildus")]
    public class RebuildUsPlugin : PluginBase
    {
        /// <summary>
        ///     A logger that works seamlessly with the server.
        /// </summary>
        private readonly ILogger<RebuildUsPlugin> _logger;

        /// <summary>
        ///     The constructor of the plugin. There are a few parameters you can add here and they
        ///     will be injected automatically by the server.
        ///
        ///     They are not necessary but very recommended.
        /// </summary>
        /// <param name="logger">
        ///     A logger to write messages in the console.
        /// </param>
        /// <param name="eventManager">
        ///     An event manager to register event listeners.
        ///     Useful if you want your plugin to interact with the game.
        /// </param>
        public RebuildUsPlugin(ILogger<RebuildUsPlugin> logger, IEventManager eventManager)
        {
            _logger = logger;
        }

        /// <summary>
        ///     This is called when your plugin is enabled by the server.
        /// </summary>
        /// <returns></returns>
        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("RebuildUs plugin is being enabled.");
            return default;
        }

        /// <summary>
        ///     This is called when your plugin is disabled by the server.
        ///     Most likely because it is shutting down, this is the place to clean up any managed resources.
        /// </summary>
        /// <returns></returns>
        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("RebuildUs plugin is being disabled.");
            return default;
        }
    }
}
