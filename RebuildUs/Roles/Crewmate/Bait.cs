namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Bait : RoleBase<Bait>
{
    public static Color NameColor = new Color32(0, 247, 255, byte.MaxValue);
    public override Color RoleColor => NameColor;

    // write configs here
    public static bool highlightAllVents { get { return CustomOptionHolder.baitHighlightAllVents.GetBool(); } }
    public static float reportDelay { get { return CustomOptionHolder.baitReportDelay.GetFloat(); } }
    public static bool showKillFlash { get { return CustomOptionHolder.baitShowKillFlash.GetBool(); } }

    public bool reported = false;
    public float delay = 1f;

    public Bait()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Bait;
        delay = reportDelay;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (!CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Bait)) return;

        // Bait report
        if (Player.Data.IsDead && !reported)
        {
            delay -= Time.fixedDeltaTime;
            var deadPlayer = GameHistory.DeadPlayers?.Where(x => x.Player.IsRole(RoleType.Bait))?.FirstOrDefault();
            if (deadPlayer.KillerIfExisting != null && Bait.reportDelay <= 0f)
            {
                Helpers.handleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called

                byte reporter = deadPlayer.KillerIfExisting.PlayerId;
                if (Player.HasModifier(ModifierType.Madmate))
                {
                    var candidates = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => x.IsAlive() && !x.IsTeamImpostor() && !x.isDummy).ToList();
                    int i = RebuildUs.Instance.Rnd.Next(0, candidates.Count);
                    reporter = candidates.Count > 0 ? candidates[i].PlayerId : deadPlayer.KillerIfExisting.PlayerId;
                }

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.UncheckedCmdReportDeadBody, Hazel.SendOption.Reliable, -1);
                writer.Write(reporter);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.UncheckedCmdReportDeadBody(reporter, Player.PlayerId);
                reported = true;
            }
        }

        // Bait Vents
        if (MapUtilities.CachedShipStatus?.AllVents != null)
        {
            var ventsWithPlayers = new List<int>();
            foreach (PlayerControl player in CachedPlayer.AllPlayers)
            {
                if (player == null) continue;

                if (player.inVent)
                {
                    Vent target = MapUtilities.CachedShipStatus.AllVents.OrderBy(x => Vector2.Distance(x.transform.position, player.GetTruePosition())).FirstOrDefault();
                    if (target != null) ventsWithPlayers.Add(target.Id);
                }
            }

            foreach (Vent vent in MapUtilities.CachedShipStatus.AllVents)
            {
                if (vent.myRend == null || vent.myRend.material == null) continue;
                if (ventsWithPlayers.Contains(vent.Id) || (ventsWithPlayers.Count > 0 && Bait.highlightAllVents))
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
    public override void MakeButtons(HudManager hm) { }
    public override void SetButtonCooldowns() { }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}