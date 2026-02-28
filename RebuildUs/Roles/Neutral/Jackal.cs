namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
[RegisterRole(RoleType.Jackal, RoleTeam.Neutral, typeof(SingleRoleBase<Jackal>), nameof(Jackal.NameColor), nameof(CustomOptionHolder.JackalSpawnRate))]
internal class Jackal : SingleRoleBase<Jackal>
{
    internal static Color NameColor = new Color32(0, 180, 235, byte.MaxValue);

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

    internal override Color RoleColor
    {
        get => NameColor;
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

    internal override void OnUpdateNameColors()
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

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
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

    internal override void OnKill(PlayerControl target) { }

    internal override void OnDeath(PlayerControl killer = null)
    {
        // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
        if (!Sidekick.PromotesToJackal || MySidekick == null || !MySidekick.IsAlive())
        {
            return;
        }
        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SidekickPromotes);
        RPCProcedure.SidekickPromotes();
    }

    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

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

                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.JackalCreatesSidekick);
                sender.Write(local._currentTarget.PlayerId);
                RPCProcedure.JackalCreatesSidekick(local._currentTarget.PlayerId);
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

    internal static void Clear()
    {
        FormerJackals = [];
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}