namespace RebuildUs.Roles.Modifier;

[HarmonyPatch]
public class Madmate : ModifierBase<Madmate>
{
    public static Color ModifierColor = Palette.CrewmateBlue;

    public enum MadmateType
    {
        Simple = 0,
        WithRole = 1,
        Random = 2,
    }

    public enum MadmateAbility
    {
        None = 0,
        Fanatic = 1,
    }

    // write configs here
    public static bool canEnterVents { get { return CustomOptionHolder.madmateCanEnterVents.GetBool(); } }
    public static bool hasImpostorVision { get { return CustomOptionHolder.madmateHasImpostorVision.GetBool(); } }
    public static bool canSabotage { get { return CustomOptionHolder.madmateCanSabotage.GetBool(); } }
    public static bool canFixComm { get { return CustomOptionHolder.madmateCanFixComm.GetBool(); } }

    public static MadmateType madmateType { get { return (MadmateType)CustomOptionHolder.madmateType.GetSelection(); } }
    public static MadmateAbility madmateAbility { get { return (MadmateAbility)CustomOptionHolder.madmateAbility.GetSelection(); } }
    public static RoleType fixedRole { get { return CustomOptionHolder.madmateFixedRole.Role; } }

    public static int numCommonTasks { get { return CustomOptionHolder.madmateTasks.CommonTasksNum; } }
    public static int numLongTasks { get { return CustomOptionHolder.madmateTasks.LongTasksNum; } }
    public static int numShortTasks { get { return CustomOptionHolder.madmateTasks.ShortTasksNum; } }

    public static bool hasTasks { get { return madmateAbility == MadmateAbility.Fanatic; } }
    public static bool exileCrewmate { get { return CustomOptionHolder.madmateExilePlayer.GetBool(); } }

    public static string prefix
    {
        get
        {
            return Tr.Get("madmatePrefix");
        }
    }

    public static string fullName
    {
        get
        {
            return Tr.Get("madmate");
        }
    }

    public static RoleType[] validRoles = [
        RoleType.NoRole, // NoRole = off
        RoleType.Shifter,
        RoleType.Mayor,
        RoleType.Engineer,
        RoleType.Sheriff,
        RoleType.Lighter,
        RoleType.Detective,
        RoleType.TimeMaster,
        RoleType.Medic,
        RoleType.Swapper,
        RoleType.Seer,
        RoleType.Hacker,
        RoleType.Tracker,
        RoleType.SecurityGuard,
        RoleType.Bait,
        RoleType.Medium,
        RoleType.NiceGuesser,
        RoleType.Watcher,
    ];

    public static List<PlayerControl> candidates
    {
        get
        {
            List<PlayerControl> crewHasRole = [];
            List<PlayerControl> crewNoRole = [];
            List<PlayerControl> validCrewmates = [];

            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator().ToArray().Where(x => x.IsTeamCrewmate() && !HasModifier(x)).ToList())
            {
                var info = RoleInfo.GetRoleInfoForPlayer(player);
                if (info.Contains(RoleInfo.Crewmate) && !player.HasModifier(ModifierType.Munou) && !player.IsRole(RoleType.FortuneTeller))
                {
                    crewNoRole.Add(player);
                    validCrewmates.Add(player);
                }
                else if (info.Any(x => validRoles.Contains(x.RoleType)))
                {
                    if (fixedRole == RoleType.NoRole || info.Any(x => x.RoleType == fixedRole))
                    {
                        crewHasRole.Add(player);
                    }

                    validCrewmates.Add(player);
                }
            }

            if (madmateType == MadmateType.Simple) return crewNoRole;
            else if (madmateType == MadmateType.WithRole && crewHasRole.Count > 0) return crewHasRole;
            else if (madmateType == MadmateType.Random) return validCrewmates;
            return validCrewmates;
        }
    }

    public Madmate()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.Madmate;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate() { }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null)
    {
        Player.ClearAllTasks();
    }
    public override void OnFinishShipStatusBegin()
    {
        Player.ClearAllTasks();
        Player.GenerateAndAssignTasks(numCommonTasks, numShortTasks, numLongTasks);
    }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm) { }
    public override void SetButtonCooldowns() { }

    // write functions here

    public static bool knowsImpostors(PlayerControl player)
    {
        return hasTasks && HasModifier(player) && tasksComplete(player);
    }

    public static bool tasksComplete(PlayerControl player)
    {
        if (!hasTasks) return false;

        int counter = 0;
        int totalTasks = numCommonTasks + numLongTasks + numShortTasks;
        if (totalTasks == 0) return true;
        foreach (var task in player.Data.Tasks)
        {
            if (task.Complete)
            {
                counter++;
            }
        }
        return counter == totalTasks;
    }

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}