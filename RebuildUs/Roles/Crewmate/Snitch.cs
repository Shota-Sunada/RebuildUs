namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Snitch : RoleBase<Snitch>
{
    public static Color NameColor = new Color32(184, 251, 79, byte.MaxValue);
    public override Color RoleColor => Snitch.NameColor;

    // write configs here
    public static List<Arrow> localArrows = [];

    public static int leftTasksForReveal { get { return Mathf.RoundToInt(CustomOptionHolder.snitchLeftTasksForReveal.GetFloat()); } }
    public static bool includeTeamJackal { get { return CustomOptionHolder.snitchIncludeTeamJackal.GetBool(); } }
    public static bool teamJackalUseDifferentArrowColor { get { return CustomOptionHolder.snitchTeamJackalUseDifferentArrowColor.GetBool(); } }

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
        if (localArrows == null) return;

        foreach (var arrow in localArrows) arrow.ArrowObject.SetActive(false);

        if (Player.Data.IsDead) return;

        var (playerCompleted, playerTotal) = TasksHandler.TaskInfo(Player.Data);
        int numberOfTasks = playerTotal - playerCompleted;

        if (numberOfTasks <= leftTasksForReveal && (CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor || (includeTeamJackal && (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Jackal) || CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Sidekick)))))
        {
            if (localArrows.Count == 0) localArrows.Add(new Arrow(Color.blue));
            if (localArrows.Count != 0 && localArrows[0] != null)
            {
                localArrows[0].ArrowObject.SetActive(true);
                localArrows[0].Image.color = Color.blue;
                localArrows[0].Update(Player.transform.position);
            }
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Snitch) && numberOfTasks == 0)
        {
            int arrowIndex = 0;
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                bool arrowForImp = p.Data.Role.IsImpostor;
                bool arrowForTeamJackal = includeTeamJackal && (p.IsRole(RoleType.Jackal) || p.IsRole(RoleType.Sidekick));

                // Update the arrows' color every time bc things go weird when you add a sidekick or someone dies
                Color c = Palette.ImpostorRed;
                if (arrowForTeamJackal)
                {
                    c = Jackal.NameColor;
                }
                if (!p.Data.IsDead && (arrowForImp || arrowForTeamJackal))
                {
                    if (arrowIndex >= localArrows.Count)
                    {
                        localArrows.Add(new Arrow(c));
                    }
                    if (arrowIndex < localArrows.Count && localArrows[arrowIndex] != null)
                    {
                        localArrows[arrowIndex].Image.color = c;
                        localArrows[arrowIndex].ArrowObject.SetActive(true);
                        localArrows[arrowIndex].Update(p.transform.position, c);
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

    public override void Clear()
    {
        // reset configs here
        Players.Clear();

        if (localArrows != null)
        {
            foreach (var arrow in localArrows)
            {
                if (arrow?.ArrowObject != null)
                {
                    UnityEngine.Object.Destroy(arrow.ArrowObject);
                }
            }
        }
    }
}