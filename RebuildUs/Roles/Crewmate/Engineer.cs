namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class Engineer : RoleBase<Engineer>
{
    internal static Color NameColor = new Color32(0, 40, 245, byte.MaxValue);

    private static CustomButton _engineerRepairButton;

    // write configs here
    internal int RemainingFixes = 1;

    public Engineer()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Engineer;
        RemainingFixes = NumberOfFixes;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    private static int NumberOfFixes { get => (int)CustomOptionHolder.EngineerNumberOfFixes.GetFloat(); }
    internal static bool HighlightForImpostors { get => CustomOptionHolder.EngineerHighlightForImpostors.GetBool(); }
    internal static bool HighlightForTeamJackal { get => CustomOptionHolder.EngineerHighlightForTeamJackal.GetBool(); }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }
    internal override void FixedUpdate() { }
    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

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
                                    Tr.Get(TrKey.RepairText)
                                   );
    }

    private static bool CouldUse()
    {
        bool sabotageActive = false;
        foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
        {
            if (task.TaskType is not (TaskTypes.FixLights
                                      or TaskTypes.RestoreOxy
                                      or TaskTypes.ResetReactor
                                      or TaskTypes.ResetSeismic
                                      or TaskTypes.FixComms
                                      or TaskTypes.StopCharles)
                && (!SubmergedCompatibility.IsSubmerged || task.TaskType != SubmergedCompatibility.RetrieveOxygenMask)
               )
                continue;

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

        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.EngineerUsedRepair);
        sender.Write(PlayerControl.LocalPlayer.PlayerId);
        RPCProcedure.EngineerUsedRepair(PlayerControl.LocalPlayer.PlayerId);

        foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
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
    }

    internal static void SetButtonCooldowns()
    {
        _engineerRepairButton.MaxTimer = 0f;
    }

    internal static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}