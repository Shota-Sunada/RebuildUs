namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
[RegisterRole(RoleType.Jackal, RoleTeam.Neutral, typeof(SingleRoleBase<Jackal>), nameof(CustomOptionHolder.JackalSpawnRate))]
internal class Jackal : SingleRoleBase<Jackal>
{
    internal static new Color RoleColor = new Color32(0, 180, 235, byte.MaxValue);

    private static CustomButton _jackalKillButton;
    private static CustomButton _jackalSidekickButton;
    private static CustomButton _jackalSabotageLightsButton;
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
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Jackal;
        CanSidekick = CanCreateSidekick;
    }

    // write configs here
    internal static float KillCooldown
    {
        get => CustomOptionHolder.JackalKillCooldown.GetFloat();
    }

    private static bool CanSabotageLights
    {
        get => CustomOptionHolder.JackalCanSabotageLights.GetBool();
    }

    internal static bool CanUseVents
    {
        get => CustomOptionHolder.JackalCanUseVents.GetBool();
    }

    internal static bool HasImpostorVision
    {
        get => CustomOptionHolder.JackalHasImpostorVision.GetBool();
    }

    internal static bool CanCreateSidekick
    {
        get => CustomOptionHolder.JackalCanCreateSidekick.GetBool();
    }

    private static float CreateSidekickCooldown
    {
        get => CustomOptionHolder.JackalCreateSidekickCooldown.GetFloat();
    }

    internal static bool JackalPromotedFromSidekickCanCreateSidekick
    {
        get => CustomOptionHolder.JackalPromotedFromSidekickCanCreateSidekick.GetBool();
    }

    internal static bool CanCreateSidekickFromImpostor
    {
        get => CustomOptionHolder.JackalCanCreateSidekickFromImpostor.GetBool();
    }

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
        var local = Local;
        if (local == null)
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
        SidekickPromotes(PlayerControl.LocalPlayer);
    }



    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _jackalKillButton = new(() =>
            {
                var local = Local;
                if (local == null)
                {
                    return;
                }
                if (Helpers.CheckMurderAttemptAndKill(local.Player, local._currentTarget) == MurderAttemptResult.SuppressKill)
                {
                    return;
                }

                _jackalKillButton?.Timer = _jackalKillButton.MaxTimer;
                local._currentTarget = null;
            },
            () => PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                var local = Local;
                return local != null && local._currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                _jackalKillButton?.Timer = _jackalKillButton.MaxTimer;
            },
            hm.KillButton.graphic.sprite,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilityPrimary,
            false,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel));

        // Jackal Sidekick Button
        _jackalSidekickButton = new(() =>
            {
                var local = Local;
                if (local == null || local._currentTarget == null)
                {
                    return;
                }

                JackalCreatesSidekick(PlayerControl.LocalPlayer, local._currentTarget.PlayerId);
            },
            () =>
            {
                var local = Local;
                return local != null && local.CanSidekick && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && PlayerControl.LocalPlayer.IsAlive();
            },
            () =>
            {
                var local = Local;
                return local != null && local.CanSidekick && local._currentTarget != null && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                _jackalSidekickButton?.Timer = _jackalSidekickButton.MaxTimer;
            },
            AssetLoader.SidekickButton,
            ButtonPosition.Layout,
            hm,
            hm.KillButton,
            AbilitySlot.NeutralAbilitySecondary,
            false,
            Tr.Get(TrKey.SidekickText));

        _jackalSabotageLightsButton = new(() =>
            {
                MapUtilities.CachedShipStatus?.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
            },
            () => PlayerControl.LocalPlayer != null
                  && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal)
                  && CanSabotageLights
                  && PlayerControl.LocalPlayer.IsAlive(),
            () =>
            {
                if (_jackalSabotageLightsButton == null)
                {
                    return false;
                }
                if (Helpers.SabotageTimer() > _jackalSabotageLightsButton.Timer || Helpers.SabotageActive())
                {
                    // this will give imps time to do another sabotage.
                    _jackalSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
                }

                return Helpers.CanUseSabotage();
            },
            () =>
            {
                _jackalSabotageLightsButton?.Timer = Helpers.SabotageTimer() + 5f;
            },
            AssetLoader.LightsOutButton,
            ButtonPosition.Layout,
            hm,
            hm.AbilityButton,
            AbilitySlot.CommonAbilitySecondary,
            false,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FixLights));
    }

    [RegisterCustomButton]
    internal static void SetButtonCooldowns()
    {
        _jackalKillButton?.MaxTimer = KillCooldown;
        _jackalSidekickButton?.MaxTimer = CreateSidekickCooldown;
    }

    // write functions here
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

    [MethodRpc((uint)CustomRPC.JackalCreatesSidekick)]
    internal static void JackalCreatesSidekick(PlayerControl sender, byte targetId)
    {
        var target = Helpers.PlayerById(targetId);
        var jackal = Instance;
        if (target == null)
        {
            return;
        }

        if (!CanCreateSidekickFromImpostor && target.Data.Role.IsImpostor)
        {
            jackal?.FakeSidekick = target;
        }
        else
        {
            var wasSpy = target.IsRole(RoleType.Spy);
            var wasImpostor = target.IsTeamImpostor(); // This can only be reached if impostors can be sidekicked.
            FastDestroyableSingleton<RoleManager>.Instance.SetRole(target, RoleTypes.Crewmate);
            Eraser.ErasePlayerRolesLocal(target.PlayerId, true);
            if (target.SetRole(RoleType.Sidekick))
            {
                var sidekick = Sidekick.Instance;
                if (sidekick != null)
                {
                    if (wasSpy || wasImpostor)
                    {
                        sidekick.WasTeamRed = true;
                    }
                    sidekick.WasSpy = wasSpy;
                    sidekick.WasImpostor = wasImpostor;
                }
            }

            if (target.PlayerId == PlayerControl.LocalPlayer.PlayerId)
            {
                PlayerControl.LocalPlayer.moveable = true;
            }
        }

        if (jackal != null)
        {
            jackal.CanSidekick = false;
            jackal.MySidekick = target;
        }
    }

    [MethodRpc((uint)CustomRPC.SidekickPromotes)]
    internal static void SidekickPromotes(PlayerControl sender)
    {
        var sidekick = Sidekick.Instance;
        if (sidekick == null)
        {
            return;
        }

        var wasTeamRed = sidekick.WasTeamRed;
        var wasImpostor = sidekick.WasImpostor;
        var wasSpy = sidekick.WasSpy;
        RemoveCurrentJackal();
        FastDestroyableSingleton<RoleManager>.Instance.SetRole(sidekick.Player, RoleTypes.Crewmate);
        Eraser.ErasePlayerRolesLocal(sidekick.Player.PlayerId, true);
        if (sidekick.Player.SetRole(RoleType.Jackal))
        {
            var newJackal = Instance;
            if (newJackal != null)
            {
                newJackal.CanSidekick = JackalPromotedFromSidekickCanCreateSidekick;
                newJackal.WasTeamRed = wasTeamRed;
                newJackal.WasImpostor = wasImpostor;
                newJackal.WasSpy = wasSpy;
            }
        }

        Sidekick.Clear();
    }

    internal static void Clear()
    {
        FormerJackals = [];
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}