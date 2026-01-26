global using Il2CppInterop.Runtime;
global using Il2CppInterop.Runtime.InteropTypes;
global using Il2CppInterop.Runtime.InteropTypes.Arrays;
global using Il2CppInterop.Runtime.Injection;

global using HarmonyLib;
global using Hazel;
global using BepInEx;
global using BepInEx.Logging;
global using BepInEx.Configuration;
global using BepInEx.Unity.IL2CPP;
global using UnityEngine;
global using TMPro;
global using System.Text;
global using AmongUs.GameOptions;
global using RebuildUs.Enums;
global using RebuildUs.Extensions;
global using RebuildUs.Localization;
global using RebuildUs.Modules;
global using RebuildUs.Modules.Consoles;
global using RebuildUs.Modules.CustomOptions;
global using RebuildUs.Modules.Discord;
global using RebuildUs.Modules.EndGame;
global using RebuildUs.Modules.GameEvents.Events;
global using RebuildUs.Modules.RPC;
global using RebuildUs.Objects;
global using RebuildUs.Patches;
global using RebuildUs.Roles;
global using RebuildUs.Roles.Crewmate;
global using RebuildUs.Roles.Impostor;
global using RebuildUs.Roles.Neutral;
global using RebuildUs.Roles.Modifier;
global using RebuildUs.Utilities;

global using Reactor.Networking;
global using Reactor.Networking.Attributes;

namespace RebuildUs;

[BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
[BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, SubmergedCompatibility.SUBMERGED_VERSION)]
[BepInDependency(REACTOR_GUID, REACTOR_VERSION)]
[BepInProcess("Among Us.exe")]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public class RebuildUs : BasePlugin
{
    public const string MOD_ID = "com.shota-sunada.rebuild-us";
    public const string MOD_NAME = "Rebuild Us";
    public const string MOD_VERSION = "1.1.0";
    public const string MOD_DEVELOPER = "Shota Sunada";

    public const string REACTOR_GUID = "gg.reactor-sunada.api";
    public const string REACTOR_VERSION = "3.3.4-SND";

    public static RebuildUs Instance;
    public Harmony Harmony { get; } = new(MOD_ID);
    public Version Version { get; } = Version.Parse(MOD_VERSION);

    public static int OptionsPage = 0;

    public static ConfigEntry<bool> GhostsSeeInformation { get; set; }
    public static ConfigEntry<bool> GhostsSeeRoles { get; set; }
    public static ConfigEntry<bool> GhostsSeeModifier { get; set; }
    public static ConfigEntry<bool> GhostsSeeVotes { get; set; }
    public static ConfigEntry<bool> ShowRoleSummary { get; set; }
    public static ConfigEntry<bool> ShowLighterDarker { get; set; }
    public static ConfigEntry<bool> ShowVentsOnMap { get; set; }
    public static ConfigEntry<bool> ShowChatNotifications { get; set; }
    public static ConfigEntry<bool> ForceNormalSabotageMap { get; set; }
    public static ConfigEntry<bool> BetterSabotageMap { get; set; }
    public static ConfigEntry<bool> TransparentMap { get; set; }
    public static ConfigEntry<bool> HideFakeTasks { get; set; }

    public static ConfigEntry<string> DiscordBotToken { get; set; }
    public static ConfigEntry<string> DiscordBotToken2 { get; set; }
    public static ConfigEntry<string> DiscordBotToken3 { get; set; }
    public static ConfigEntry<string> DiscordGuildId { get; set; }
    public static ConfigEntry<string> DiscordVCId { get; set; }
    public static ConfigEntry<string> StatusChannelId { get; set; }

    public static ConfigEntry<string> Ip { get; set; }
    public static ConfigEntry<ushort> Port { get; set; }
    public static IRegionInfo[] DefaultRegions;

    // public System.Random Rnd = new((int)DateTime.Now.Ticks);
    public System.Random Rnd = new MersenneTwister((int)DateTime.Now.Ticks);

    public override void Load()
    {
        Logger.Initialize(Log);
        Instance = this;

        Debug.Initialize();

        GhostsSeeInformation = Config.Bind("Custom", "Ghosts See Remaining Tasks", true);
        GhostsSeeRoles = Config.Bind("Custom", "Ghosts See Roles", true);
        GhostsSeeModifier = Config.Bind("Custom", "Ghosts See Modifier", true);
        GhostsSeeVotes = Config.Bind("Custom", "Ghosts See Votes", true);
        ShowRoleSummary = Config.Bind("Custom", "Show Role Summary", true);
        ShowLighterDarker = Config.Bind("Custom", "Show Lighter / Darker", false);
        ShowVentsOnMap = Config.Bind("Custom", "Show vent positions on minimap", false);
        ShowChatNotifications = Config.Bind("Custom", "Show Chat Notifications", true);
        ForceNormalSabotageMap = Config.Bind("Custom", "Force Normal Sabotage Map", false);
        BetterSabotageMap = Config.Bind("Custom", "Better Sabotage Map", false);
        TransparentMap = Config.Bind("Custom", "Transparent Map", false);
        HideFakeTasks = Config.Bind("Custom", "Hide Fake Tasks", false);

        DiscordBotToken = Config.Bind("Discord", "Bot Token", "");
        DiscordBotToken2 = Config.Bind("Discord", "Bot Token 2", "");
        DiscordBotToken3 = Config.Bind("Discord", "Bot Token 3", "");
        DiscordGuildId = Config.Bind("Discord", "Guild ID", "");
        DiscordVCId = Config.Bind("Discord", "Voice Channel ID", "");
        StatusChannelId = Config.Bind("Discord", "Status Channel ID", "");

        KeyBindingManager.Initialize(Config);

        Ip = Config.Bind("Custom", "Custom Server IP", "127.0.0.1");
        Port = Config.Bind("Custom", "Custom Server Port", (ushort)22023);
        DefaultRegions = ServerManager.DefaultRegions;

        AssetLoader.LoadAssets();

        Tr.Initialize();
        RoleInfo.Load();
        CustomColors.Load();
        UpdateRegions();
        CustomOptionHolder.Load();

        DiscordModManager.Initialize();

        Harmony.PatchAll();

        SubmergedCompatibility.Initialize();

        Logger.LogMessage("\"Rebuild Us\" was completely loaded! Enjoy the modifications!");
    }

    public static void ClearAndReloadRoles()
    {
        // Crewmate
        Bait.Clear();
        Detective.Clear();
        Engineer.Clear();
        Hacker.Clear();
        Lighter.Clear();
        Mayor.Clear();
        Medic.Clear();
        Medium.Clear();
        SecurityGuard.Clear();
        Seer.Clear();
        Sheriff.Clear();
        Shifter.Clear();
        Snitch.Clear();
        Spy.Clear();
        Swapper.Clear();
        TimeMaster.Clear();
        Tracker.Clear();

        // Impostor
        BountyHunter.Clear();
        Camouflager.Clear();
        Cleaner.Clear();
        Eraser.Clear();
        EvilHacker.Clear();
        EvilTracker.Clear();
        Morphing.Clear();
        Trickster.Clear();
        Vampire.Clear();
        Warlock.Clear();
        Witch.Clear();
        Mafia.ClearAndReload();

        // Neutral
        Arsonist.Clear();
        Jackal.Clear();
        Jester.Clear();
        Guesser.ClearAndReload();
        Sidekick.Clear();
        Vulture.Clear();

        // Modifier
        AntiTeleport.Clear();
        CreatedMadmate.Clear();
        LastImpostor.Clear();
        Lovers.Clear();
        Madmate.Clear();
        Mini.Clear();

        PlayerRole.ClearAll();
        PlayerModifier.ClearAll();
    }

    public static void FixedUpdate(PlayerControl player)
    {
        PlayerRole.AllRoles.DoIf(x => x.Player == player, x => x.FixedUpdate());
        PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => x.FixedUpdate());
    }

    public static void OnMeetingStart()
    {
        PlayerRole.AllRoles.Do(x => x.OnMeetingStart());
        PlayerModifier.AllModifiers.Do(x => x.OnMeetingStart());

        // GM.resetZoom();
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>((p) =>
        {
            if (p == 1)
            {
                Camouflager.ResetCamouflage();
                Morphing.ResetMorph();
            }
        })));
    }

    public static void OnMeetingEnd()
    {
        PlayerRole.AllRoles.Do(x => x.OnMeetingEnd());
        PlayerModifier.AllModifiers.Do(x => x.OnMeetingEnd());

        CustomOverlays.HideInfoOverlay();
        CustomOverlays.HideBlackBG();
    }

    public static void OnIntroEnd()
    {
        PlayerRole.AllRoles.Do(x => x.OnIntroEnd());
        PlayerModifier.AllModifiers.Do(x => x.OnIntroEnd());
    }

    public static void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
    {
        if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
        {
            PlayerRole.AllRoles.Do(x => x.HandleDisconnect(player, reason));
            PlayerModifier.AllModifiers.Do(x => x.HandleDisconnect(player, reason));

            Lovers.HandleDisconnect(player, reason);
            // Shifter.HandleDisconnect(player, reason);

            GameHistory.FinalStatuses[player.PlayerId] = FinalStatus.Disconnected;
        }
    }

    public static void MakeButtons(HudManager hm)
    {
        // Crewmate
        Engineer.MakeButtons(hm);
        Hacker.MakeButtons(hm);
        Lighter.MakeButtons(hm);
        Mayor.MakeButtons(hm);
        Medic.MakeButtons(hm);
        Medium.MakeButtons(hm);
        Sheriff.MakeButtons(hm);
        Shifter.MakeButtons(hm);
        TimeMaster.MakeButtons(hm);
        SecurityGuard.MakeButtons(hm);
        Tracker.MakeButtons(hm);

        // Impostor
        Camouflager.MakeButtons(hm);
        Cleaner.MakeButtons(hm);
        Eraser.MakeButtons(hm);
        EvilHacker.MakeButtons(hm);
        EvilTracker.MakeButtons(hm);
        Mafia.Janitor.MakeButtons(hm);
        Morphing.MakeButtons(hm);
        Trickster.MakeButtons(hm);
        Vampire.MakeButtons(hm);
        Warlock.MakeButtons(hm);
        Witch.MakeButtons(hm);

        // Neutral
        Arsonist.MakeButtons(hm);
        Jackal.MakeButtons(hm);
        Sidekick.MakeButtons(hm);
        Vulture.MakeButtons(hm);

        // Modifier
        LastImpostor.MakeButtons(hm);
    }

    public static void SetButtonCooldowns()
    {
        // Crewmate
        Engineer.SetButtonCooldowns();
        Hacker.SetButtonCooldowns();
        Lighter.SetButtonCooldowns();
        Mayor.SetButtonCooldowns();
        Medic.SetButtonCooldowns();
        Medium.SetButtonCooldowns();
        Sheriff.SetButtonCooldowns();
        Shifter.SetButtonCooldowns();
        TimeMaster.SetButtonCooldowns();
        SecurityGuard.SetButtonCooldowns();
        Tracker.SetButtonCooldowns();

        // Impostor
        Camouflager.SetButtonCooldowns();
        Cleaner.SetButtonCooldowns();
        Eraser.SetButtonCooldowns();
        EvilHacker.SetButtonCooldowns();
        EvilTracker.SetButtonCooldowns();
        Mafia.Janitor.SetButtonCooldowns();
        Morphing.SetButtonCooldowns();
        Trickster.SetButtonCooldowns();
        Vampire.SetButtonCooldowns();
        Warlock.SetButtonCooldowns();
        Witch.SetButtonCooldowns();

        // Neutral
        Arsonist.SetButtonCooldowns();
        Jackal.SetButtonCooldowns();
        Sidekick.SetButtonCooldowns();
        Vulture.SetButtonCooldowns();
    }

    public static void UpdateRegions()
    {
        var serverManager = FastDestroyableSingleton<ServerManager>.Instance;

        serverManager.AvailableRegions = ServerManager.DefaultRegions;

        IRegionInfo[] regions = [new DnsRegionInfo(Ip.Value, "Custom", StringNames.NoTranslation, Ip.Value, Port.Value, false).CastFast<IRegionInfo>()];
#nullable enable
        IRegionInfo? currentRegion = serverManager.CurrentRegion;
#nullable disable
        foreach (IRegionInfo region in regions)
        {
            if (region == null)
            {
                Logger.LogError("Could not add region");
            }
            else
            {
                if (currentRegion != null && region.Name.Equals(currentRegion.Name, StringComparison.OrdinalIgnoreCase))
                {
                    currentRegion = region;
                }
                serverManager.AddOrUpdateRegion(region);
            }
        }

        if (currentRegion != null)
        {
            Logger.LogDebug("Resetting previous region");
            serverManager.SetRegion(currentRegion);
        }
    }
}