namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Snitch, RoleTeam.Crewmate, typeof(SingleRoleBase<Snitch>), nameof(CustomOptionHolder.SnitchSpawnRate))]
internal class Snitch : SingleRoleBase<Snitch>
{
    public static Color Color = new Color32(184, 251, 79, byte.MaxValue);

    internal static List<Arrow> LocalArrows = [];

    public Snitch()
    {
        StaticRoleType = CurrentRoleType = RoleType.Snitch;
    }

    internal static int LeftTasksForReveal { get => Mathf.RoundToInt(CustomOptionHolder.SnitchLeftTasksForReveal.GetFloat()); }
    internal static bool IncludeTeamJackal { get => CustomOptionHolder.SnitchIncludeTeamJackal.GetBool(); }
    internal static bool TeamJackalUseDifferentArrowColor { get => CustomOptionHolder.SnitchTeamJackalUseDifferentArrowColor.GetBool(); }

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        if (LocalArrows == null || Player.IsDead())
        {
            return;
        }

        for (var i = 0; i < LocalArrows.Count; i++)
        {
            LocalArrows[i]?.ArrowObject.SetActive(false);
        }

        var (playerCompleted, playerTotal) = TasksHandler.TaskInfo(Player.Data);
        var taskRemain = playerTotal - playerCompleted;

        if (taskRemain <= LeftTasksForReveal && (PlayerControl.LocalPlayer.Data.Role.IsImpostor || IncludeTeamJackal && (PlayerControl.LocalPlayer.IsRole(RoleType.Jackal) || PlayerControl.LocalPlayer.IsRole(RoleType.Sidekick))))
        {
            if (LocalArrows.Count == 0)
            {
                LocalArrows.Add(new(Color));
            }
            if (LocalArrows.Count != 0 && LocalArrows[0] != null)
            {
                LocalArrows[0].ArrowObject.SetActive(true);
                LocalArrows[0].Image.color = Color;
                LocalArrows[0].Update(Player.transform.position);
            }
        }
        else if (PlayerControl.LocalPlayer.IsRole(RoleType.Snitch) && taskRemain == 0)
        {
            var arrowIndex = 0;
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p == null || p.Data == null)
                {
                    continue;
                }

                var arrowForImp = p.IsTeamImpostor();
                var arrowForTeamJackal = IncludeTeamJackal && (p.IsRole(RoleType.Jackal) || p.IsRole(RoleType.Sidekick));

                if (!p.Data.IsDead && (arrowForImp || arrowForTeamJackal))
                {
                    var color = Palette.ImpostorRed;
                    if (arrowForTeamJackal && TeamJackalUseDifferentArrowColor)
                    {
                        color = Jackal.Color;
                    }

                    if (arrowIndex >= LocalArrows.Count)
                    {
                        LocalArrows.Add(new(color));
                    }

                    var arrow = LocalArrows[arrowIndex];
                    if (arrow != null)
                    {
                        arrow.Image.color = color;
                        arrow.ArrowObject.SetActive(true);
                        arrow.Update(p.transform.position, color);
                    }

                    arrowIndex++;
                }
            }
        }
    }

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;

        if (LocalArrows != null)
        {
            foreach (var arrow in LocalArrows)
            {
                if (arrow?.ArrowObject != null)
                {
                    UnityObject.Destroy(arrow.ArrowObject);
                }
            }
        }

        LocalArrows = [];
    }
}