namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Engineer, RoleTeam.Crewmate, typeof(MultiRoleBase<Engineer>), nameof(CustomOptionHolder.EngineerSpawnRate))]
internal class Engineer : MultiRoleBase<Engineer>
{
    internal static Color Color = new Color32(0, 40, 245, byte.MaxValue);

    private static CustomButton _engineerRepairButton;

    // write configs here
    internal int RemainingFixes = 1;

    public Engineer()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Engineer;
        RemainingFixes = NumberOfFixes;
    }

    private static int NumberOfFixes
    {
        get => (int)CustomOptionHolder.EngineerNumberOfFixes.GetFloat();
    }

    internal static bool HighlightForImpostors
    {
        get => CustomOptionHolder.EngineerHighlightForImpostors.GetBool();
    }

    internal static bool HighlightForTeamJackal
    {
        get => CustomOptionHolder.EngineerHighlightForTeamJackal.GetBool();
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _engineerRepairButton = new(OnClick,
            HasButton,
            CouldUse,
            () => { },
            AssetLoader.RepairButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.RepairText));
    }

    private static bool CouldUse()
    {
        var sabotageActive = false;
        foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
        {
            if (task.TaskType is not (TaskTypes.FixLights
                                      or TaskTypes.RestoreOxy
                                      or TaskTypes.ResetReactor
                                      or TaskTypes.ResetSeismic
                                      or TaskTypes.FixComms
                                      or TaskTypes.StopCharles)
                && (!SubmergedCompatibility.IsSubmerged || task.TaskType != SubmergedCompatibility.RetrieveOxygenMask))
            {
                continue;
            }

            sabotageActive = true;
            break;
        }

        return sabotageActive && Local.RemainingFixes > 0 && PlayerControl.LocalPlayer.CanMove;
    }

    private static bool HasButton()
    {
        return PlayerControl.LocalPlayer.IsRole(RoleType.Engineer) && Local.RemainingFixes > 0 && PlayerControl.LocalPlayer.IsAlive();
    }

    private static void OnClick()
    {
        _engineerRepairButton.Timer = 0f;

        EngineerUsedRepair(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.PlayerId);

        foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
        {
            switch (task.TaskType)
            {
                case TaskTypes.FixLights:
                    {
                        EngineerFixLights(PlayerControl.LocalPlayer);
                        break;
                    }
                case TaskTypes.RestoreOxy:
                    MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, 0 | 64);
                    MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, 1 | 64);
                    break;
                case TaskTypes.ResetReactor:
                    MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Reactor, 16);
                    break;
                case TaskTypes.ResetSeismic:
                    MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Laboratory, 16);
                    break;
                case TaskTypes.FixComms:
                    MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
                    MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
                    break;
                case TaskTypes.StopCharles:
                    MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Reactor, 0 | 16);
                    MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Reactor, 1 | 16);
                    break;
                default:
                    {
                        if (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                        {
                            EngineerFixSubmergedOxygen(PlayerControl.LocalPlayer);
                        }

                        break;
                    }
            }
        }
    }

    [RegisterCustomButton]
    internal static void SetButtonCooldowns()
    {
        _engineerRepairButton.MaxTimer = 0f;
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }

    [MethodRpc((uint)CustomRPC.EngineerFixLights)]
    internal static void EngineerFixLights(PlayerControl sender)
    {
        var switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
        switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
    }

    [MethodRpc((uint)CustomRPC.EngineerFixSubmergedOxygen)]
    internal static void EngineerFixSubmergedOxygen(PlayerControl sender)
    {
        SubmergedCompatibility.RepairOxygen();
    }

    [MethodRpc((uint)CustomRPC.EngineerUsedRepair)]
    internal static void EngineerUsedRepair(PlayerControl sender, byte engineerId)
    {
        var engineerPlayer = Helpers.PlayerById(engineerId);
        if (engineerPlayer == null)
        {
            return;
        }
        var engineer = GetRole(engineerPlayer);
        if (engineer != null)
        {
            engineer.RemainingFixes--;
        }
    }
}