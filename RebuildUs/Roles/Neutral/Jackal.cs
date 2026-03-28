namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
[RegisterRole(RoleType.Jackal, RoleTeam.Neutral, typeof(SingleRoleBase<Jackal>), nameof(CustomOptionHolder.JackalSpawnRate))]
internal class Jackal : SingleRoleBase<Jackal>
{
    public static Color Color = new Color32(0, 180, 235, byte.MaxValue);

    private static CustomButton JackalKillButton;
    private static CustomButton JackalSidekickButton;
    private static CustomButton JackalSabotageLightsButton;
    internal static List<PlayerControl> FormerJackals = [];
    private PlayerControl _currentTarget;
    internal bool CanSidekick;

    internal PlayerControl FakeSidekick;
    internal PlayerControl MySidekick;
    internal bool WasImpostor = false;
    internal bool WasSpy = false;
    internal bool WasTeamRed = false;

    public Jackal()
    {
        StaticRoleType = CurrentRoleType = RoleType.Jackal;
        CanSidekick = CanCreateSidekick;
    }

    internal static float KillCooldown { get => CustomOptionHolder.JackalKillCooldown.GetFloat(); }
    private static bool CanSabotageLights { get => CustomOptionHolder.JackalCanSabotageLights.GetBool(); }
    internal static bool CanUseVents { get => CustomOptionHolder.JackalCanUseVents.GetBool(); }
    internal static bool HasImpostorVision { get => CustomOptionHolder.JackalHasImpostorVision.GetBool(); }
    internal static bool CanCreateSidekick { get => CustomOptionHolder.JackalCanCreateSidekick.GetBool(); }
    private static float CreateSidekickCooldown { get => CustomOptionHolder.JackalCreateSidekickCooldown.GetFloat(); }
    internal static bool JackalPromotedFromSidekickCanCreateSidekick { get => CustomOptionHolder.JackalPromotedFromSidekickCanCreateSidekick.GetBool(); }
    internal static bool CanCreateSidekickFromImpostor { get => CustomOptionHolder.JackalCanCreateSidekickFromImpostor.GetBool(); }

    internal override void OnUpdateRoleColors()
    {
        var lp = PlayerControl.LocalPlayer;
        if (Player == lp)
        {
            HudManagerPatch.SetPlayerNameColor(Player, RoleColor);
            if (Sidekick.Exists && Sidekick.PlayerControl)
            {
                HudManagerPatch.SetPlayerNameColor(Sidekick.PlayerControl, RoleColor);
            }

            if (FakeSidekick != null)
            {
                HudManagerPatch.SetPlayerNameColor(FakeSidekick, RoleColor);
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
        if (CanCreateSidekickFromImpostor)
        {
            // Only exclude sidekick from being targeted if the jackal can create sidekicks from impostors
            if (Sidekick.Exists)
            {
                untargetablePlayers.Add(Sidekick.PlayerControl);
            }
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
        Helpers.SetPlayerOutline(_currentTarget, Palette.ImpostorRed);
    }

    [CustomEvent(CustomEventType.OnDeath)]
    internal void OnDeath(PlayerControl killer)
    {
        // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
        if (!Sidekick.PromotesToJackal || MySidekick == null || !MySidekick.IsAlive())
        {
            return;
        }
        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SidekickPromotes);
        RPCProcedure.SidekickPromotes();
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        JackalKillButton = new(
            nameof(JackalKillButton),
            () =>
            {
                if (Local == null)
                {
                    return;
                }
                if (Helpers.CheckMurderAttemptAndKill(Local.Player, Local._currentTarget) == MurderAttemptResult.SuppressKill)
                {
                    return;
                }

                JackalKillButton?.Timer = JackalKillButton.MaxTimer;
                Local._currentTarget = null;
            },
            () => PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                return Local != null && Local._currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                JackalKillButton?.Timer = JackalKillButton.MaxTimer;
            },
            hm.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilityPrimary,
            false,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel));

        // Jackal Sidekick Button
        JackalSidekickButton = new(
            nameof(JackalSidekickButton),
            () =>
            {
                if (Local == null || Local._currentTarget == null)
                {
                    return;
                }

                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.JackalCreatesSidekick);
                sender.Write(Local._currentTarget.PlayerId);
                RPCProcedure.JackalCreatesSidekick(Local._currentTarget.PlayerId);
            },
            () =>
            {
                return Local != null && Local.CanSidekick && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                return Local != null && Local.CanSidekick && Local._currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                JackalSidekickButton?.Timer = JackalSidekickButton.MaxTimer;
            },
            AssetLoader.SidekickButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilitySecondary,
            false,
            Tr.Get(TrKey.SidekickText));

        JackalSabotageLightsButton = new(
            nameof(JackalSabotageLightsButton),
            () =>
            {
                MapUtilities.CachedShipStatus?.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
            },
            () => PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && CanSabotageLights && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                if (JackalSabotageLightsButton == null) return false;

                if (Helpers.SabotageTimer() > JackalSabotageLightsButton?.Timer || Helpers.SabotageActive())
                {
                    // this will give imps time to do another sabotage.
                    JackalSabotageLightsButton?.Timer = Helpers.SabotageTimer() + 5f;
                }

                return Helpers.CanUseSabotage();
            },
            () =>
            {
                JackalSabotageLightsButton?.Timer = Helpers.SabotageTimer() + 5f;
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
        JackalKillButton.MaxTimer = KillCooldown;
        JackalSidekickButton.MaxTimer = CreateSidekickCooldown;
    }

    internal static void RemoveCurrentJackal()
    {
        if (Instance == null)
        {
            return;
        }

        var alreadyFormer = false;
        foreach (var t in FormerJackals)
        {
            if (t.PlayerId != Instance.Player.PlayerId)
            {
                continue;
            }
            alreadyFormer = true;
            break;
        }

        if (!alreadyFormer)
        {
            FormerJackals.Add(Instance.Player);
        }
        Instance.Player.EraseRole(RoleType.Jackal);
    }

    internal static void Clear()
    {
        FormerJackals = [];
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}