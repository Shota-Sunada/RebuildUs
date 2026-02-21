namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Bait : RoleBase<Bait>
{
    internal static Color NameColor = new Color32(0, 247, 255, byte.MaxValue);
    private float _delay;

    private bool _reported;
    private CustomMessage _warningMessage;

    public Bait()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Bait;
        _delay = ReportDelay;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static bool HighlightAllVents { get => CustomOptionHolder.BaitHighlightAllVents.GetBool(); }
    private static float ReportDelay { get => CustomOptionHolder.BaitReportDelay.GetFloat(); }
    internal static bool ShowKillFlash { get => CustomOptionHolder.BaitShowKillFlash.GetBool(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (Player == null) return;

        // Bait report
        if (!Player.Data.IsDead || _reported) return;
        DeadPlayer baitDeadPlayer = null;
        List<DeadPlayer> deadPlayers = GameHistory.DeadPlayers;
        if (deadPlayers != null)
        {
            foreach (DeadPlayer dp in deadPlayers)
            {
                if (dp.Player == null || dp.Player.PlayerId != Player.PlayerId) continue;
                baitDeadPlayer = dp;
                break;
            }
        }

        if (baitDeadPlayer != null && baitDeadPlayer.KillerIfExisting != null)
        {
            // Show warning to the killer
            if (baitDeadPlayer.KillerIfExisting == PlayerControl.LocalPlayer) _warningMessage ??= new("BaitWarning", _delay);
        }

        if (Player != PlayerControl.LocalPlayer) return;

        _delay -= Time.fixedDeltaTime;

        if (baitDeadPlayer == null || baitDeadPlayer.KillerIfExisting == null || !(_delay <= 0f)) return;
        Helpers.HandleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called

        byte reporter = baitDeadPlayer.KillerIfExisting.PlayerId;
        if (Player.HasModifier(ModifierType.Madmate))
        {
            List<PlayerControl> candidates = new();
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p != null && p.IsAlive() && !p.IsTeamImpostor() && !p.isDummy)
                    candidates.Add(p);
            }

            if (candidates.Count > 0)
            {
                int i = RebuildUs.Rnd.Next(0, candidates.Count);
                reporter = candidates[i].PlayerId;
            }
        }

        {
            using RPCSender sender = new(Player.NetId, CustomRPC.UncheckedCmdReportDeadBody);
            sender.Write(reporter);
            sender.Write(Player.PlayerId);
        }
        RPCProcedure.UncheckedCmdReportDeadBody(reporter, Player.PlayerId);
        _reported = true;
        _warningMessage = null;
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}