namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.Engineer, RoleTeam.Crewmate, typeof(MultiRoleBase<Engineer>), nameof(CustomOptionHolder.EngineerSpawnRate))]
internal class Engineer : MultiRoleBase<Engineer>
{
    public static Color Color = new Color32(0, 40, 245, byte.MaxValue);

    private static CustomButton _engineerRepairButton;

    internal int RemainingFixes = 1;

    public Engineer()
    {
        StaticRoleType = CurrentRoleType = RoleType.Engineer;
        RemainingFixes = NumberOfFixes;
    }

    private static int NumberOfFixes { get => (int)CustomOptionHolder.EngineerNumberOfFixes.GetFloat(); }
    internal static bool HighlightForImpostors { get => CustomOptionHolder.EngineerHighlightForImpostors.GetBool(); }
    internal static bool HighlightForTeamJackal { get => CustomOptionHolder.EngineerHighlightForTeamJackal.GetBool(); }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _engineerRepairButton = new(
            () =>
            {
                _engineerRepairButton.Timer = 0f;

                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.EngineerUsedRepair);
                sender.Write(PlayerControl.LocalPlayer.PlayerId);
                RPCProcedure.EngineerUsedRepair(PlayerControl.LocalPlayer.PlayerId);

                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                {
                    switch (task.TaskType)
                    {
                        case TaskTypes.FixLights:
                            {
                                using RPCSender sender2 = new(PlayerControl.LocalPlayer.NetId, CustomRPC.EngineerFixLights);
                                RPCProcedure.EngineerFixLights();
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
                                    using RPCSender sender3 = new(PlayerControl.LocalPlayer.NetId, CustomRPC.EngineerFixSubmergedOxygen);
                                    RPCProcedure.EngineerFixSubmergedOxygen();
                                }

                                break;
                            }
                    }
                }
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Engineer) && Local.RemainingFixes > 0 && PlayerControl.LocalPlayer.IsAlive(); },
            () =>
            {
                var sabotageActive = false;
                foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                {
                    if (task.TaskType is not (TaskTypes.FixLights or TaskTypes.RestoreOxy or TaskTypes.ResetReactor or TaskTypes.ResetSeismic or TaskTypes.FixComms or TaskTypes.StopCharles)
                        && (!SubmergedCompatibility.IsSubmerged || task.TaskType != SubmergedCompatibility.RetrieveOxygenMask))
                    {
                        continue;
                    }

                    sabotageActive = true;
                    break;
                }

                return sabotageActive && Local.RemainingFixes > 0 && PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            AssetLoader.RepairButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.RepairText));
    }

    [SetCustomButtonTimer]
    internal static void SetButtonCooldowns()
    {
        _engineerRepairButton.MaxTimer = 0f;
    }

    internal static void Clear()
    {
        Players.Clear();
    }
}