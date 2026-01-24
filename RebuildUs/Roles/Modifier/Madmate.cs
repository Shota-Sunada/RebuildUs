namespace RebuildUs.Roles.Modifier;

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

[HarmonyPatch]
public class Madmate : ModifierBase<Madmate>
{
    public static Color NameColor = Palette.ImpostorRed;
    public override Color ModifierColor => NameColor;

    public override void OnUpdateNameColors()
    {
        if (Player == PlayerControl.LocalPlayer)
        {
            Update.SetPlayerNameColor(Player, NameColor);

            if (KnowsImpostors(Player))
            {
                var allPlayers = PlayerControl.AllPlayerControls;
                for (var i = 0; i < allPlayers.Count; i++)
                {
                    var p = allPlayers[i];
                    if (p.IsTeamImpostor() || p.IsRole(RoleType.Spy) || (p.IsRole(RoleType.Jackal) && Jackal.GetRole(p).WasTeamRed) || (p.IsRole(RoleType.Sidekick) && Sidekick.GetRole(p).WasTeamRed))
                    {
                        Update.SetPlayerNameColor(p, Palette.ImpostorRed);
                    }
                }
            }
        }
    }

    // write configs here
    public static bool CanEnterVents { get { return CustomOptionHolder.MadmateCanEnterVents.GetBool(); } }
    public static bool HasImpostorVision { get { return CustomOptionHolder.MadmateHasImpostorVision.GetBool(); } }
    public static bool CanSabotage { get { return CustomOptionHolder.MadmateCanSabotage.GetBool(); } }
    public static bool CanFixComm { get { return CustomOptionHolder.MadmateCanFixComm.GetBool(); } }

    public static MadmateType MadmateType { get { return (MadmateType)CustomOptionHolder.MadmateType.GetSelection(); } }
    public static MadmateAbility MadmateAbility { get { return (MadmateAbility)CustomOptionHolder.MadmateAbility.GetSelection(); } }
    public static RoleType FixedRole { get { return CustomOptionHolder.MadmateFixedRole.Role; } }

    public static int NumCommonTasks { get { return CustomOptionHolder.MadmateTasks.CommonTasksNum; } }
    public static int NumLongTasks { get { return CustomOptionHolder.MadmateTasks.LongTasksNum; } }
    public static int NumShortTasks { get { return CustomOptionHolder.MadmateTasks.ShortTasksNum; } }

    public static bool HasTasks { get { return MadmateAbility == MadmateAbility.Fanatic; } }
    public static bool ExileCrewmate { get { return CustomOptionHolder.MadmateExilePlayer.GetBool(); } }

    public static string Prefix
    {
        get
        {
            return Tr.Get("Hud.MadmatePrefix");
        }
    }

    public static string FullName
    {
        get
        {
            return Tr.Get("Modifier.Madmate");
        }
    }

    public static RoleType[] ValidRoles = [
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
        // RoleType.Watcher,
    ];

    public static List<PlayerControl> Candidates
    {
        get
        {
            List<PlayerControl> crewHasRole = [];
            List<PlayerControl> crewNoRole = [];
            List<PlayerControl> validCrewmates = [];

            var allPlayers = PlayerControl.AllPlayerControls;
            for (var i = 0; i < allPlayers.Count; i++)
            {
                var player = allPlayers[i];
                if (player.IsTeamCrewmate() && !HasModifier(player))
                {
                    var info = RoleInfo.GetRoleInfoForPlayer(player);
                    // if (info.Contains(RoleInfo.Crewmate) && !player.HasModifier(ModifierType.Munou) && !player.IsRole(RoleType.FortuneTeller))

                    bool isCrewmateOnly = false;
                    bool isFortuneTeller = false;
                    bool hasValidRole = false;
                    bool hasFixedRole = false;

                    for (var j = 0; j < info.Count; j++)
                    {
                        var ri = info[j];
                        if (ri.RoleType == RoleType.Crewmate) isCrewmateOnly = true;
                        if (ri.RoleType == RoleType.FortuneTeller) isFortuneTeller = true;

                        for (var k = 0; k < ValidRoles.Length; k++)
                        {
                            if (ri.RoleType == ValidRoles[k])
                            {
                                hasValidRole = true;
                                break;
                            }
                        }

                        if (ri.RoleType == FixedRole) hasFixedRole = true;
                    }

                    if (isCrewmateOnly && !isFortuneTeller)
                    {
                        crewNoRole.Add(player);
                        validCrewmates.Add(player);
                    }
                    else if (hasValidRole)
                    {
                        if (FixedRole == RoleType.NoRole || hasFixedRole)
                        {
                            crewHasRole.Add(player);
                        }

                        validCrewmates.Add(player);
                    }
                }
            }

            if (MadmateType == MadmateType.Simple) return crewNoRole;
            else if (MadmateType == MadmateType.WithRole && crewHasRole.Count > 0) return crewHasRole;
            else if (MadmateType == MadmateType.Random) return validCrewmates;
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
        PlayerControlHelpers.GenerateAndAssignTasks(Player, NumCommonTasks, NumShortTasks, NumLongTasks);
    }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    public static bool KnowsImpostors(PlayerControl player)
    {
        return HasTasks && HasModifier(player) && TasksComplete(player);
    }

    public static bool TasksComplete(PlayerControl player)
    {
        if (!HasTasks) return false;

        int counter = 0;
        int totalTasks = NumCommonTasks + NumLongTasks + NumShortTasks;
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

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}