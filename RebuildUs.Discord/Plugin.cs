using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using RebuildUs.Discord.Services;

namespace RebuildUs.Discord;

[BepInPlugin(ID, NAME, VERSION)]
public class Plugin : BasePlugin
{
    public const string ID = "RebuildUs.Discord";
    public const string NAME = "RebuildUs.Discord";
    public const string VERSION = "1.0.0";

    public static Plugin Instance { get; private set; } = null!;
    public Harmony Harmony { get; } = new(ID);

    // Config entries
    public ConfigEntry<string> BotToken { get; private set; } = null!;
    public ConfigEntry<string> BotTokenAux1 { get; private set; } = null!;
    public ConfigEntry<string> BotTokenAux2 { get; private set; } = null!;
    public ConfigEntry<string> GuildId { get; private set; } = null!;
    public ConfigEntry<string> LinkChannelId { get; private set; } = null!;

    public override void Load()
    {
        Instance = this;

        // BepInExのコンフィグ
        BotToken = Config.Bind("Discord", "BotToken", "", "Token for the Main Discord bot");
        BotTokenAux1 = Config.Bind("Discord", "BotTokenAux1", "", "Token for the 1st Auxiliary Discord bot");
        BotTokenAux2 = Config.Bind("Discord", "BotTokenAux2", "", "Token for the 2nd Auxiliary Discord bot");
        GuildId = Config.Bind("Discord", "GuildId", "", "ID of the Discord server (Guild)");
        LinkChannelId = Config.Bind("Discord", "LinkChannelId", "", "ID of the Channel to post the Link Embed in");

        LinkManager.Load();

        Harmony.PatchAll();
        Log.LogInfo($"Plugin {ID} is loaded!");

        _ = Task.Run(async () => await DiscordMultiBotService.Instance.InitializeAsync());
    }
}