namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Bait, RoleTeam.Crewmate, typeof(MultiRoleBase<Bait>), nameof(CustomOptionHolder.BaitSpawnRate))]
internal class Bait : MultiRoleBase<Bait>
{
    public static Color Color = new Color32(0, 247, 255, byte.MaxValue);
    private float _delay;

    private bool _reported;
    private CustomMessage _warningMessage;

    public Bait()
    {
        StaticRoleType = CurrentRoleType = RoleType.Bait;
        _delay = ReportDelay;
    }

    internal static bool HighlightAllVents { get => CustomOptionHolder.BaitHighlightAllVents.GetBool(); }
    private static float ReportDelay { get => CustomOptionHolder.BaitReportDelay.GetFloat(); }
    internal static bool ShowKillFlash { get => CustomOptionHolder.BaitShowKillFlash.GetBool(); }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        if (Player == null)
        {
            return;
        }

        // Bait report
        if (!Player.Data.IsDead || _reported)
        {
            return;
        }
        DeadPlayer baitDeadPlayer = null;
        var deadPlayers = GameHistory.DeadPlayers;
        if (deadPlayers != null)
        {
            foreach (var dp in deadPlayers)
            {
                if (dp.Player == null || dp.Player.PlayerId != Player.PlayerId)
                {
                    continue;
                }
                baitDeadPlayer = dp;
                break;
            }
        }

        if (baitDeadPlayer != null && baitDeadPlayer.KillerIfExisting != null)
        {
            // Show warning to the killer
            if (baitDeadPlayer.KillerIfExisting == PlayerControl.LocalPlayer)
            {
                _warningMessage ??= new("BaitWarning", _delay);
            }
        }

        if (Player != PlayerControl.LocalPlayer)
        {
            return;
        }

        _delay -= Time.fixedDeltaTime;

        if (baitDeadPlayer == null || baitDeadPlayer.KillerIfExisting == null || !(_delay <= 0f))
        {
            return;
        }
        Helpers.HandleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called

        var reporter = baitDeadPlayer.KillerIfExisting.PlayerId;
        if (Player.HasModifier(ModifierType.Madmate))
        {
            List<PlayerControl> candidates = [];
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p != null && p.IsAlive() && !p.IsTeamImpostor() && !p.isDummy)
                {
                    candidates.Add(p);
                }
            }

            if (candidates.Count > 0)
            {
                var i = RebuildUs.Rnd.Next(0, candidates.Count);
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

    internal static void Clear()
    {
        Players.Clear();
    }
}