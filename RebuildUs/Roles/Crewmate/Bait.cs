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
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Bait)) return;

        // Bait report
        if (Player.Data.IsDead && !Reported)
        {
            Delay -= Time.fixedDeltaTime;
            var deadPlayer = GameHistory.DeadPlayers?.Where(x => x.Player.IsRole(RoleType.Bait))?.FirstOrDefault();
            if (deadPlayer.KillerIfExisting != null && Bait.ReportDelay <= 0f)
            {
                Helpers.HandleVampireBiteOnBodyReport(); // Manually call Vampire handling, since the CmdReportDeadBody Prefix won't be called

                byte reporter = deadPlayer.KillerIfExisting.PlayerId;
                if (Player.HasModifier(ModifierType.Madmate))
                {
                    var candidates = PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => x.IsAlive() && !x.IsTeamImpostor() && !x.isDummy).ToList();
                    int i = RebuildUs.Instance.Rnd.Next(0, candidates.Count);
                    reporter = candidates.Count > 0 ? candidates[i].PlayerId : deadPlayer.KillerIfExisting.PlayerId;
                }

                {
                    using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedCmdReportDeadBody);
                    sender.Write(reporter);
                    sender.Write(Player.PlayerId);
                }
                RPCProcedure.UncheckedCmdReportDeadBody(reporter, Player.PlayerId);
                Reported = true;
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
                if (ventsWithPlayers.Contains(vent.Id) || (ventsWithPlayers.Count > 0 && Bait.HighlightAllVents))
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

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}