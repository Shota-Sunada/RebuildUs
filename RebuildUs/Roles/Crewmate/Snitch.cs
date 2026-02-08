using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Snitch : RoleBase<Snitch>
{
    public static Color NameColor = new Color32(184, 251, 79, byte.MaxValue);

    // write configs here
    public static List<Arrow> LocalArrows = [];

    public Snitch()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Snitch;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    public static int LeftTasksForReveal
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SnitchLeftTasksForReveal.GetFloat());
    }

    public static bool IncludeTeamJackal
    {
        get => CustomOptionHolder.SnitchIncludeTeamJackal.GetBool();
    }

    public static bool TeamJackalUseDifferentArrowColor
    {
        get => CustomOptionHolder.SnitchTeamJackalUseDifferentArrowColor.GetBool();
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }

    public override void FixedUpdate()
    {
        if (LocalArrows == null) return;

        for (var i = 0; i < LocalArrows.Count; i++) LocalArrows[i]?.ArrowObject.SetActive(false);

        if (Player.Data.IsDead) return;

        var (playerCompleted, playerTotal) = TasksHandler.TaskInfo(Player.Data);
        var numberOfTasks = playerTotal - playerCompleted;

        var localPlayer = PlayerControl.LocalPlayer;
        if (numberOfTasks <= LeftTasksForReveal && (localPlayer.Data.Role.IsImpostor || (IncludeTeamJackal && (localPlayer.IsRole(RoleType.Jackal) || localPlayer.IsRole(RoleType.Sidekick)))))
        {
            if (LocalArrows.Count == 0) LocalArrows.Add(new(Color.blue));
            if (LocalArrows.Count != 0 && LocalArrows[0] != null)
            {
                LocalArrows[0].ArrowObject.SetActive(true);
                LocalArrows[0].Image.color = Color.blue;
                LocalArrows[0].Update(Player.transform.position);
            }
        }
        else if (localPlayer.IsRole(RoleType.Snitch) && numberOfTasks == 0)
        {
            var arrowIndex = 0;
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p == null || p.Data == null) continue;

                var arrowForImp = p.Data.Role.IsImpostor;
                var arrowForTeamJackal = IncludeTeamJackal && (p.IsRole(RoleType.Jackal) || p.IsRole(RoleType.Sidekick));

                if (!p.Data.IsDead && (arrowForImp || arrowForTeamJackal))
                {
                    var c = arrowForTeamJackal ? Jackal.NameColor : Palette.ImpostorRed;

                    if (arrowIndex >= LocalArrows.Count) LocalArrows.Add(new(c));
                    var arrow = LocalArrows[arrowIndex];
                    if (arrow != null)
                    {
                        arrow.Image.color = c;
                        arrow.ArrowObject.SetActive(true);
                        arrow.Update(p.transform.position, c);
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
                    Object.Destroy(arrow.ArrowObject);
            }
        }

        LocalArrows = [];
    }
}
