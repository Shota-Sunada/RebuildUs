namespace RebuildUs.Roles.Neutral;

[HarmonyPatch]
internal class Jackal : RoleBase<Jackal>
{
    internal static Color NameColor = new Color32(0, 180, 235, byte.MaxValue);

    internal static CustomButton JackalKillButton;
    private static CustomButton _jackalSidekickButton;
    internal static CustomButton JackalSabotageLightsButton;
    internal static List<PlayerControl> FormerJackals = [];
    internal bool CanSidekick;
    internal PlayerControl CurrentTarget;

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
    internal static float KillCooldown { get => CustomOptionHolder.JackalKillCooldown.GetFloat(); }
    internal static bool CanSabotageLights { get => CustomOptionHolder.JackalCanSabotageLights.GetBool(); }
    internal static bool CanUseVents { get => CustomOptionHolder.JackalCanUseVents.GetBool(); }
    internal static bool HasImpostorVision { get => CustomOptionHolder.JackalHasImpostorVision.GetBool(); }
    internal static bool CanCreateSidekick { get => CustomOptionHolder.JackalCanCreateSidekick.GetBool(); }
    internal static float CreateSidekickCooldown { get => CustomOptionHolder.JackalCreateSidekickCooldown.GetFloat(); }
    internal static bool JackalPromotedFromSidekickCanCreateSidekick { get => CustomOptionHolder.JackalPromotedFromSidekickCanCreateSidekick.GetBool(); }
    internal static bool CanCreateSidekickFromImpostor { get => CustomOptionHolder.JackalCanCreateSidekickFromImpostor.GetBool(); }

    internal override void OnUpdateNameColors()
    {
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (Player == lp)
        {
            Update.SetPlayerNameColor(Player, RoleColor);
            if (Sidekick.Exists)
            {
                List<Sidekick> skPlayers = Sidekick.Players;
                if (skPlayers.Count > 0)
                {
                    Sidekick sk = skPlayers[0];
                    if (sk != null) Update.SetPlayerNameColor(sk.Player, RoleColor);
                }
            }

            if (FakeSidekick != null) Update.SetPlayerNameColor(FakeSidekick, RoleColor);
        }
        else if (lp.IsTeamImpostor() && WasTeamRed) Update.SetPlayerNameColor(Player, RoleColor);
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        Jackal local = Local;
        if (local != null)
        {
            List<PlayerControl> untargetablePlayers = new();
            if (CanCreateSidekickFromImpostor)
            {
                // Only exclude sidekick from being targeted if the jackal can create sidekicks from impostors
                if (Sidekick.Exists)
                {
                    List<PlayerControl> skPlayers = Sidekick.AllPlayers;
                    for (int i = 0; i < skPlayers.Count; i++) untargetablePlayers.Add(skPlayers[i]);
                }
            }

            List<Mini> miniPlayers = Mini.Players;
            for (int i = 0; i < miniPlayers.Count; i++)
            {
                Mini mini = miniPlayers[i];
                if (!Mini.IsGrownUp(mini.Player)) untargetablePlayers.Add(mini.Player);
            }

            CurrentTarget = Helpers.SetTarget(untargetablePlayers: untargetablePlayers);
            Helpers.SetPlayerOutline(CurrentTarget, Palette.ImpostorRed);
        }
    }

    internal override void OnKill(PlayerControl target) { }

    internal override void OnDeath(PlayerControl killer = null)
    {
        // If LocalPlayer is Sidekick, the Jackal is disconnected and Sidekick promotion is enabled, then trigger promotion
        if (Sidekick.PromotesToJackal && MySidekick != null && MySidekick.IsAlive())
        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SidekickPromotes);
            sender.Write(MySidekick.PlayerId);
            RPCProcedure.SidekickPromotes(MySidekick.PlayerId);
        }
    }

    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        JackalKillButton = new(() =>
        {
            Jackal local = Local;
            if (local == null) return;
            if (Helpers.CheckMurderAttemptAndKill(local.Player, local.CurrentTarget) == MurderAttemptResult.SuppressKill) return;

            JackalKillButton?.Timer = JackalKillButton.MaxTimer;
            local.CurrentTarget = null;
        }, () => { return PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && PlayerControl.LocalPlayer.IsAlive(); }, () =>
        {
            Jackal local = Local;
            return local != null && local.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove;
        }, () => { JackalKillButton?.Timer = JackalKillButton.MaxTimer; }, hm.KillButton.graphic.sprite, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.NeutralAbilityPrimary, false, FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.KillLabel));

        // Jackal Sidekick Button
        _jackalSidekickButton = new(() =>
        {
            Jackal local = Local;
            if (local == null || local.CurrentTarget == null) return;

            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.JackalCreatesSidekick);
            sender.Write(local.CurrentTarget.PlayerId);
            sender.Write(PlayerControl.LocalPlayer.PlayerId);
            RPCProcedure.JackalCreatesSidekick(local.CurrentTarget.PlayerId, PlayerControl.LocalPlayer.PlayerId);
        }, () =>
        {
            Jackal local = Local;
            return local != null && local.CanSidekick && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && PlayerControl.LocalPlayer.IsAlive();
        }, () =>
        {
            Jackal local = Local;
            return local != null && local.CanSidekick && local.CurrentTarget != null && PlayerControl.LocalPlayer.CanMove;
        }, () => { _jackalSidekickButton?.Timer = _jackalSidekickButton.MaxTimer; }, AssetLoader.SidekickButton, ButtonPosition.Layout, hm, hm.KillButton, AbilitySlot.NeutralAbilitySecondary, false, Tr.Get(TrKey.SidekickText));

        JackalSabotageLightsButton = new(() =>
        {
            MapUtilities.CachedShipStatus?.RpcUpdateSystem(SystemTypes.Sabotage, (byte)SystemTypes.Electrical);
        }, () =>
        {
            return PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) && CanSabotageLights && PlayerControl.LocalPlayer.IsAlive();
        }, () =>
        {
            if (JackalSabotageLightsButton == null) return false;
            if (Helpers.SabotageTimer() > JackalSabotageLightsButton.Timer || Helpers.SabotageActive())
            {
                // this will give imps time to do another sabotage.
                JackalSabotageLightsButton.Timer = Helpers.SabotageTimer() + 5f;
            }

            return Helpers.CanUseSabotage();
        }, () =>
        {
            JackalSabotageLightsButton?.Timer = Helpers.SabotageTimer() + 5f;
        }, AssetLoader.LightsOutButton, ButtonPosition.Layout, hm, hm.AbilityButton, AbilitySlot.CommonAbilitySecondary, false, FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.FixLights));
    }

    internal static void SetButtonCooldowns()
    {
        JackalKillButton?.MaxTimer = KillCooldown;
        _jackalSidekickButton?.MaxTimer = CreateSidekickCooldown;
    }

    // write functions here
    internal static void RemoveCurrentJackal()
    {
        for (int i = Players.Count - 1; i >= 0; i--)
        {
            PlayerControl p = Players[i].Player;
            bool alreadyFormer = false;
            for (int j = 0; j < FormerJackals.Count; j++)
            {
                if (FormerJackals[j].PlayerId == p.PlayerId)
                {
                    alreadyFormer = true;
                    break;
                }
            }

            if (!alreadyFormer) FormerJackals.Add(p);
            p.EraseRole(RoleType.Jackal);
        }
    }

    internal static void Clear()
    {
        // reset configs here
        FormerJackals = [];
        Players.Clear();
    }
}