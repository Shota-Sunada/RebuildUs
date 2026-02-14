namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Bait : RoleBase<Bait>
{
    public static Color NameColor = new Color32(0, 247, 255, byte.MaxValue);
    public override Color RoleColor => NameColor;

    // write configs here
    public static bool HighlightAllVents { get { return CustomOptionHolder.BaitHighlightAllVents.GetBool(); } }
    public static float ReportDelay { get { return CustomOptionHolder.BaitReportDelay.GetFloat(); } }
    public static bool ShowKillFlash { get { return CustomOptionHolder.BaitShowKillFlash.GetBool(); } }

    public bool Reported = false;
    public float Delay = 1f;
    public CustomMessage WarningMessage;

    public Bait()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Bait;
        Delay = ReportDelay;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (Player == null) return;

        // Bait report
        if (Player.Data.IsDead && !Reported)
        {
            DeadPlayer baitDeadPlayer = null;
            var deadPlayers = GameHistory.DeadPlayers;
            if (deadPlayers != null)
            {
                for (int i = 0; i < deadPlayers.Count; i++)
                {
                    var dp = deadPlayers[i];
                    if (dp.Player != null && dp.Player.PlayerId == Player.PlayerId)
                    {
                        baitDeadPlayer = dp;
                        break;
                    }
                }
            }

            if (baitDeadPlayer != null && baitDeadPlayer.KillerIfExisting != null)
            {
                // Show warning to the killer
                if (baitDeadPlayer.KillerIfExisting == PlayerControl.LocalPlayer)
                {
                    WarningMessage ??= new("BaitWarning", Delay, new(0f, -1.8f), MessageType.Normal);
                }
            }

            if (Player != PlayerControl.LocalPlayer) return;

            Delay -= Time.fixedDeltaTime;

            if (baitDeadPlayer != null && baitDeadPlayer.KillerIfExisting != null && Delay <= 0f)
            {
                Helpers.HandleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called

                byte reporter = baitDeadPlayer.KillerIfExisting.PlayerId;
                if (Player.HasModifier(ModifierType.Madmate))
                {
                    var candidates = new List<PlayerControl>();
                    foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                    {
                        if (p != null && p.IsAlive() && !p.IsTeamImpostor() && !p.isDummy)
                        {
                            candidates.Add(p);
                        }
                    }

                    if (candidates.Count > 0)
                    {
                        int i = RebuildUs.Instance.Rnd.Next(0, candidates.Count);
                        reporter = candidates[i].PlayerId;
                    }
                }

                {
                    using var sender = new RPCSender(Player.NetId, CustomRPC.UncheckedCmdReportDeadBody);
                    sender.Write(reporter);
                    sender.Write(Player.PlayerId);
                }
                RPCProcedure.UncheckedCmdReportDeadBody(reporter, Player.PlayerId);
                Reported = true;
                WarningMessage = null;
            }
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}