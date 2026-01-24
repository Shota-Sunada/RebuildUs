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
        if (Player == null || Player != PlayerControl.LocalPlayer) return;

        // Bait report
        if (Player.Data.IsDead && !Reported)
        {
            Delay -= Time.fixedDeltaTime;

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

            if (baitDeadPlayer != null && baitDeadPlayer.KillerIfExisting != null && Delay <= 0f)
            {
                Helpers.HandleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called

                byte reporter = baitDeadPlayer.KillerIfExisting.PlayerId;
                if (Player.HasModifier(ModifierType.Madmate))
                {
                    var candidates = new List<PlayerControl>();
                    var allPlayers = PlayerControl.AllPlayerControls;
                    for (int i = 0; i < allPlayers.Count; i++)
                    {
                        var p = allPlayers[i];
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
            }
        }

        // Bait Vents
        var shipStatus = MapUtilities.CachedShipStatus;
        if (shipStatus != null && shipStatus.AllVents != null)
        {
            var ventsWithPlayers = new HashSet<int>();
            var allPlayers = PlayerControl.AllPlayerControls;
            for (int i = 0; i < allPlayers.Count; i++)
            {
                PlayerControl player = allPlayers[i];
                if (player == null || !player.inVent) continue;

                var playerPos = player.GetTruePosition();
                Vent closestVent = null;
                float minDistance = float.MaxValue;
                var allVents = shipStatus.AllVents;

                for (int j = 0; j < allVents.Length; j++)
                {
                    var v = allVents[j];
                    if (v == null) continue;
                    float dist = Vector2.Distance(v.transform.position, playerPos);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closestVent = v;
                    }
                }

                if (closestVent != null) ventsWithPlayers.Add(closestVent.Id);
            }

            var allVentsForHighlight = shipStatus.AllVents;
            bool highlightAll = Bait.HighlightAllVents && ventsWithPlayers.Count > 0;
            for (int i = 0; i < allVentsForHighlight.Length; i++)
            {
                Vent vent = allVentsForHighlight[i];
                if (vent == null || vent.myRend == null) continue;

                if (highlightAll || ventsWithPlayers.Contains(vent.Id))
                {
                    vent.myRend.material.SetFloat("_Outline", 1f);
                    vent.myRend.material.SetColor("_OutlineColor", Color.yellow);
                }
                else
                {
                    vent.myRend.material.SetFloat("_Outline", 0);
                }
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