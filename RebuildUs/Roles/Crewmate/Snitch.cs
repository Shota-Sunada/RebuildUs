namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Snitch : RoleBase<Snitch>
{
    public static Color NameColor = new Color32(184, 251, 79, byte.MaxValue);
    public override Color RoleColor => Snitch.NameColor;

    // write configs here
    public static List<Arrow> LocalArrows = [];

    public static int LeftTasksForReveal { get { return Mathf.RoundToInt(CustomOptionHolder.SnitchLeftTasksForReveal.GetFloat()); } }
    public static bool IncludeTeamJackal { get { return CustomOptionHolder.SnitchIncludeTeamJackal.GetBool(); } }
    public static bool TeamJackalUseDifferentArrowColor { get { return CustomOptionHolder.SnitchTeamJackalUseDifferentArrowColor.GetBool(); } }

    public Snitch()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Snitch;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (LocalArrows == null) return;

        foreach (var arrow in LocalArrows) arrow.ArrowObject.SetActive(false);

        if (Player.Data.IsDead) return;

        var (playerCompleted, playerTotal) = TasksHandler.TaskInfo(Player.Data);
        int numberOfTasks = playerTotal - playerCompleted;

        if (numberOfTasks <= LeftTasksForReveal && (PlayerControl.LocalPlayer.Data.Role.IsImpostor || (IncludeTeamJackal && (PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) || PlayerControl.LocalPlayer.IsRole(RoleType.Sidekick)))))
        {
            if (LocalArrows.Count == 0) LocalArrows.Add(new Arrow(Color.blue));
            if (LocalArrows.Count != 0 && LocalArrows[0] != null)
            {
                LocalArrows[0].ArrowObject.SetActive(true);
                LocalArrows[0].Image.color = Color.blue;
                LocalArrows[0].Update(Player.transform.position);
            }
        }
        else if (PlayerControl.LocalPlayer.IsRole(RoleType.Snitch) && numberOfTasks == 0)
        {
            int arrowIndex = 0;
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                bool arrowForImp = p.Data.Role.IsImpostor;
                bool arrowForTeamJackal = IncludeTeamJackal && (p.IsRole(RoleType.Jackal) || p.IsRole(RoleType.Sidekick));

                // Update the arrows' color every time bc things go weird when you add a sidekick or someone dies
                Color c = Palette.ImpostorRed;
                if (arrowForTeamJackal)
                {
                    c = Jackal.NameColor;
                }
                if (!p.Data.IsDead && (arrowForImp || arrowForTeamJackal))
                {
                    if (arrowIndex >= LocalArrows.Count)
                    {
                        LocalArrows.Add(new Arrow(c));
                    }
                    if (arrowIndex < LocalArrows.Count && LocalArrows[arrowIndex] != null)
                    {
                        LocalArrows[arrowIndex].Image.color = c;
                        LocalArrows[arrowIndex].ArrowObject.SetActive(true);
                        LocalArrows[arrowIndex].Update(p.transform.position, c);
                    }
                    arrowIndex++;
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

        if (LocalArrows != null)
        {
            foreach (var arrow in LocalArrows)
            {
                if (arrow?.ArrowObject != null)
                {
                    UnityEngine.Object.Destroy(arrow.ArrowObject);
                }
            }
        }
        LocalArrows = [];
    }
}