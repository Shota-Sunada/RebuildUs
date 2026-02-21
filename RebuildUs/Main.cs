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
global using RebuildUs.Modules.EndGame;
global using RebuildUs.Modules.GameEvents.Events;
global using RebuildUs.Modules.Random;
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
using InnerNet;
using Random = System.Random;

namespace RebuildUs;

[BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
[BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, SubmergedCompatibility.SUBMERGED_VERSION)]
[BepInDependency(REACTOR_GUID, REACTOR_VERSION)]
[BepInProcess("Among Us.exe")]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
// ReSharper disable once MemberCanBeInternal
// ReSharper disable once ClassNeverInstantiated.Global
public class RebuildUs : BasePlugin
{
    private const string MOD_ID = "com.shota-sunada.rebuild-us";
    internal const string MOD_NAME = "Rebuild Us";
    internal const string MOD_VERSION = "1.2.3";
    internal const string MOD_DEVELOPER = "Shota Sunada";

    private const string REACTOR_GUID = "gg.reactor-sunada.api";
    private const string REACTOR_VERSION = "3.3.4-SND";

    internal static RebuildUs Instance;

    internal static int OptionsPage = 0;
    internal static IRegionInfo[] DefaultRegions;
    private Harmony Harmony { get; } = new(MOD_ID);
    internal Version Version { get; } = Version.Parse(MOD_VERSION);

    internal static ConfigEntry<bool> GhostsSeeInformation { get; private set; }
    internal static ConfigEntry<bool> GhostsSeeRoles { get; private set; }
    internal static ConfigEntry<bool> GhostsSeeModifier { get; private set; }
    internal static ConfigEntry<bool> GhostsSeeVotes { get; private set; }
    internal static ConfigEntry<bool> ShowRoleSummary { get; private set; }
    internal static ConfigEntry<bool> ShowLighterDarker { get; private set; }
    internal static ConfigEntry<bool> ShowVentsOnMap { get; private set; }
    internal static ConfigEntry<bool> ShowChatNotifications { get; private set; }
    internal static ConfigEntry<bool> ForceNormalSabotageMap { get; private set; }
    internal static ConfigEntry<bool> BetterSabotageMap { get; private set; }
    internal static ConfigEntry<bool> TransparentMap { get; private set; }
    internal static ConfigEntry<bool> HideFakeTasks { get; private set; }

    private static ConfigEntry<string> Ip { get; set; }
    private static ConfigEntry<ushort> Port { get; set; }

    internal static Random Rnd
    {
        get => RandomMain.Rnd;
    }

    internal static void RefreshRnd(int seed)
    {
        RandomMain.RefreshRnd(seed);
    }

    public override void Load()
    {
        Logger.Initialize(Log);
        Instance = this;

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

        RefreshRnd((int)DateTime.Now.Ticks);

        Harmony.PatchAll();

        SubmergedCompatibility.Initialize();

        Logger.LogMessage("\"Rebuild Us\" was completely loaded! Enjoy the modifications!");
    }

    internal static void ClearAndReloadRoles()
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

        ModRoleManager.ClearAll();
        PlayerModifier.ClearAll();
    }

    internal static void FixedUpdate(PlayerControl player)
    {
        ModRoleManager.AllRoles.DoIf(x => x.Player == player, x => x.FixedUpdate());
        PlayerModifier.AllModifiers.DoIf(x => x.Player == player, x => x.FixedUpdate());
    }

    internal static void OnMeetingStart()
    {
        ModRoleManager.AllRoles.Do(x => x.OnMeetingStart());
        PlayerModifier.AllModifiers.Do(x => x.OnMeetingStart());

        // GM.resetZoom();
        FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(3f, new Action<float>(p =>
        {
            if (!Mathf.Approximately(p, 1)) return;
            Camouflager.ResetCamouflage();
            Morphing.ResetMorph();
        })));
    }

    internal static void OnMeetingEnd()
    {
        ModRoleManager.AllRoles.Do(x => x.OnMeetingEnd());
        PlayerModifier.AllModifiers.Do(x => x.OnMeetingEnd());

        CustomOverlays.HideInfoOverlay();
        CustomOverlays.HideBlackBg();
    }

    internal static void OnIntroEnd()
    {
        ModRoleManager.AllRoles.Do(x => x.OnIntroEnd());
        PlayerModifier.AllModifiers.Do(x => x.OnIntroEnd());
    }

    internal static void HandleDisconnect(PlayerControl player, DisconnectReasons reason)
    {
        if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
        {
            ModRoleManager.AllRoles.Do(x => x.HandleDisconnect(player, reason));
            PlayerModifier.AllModifiers.Do(x => x.HandleDisconnect(player, reason));

            Lovers.HandleDisconnect(player, reason);
            // Shifter.HandleDisconnect(player, reason);

            GameHistory.FinalStatuses[player.PlayerId] = FinalStatus.Disconnected;
        }
    }

    internal static void MakeButtons(HudManager hm)
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
        Suicider.MakeButtons(hm);

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

    internal static void SetButtonCooldowns()
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
        Suicider.SetButtonCooldowns();

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

    private static void UpdateRegions()
    {
        ServerManager serverManager = FastDestroyableSingleton<ServerManager>.Instance;

        serverManager.AvailableRegions = ServerManager.DefaultRegions;

        IRegionInfo[] regions = [new DnsRegionInfo(Ip.Value, "Custom", StringNames.NoTranslation, Ip.Value, Port.Value, false).CastFast<IRegionInfo>()];
#nullable enable
        IRegionInfo? currentRegion = serverManager.CurrentRegion;
#nullable disable
        foreach (IRegionInfo region in regions)
        {
            if (region == null)
                Logger.LogError("Could not add region");
            else
            {
                if (currentRegion != null && region.Name.Equals(currentRegion.Name, StringComparison.OrdinalIgnoreCase)) currentRegion = region;

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