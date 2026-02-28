namespace RebuildUs.Roles.Modifier;

internal enum MadmateType
{
    Simple = 0,
    WithRole = 1,
    Random = 2,
}

internal enum MadmateAbility
{
    None = 0,
    Fanatic = 1,
}

[HarmonyPatch]
[RegisterModifier(ModifierType.Madmate, typeof(Madmate), nameof(NameColor), nameof(CustomOptionHolder.MadmateSpawnRate))]
internal class Madmate : ModifierBase<Madmate>
{
    internal static Color NameColor = Palette.ImpostorRed;

    internal static RoleType[] ValidRoles =
    [
        RoleType.NoRole, // NoRole = off
        RoleType.Shifter,
        RoleType.Mayor,
        RoleType.Engineer,
        RoleType.Sheriff,
        RoleType.Lighter,
        RoleType.Detective,
        RoleType.TimeMaster,
        RoleType.Medic,
        RoleType.NiceSwapper,
        RoleType.Seer,
        RoleType.Hacker,
        RoleType.Tracker,
        RoleType.SecurityGuard,
        RoleType.Bait,
        RoleType.Medium,
        RoleType.NiceGuesser,
        // RoleType.Watcher,
    ];

    public Madmate()
    {
        // write value init here
        StaticModifierType = CurrentModifierType = ModifierType.Madmate;
    }

    internal override Color ModifierColor
    {
        get => NameColor;
    }

    // write configs here
    internal static bool CanEnterVents
    {
        get => CustomOptionHolder.MadmateCanEnterVents.GetBool();
    }

    internal static bool HasImpostorVision
    {
        get => CustomOptionHolder.MadmateHasImpostorVision.GetBool();
    }

    internal static bool CanSabotage
    {
        get => CustomOptionHolder.MadmateCanSabotage.GetBool();
    }

    internal static bool CanFixComm
    {
        get => CustomOptionHolder.MadmateCanFixComm.GetBool();
    }

    internal static MadmateType MadmateType
    {
        get => (MadmateType)CustomOptionHolder.MadmateType.GetSelection();
    }

    internal static MadmateAbility MadmateAbility
    {
        get => (MadmateAbility)CustomOptionHolder.MadmateAbility.GetSelection();
    }

    internal static RoleType FixedRole
    {
        get => CustomOptionHolder.MadmateFixedRole.Role;
    }

    internal static int NumCommonTasks
    {
        get => CustomOptionHolder.MadmateTasks.CommonTasksNum;
    }

    internal static int NumLongTasks
    {
        get => CustomOptionHolder.MadmateTasks.LongTasksNum;
    }

    internal static int NumShortTasks
    {
        get => CustomOptionHolder.MadmateTasks.ShortTasksNum;
    }

    internal static bool HasTasks
    {
        get => MadmateAbility == MadmateAbility.Fanatic;
    }

    internal static bool ExileCrewmate
    {
        get => CustomOptionHolder.MadmateExilePlayer.GetBool();
    }

    internal static string Prefix
    {
        get => Tr.Get(TrKey.MadmatePrefix);
    }

    internal static string FullName
    {
        get => Tr.Get(TrKey.Madmate);
    }

    internal static List<PlayerControl> Candidates
    {
        get
        {
            List<PlayerControl> crewHasRole = [];
            List<PlayerControl> crewNoRole = [];
            List<PlayerControl> validCrewmates = [];

            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (player.IsTeamCrewmate() && !HasModifier(player))
                {
                    var info = RoleInfo.GetRoleInfoForPlayer(player);
                    // if (info.Contains(RoleInfo.Crewmate) && !player.HasModifier(ModifierType.Munou) && !player.IsRole(RoleType.FortuneTeller))

                    var isCrewmateOnly = false;
                    var isFortuneTeller = false;
                    var hasValidRole = false;
                    var hasFixedRole = false;

                    for (var j = 0; j < info.Count; j++)
                    {
                        var ri = info[j];
                        if (ri.RoleType == RoleType.Crewmate)
                        {
                            isCrewmateOnly = true;
                        }
                        if (ri.RoleType == RoleType.FortuneTeller)
                        {
                            isFortuneTeller = true;
                        }

                        for (var k = 0; k < ValidRoles.Length; k++)
                        {
                            if (ri.RoleType == ValidRoles[k])
                            {
                                hasValidRole = true;
                                break;
                            }
                        }

                        if (ri.RoleType == FixedRole)
                        {
                            hasFixedRole = true;
                        }
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

            if (MadmateType == MadmateType.Simple)
            {
                return crewNoRole;
            }
            if (MadmateType == MadmateType.WithRole && crewHasRole.Count > 0)
            {
                return crewHasRole;
            }
            if (MadmateType == MadmateType.Random)
            {
                return validCrewmates;
            }
            return validCrewmates;
        }
    }

    internal override void OnUpdateNameColors()
    {
        if (Player == PlayerControl.LocalPlayer)
        {
            HudManagerPatch.SetPlayerNameColor(Player, NameColor);

            if (KnowsImpostors(Player))
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.IsTeamImpostor()
                        || p.IsRole(RoleType.Spy)
                        || p.IsRole(RoleType.Jackal) && Jackal.Instance.WasTeamRed
                        || p.IsRole(RoleType.Sidekick) && Sidekick.Instance.WasTeamRed)
                    {
                        HudManagerPatch.SetPlayerNameColor(p, Palette.ImpostorRed);
                    }
                }
            }
        }
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }
    internal override void FixedUpdate() { }
    internal override void OnKill(PlayerControl target) { }

    internal override void OnDeath(PlayerControl killer = null)
    {
        Player.ClearAllTasks();
    }

    internal override void OnFinishShipStatusBegin()
    {
        Player.ClearAllTasks();
        Player.GenerateAndAssignTasks(NumCommonTasks, NumShortTasks, NumLongTasks);
    }

    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    // write functions here

    internal static bool KnowsImpostors(PlayerControl player)
    {
        return HasTasks && HasModifier(player) && TasksComplete(player);
    }

    internal static bool TasksComplete(PlayerControl player)
    {
        if (!HasTasks)
        {
            return false;
        }

        var counter = 0;
        var totalTasks = NumCommonTasks + NumLongTasks + NumShortTasks;
        if (totalTasks == 0)
        {
            return true;
        }
        foreach (var task in player.Data.Tasks)
        {
            if (task.Complete)
            {
                counter++;
            }
        }

        return counter == totalTasks;
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}