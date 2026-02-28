namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Snitch, RoleTeam.Crewmate, typeof(SingleRoleBase<Snitch>), nameof(Snitch.NameColor), nameof(CustomOptionHolder.SnitchSpawnRate))]
internal class Snitch : SingleRoleBase<Snitch>
{
    internal static Color NameColor = new Color32(184, 251, 79, byte.MaxValue);

    // write configs here
    internal static List<Arrow> LocalArrows = [];

    public Snitch()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Snitch;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    internal static int LeftTasksForReveal
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SnitchLeftTasksForReveal.GetFloat());
    }

    internal static bool IncludeTeamJackal
    {
        get => CustomOptionHolder.SnitchIncludeTeamJackal.GetBool();
    }

    internal static bool TeamJackalUseDifferentArrowColor
    {
        get => CustomOptionHolder.SnitchTeamJackalUseDifferentArrowColor.GetBool();
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (LocalArrows == null)
        {
            return;
        }

        for (var i = 0; i < LocalArrows.Count; i++)
        {
            LocalArrows[i]?.ArrowObject.SetActive(false);
        }

        if (Player.Data.IsDead)
        {
            return;
        }

        (var playerCompleted, var playerTotal) = TasksHandler.TaskInfo(Player.Data);
        var numberOfTasks = playerTotal - playerCompleted;

        var localPlayer = PlayerControl.LocalPlayer;
        if (numberOfTasks <= LeftTasksForReveal
            && (localPlayer.Data.Role.IsImpostor
                || IncludeTeamJackal && (localPlayer.IsRole(RoleType.Jackal) || localPlayer.IsRole(RoleType.Sidekick))))
        {
            if (LocalArrows.Count == 0)
            {
                LocalArrows.Add(new(Color.blue));
            }
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
                if (p == null || p.Data == null)
                {
                    continue;
                }

                var arrowForImp = p.Data.Role.IsImpostor;
                var arrowForTeamJackal = IncludeTeamJackal && (p.IsRole(RoleType.Jackal) || p.IsRole(RoleType.Sidekick));

                if (!p.Data.IsDead && (arrowForImp || arrowForTeamJackal))
                {
                    var c = arrowForTeamJackal ? Jackal.NameColor : Palette.ImpostorRed;

                    if (arrowIndex >= LocalArrows.Count)
                    {
                        LocalArrows.Add(new(c));
                    }

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

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

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