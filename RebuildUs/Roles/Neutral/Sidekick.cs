namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
[RegisterRole(RoleType.Sidekick, RoleTeam.Neutral, typeof(SingleRoleBase<Sidekick>), nameof(CustomOptionHolder.JackalSpawnRate))]
internal class Sidekick : SingleRoleBase<Sidekick>
{
    private static CustomButton SidekickKillButton;
    private static CustomButton SidekickSabotageLightsButton;
    private PlayerControl _currentTarget;
    internal bool WasImpostor = false;
    internal bool WasSpy = false;
    internal bool WasTeamRed = false;

    public Sidekick()
    {
        StaticRoleType = CurrentRoleType = RoleType.Sidekick;
    }

    internal override Color RoleColor { get => Jackal.Color; }
    private static bool CanKill { get => CustomOptionHolder.SidekickCanKill.GetBool(); }
    internal static bool CanUseVents { get => CustomOptionHolder.SidekickCanUseVents.GetBool(); }
    private static bool CanSabotageLights { get => CustomOptionHolder.SidekickCanSabotageLights.GetBool(); }
    internal static bool HasImpostorVision { get => CustomOptionHolder.SidekickHasImpostorVision.GetBool(); }
    internal static bool PromotesToJackal { get => CustomOptionHolder.SidekickPromotesToJackal.GetBool(); }

    internal override void OnUpdateRoleColors()
    {
        var lp = PlayerControl.LocalPlayer;
        if (Player == lp)
        {
            HudManagerPatch.SetPlayerNameColor(Player, RoleColor);
            if (!Jackal.Exists)
            {
                return;
            }

            if (Jackal.Local.Player)
            {
                HudManagerPatch.SetPlayerNameColor(Jackal.Local.Player, RoleColor);
            }
        }
        else if (lp.IsTeamImpostor() && WasTeamRed)
        {
            HudManagerPatch.SetPlayerNameColor(Player, RoleColor);
        }
    }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        if (Local == null)
        {
            return;
        }
        List<PlayerControl> untargetablePlayers = [];
        if (Jackal.Exists)
        {
            untargetablePlayers.Add(Jackal.PlayerControl);
        }

        var miniPlayers = Mini.Players;
        foreach (var mini in miniPlayers)
        {
            if (!Mini.IsGrownUp(mini.Player))
            {
                untargetablePlayers.Add(mini.Player);
            }
        }

        _currentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePlayers);
        if (CanKill)
        {
            Helpers.SetPlayerOutline(_currentTarget, Palette.ImpostorRed);
        }
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        SidekickKillButton = new(
            nameof(SidekickKillButton),
            () =>
            {
                if (Helpers.CheckMurderAttemptAndKill(Local.Player, Local._currentTarget) == MurderAttemptResult.SuppressKill)
                {
                    return;
                }
                SidekickKillButton.Timer = SidekickKillButton.MaxTimer;
                Local._currentTarget = null;
            },
            () => CanKill && PlayerControl.LocalPlayer.IsRole(RoleType.Sidekick) && PlayerControl.LocalPlayer.IsAlive(),
            () => Local._currentTarget && PlayerControl.LocalPlayer.CanMove,
            () =>
            {
                SidekickKillButton.Timer = SidekickKillButton.MaxTimer;
            },
            hm.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilityPrimary,
            false,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel));

        SidekickSabotageLightsButton = new(
            nameof(SidekickSabotageLightsButton),
            () =>
            {
                MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
            },
            () => PlayerControl.LocalPlayer.IsRole(RoleType.Sidekick) && CanSabotageLights && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                if (Helpers.SabotageTimer() > SidekickSabotageLightsButton?.Timer || Helpers.SabotageActive())
                {
                    // this will give imps time to do another sabotage.
                    SidekickSabotageLightsButton?.Timer = Helpers.SabotageTimer() + 5f;
                }

                return Helpers.CanUseSabotage();
            },
            () =>
            {
                SidekickSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
            },
            AssetLoader.LightsOutButton,
            ButtonPosition.Layout,
            hm,
            hm.AbilityButton,
            AbilitySlot.CommonAbilitySecondary,
            false,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FixLights));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        SidekickKillButton.MaxTimer = Jackal.KillCooldown;
    }

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}