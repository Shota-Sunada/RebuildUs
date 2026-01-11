namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Engineer : RoleBase<Engineer>
{
    public static Color RoleColor = new Color32(0, 40, 245, byte.MaxValue);
    private static CustomButton EngineerRepairButton;

    // write configs here
    public int RemainingFixes = 1;
    public static int NumberOfFixes { get { return (int)CustomOptionHolder.EngineerNumberOfFixes.GetFloat(); } }
    public static bool HighlightForImpostors { get { return CustomOptionHolder.EngineerHighlightForImpostors.GetBool(); } }
    public static bool HighlightForTeamJackal { get { return CustomOptionHolder.EngineerHighlightForTeamJackal.GetBool(); } }

    public Engineer()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = ERoleType.Engineer;
        RemainingFixes = NumberOfFixes;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        var jackalHighlight = HighlightForTeamJackal && (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Jackal) || CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Sidekick));
        var impostorHighlight = HighlightForImpostors && CachedPlayer.LocalPlayer.PlayerControl.IsTeamImpostor();
        if ((jackalHighlight || impostorHighlight) && MapUtilities.CachedShipStatus?.AllVents != null)
        {
            foreach (var vent in MapUtilities.CachedShipStatus.AllVents)
            {
                try
                {
                    if (vent?.myRend?.material != null)
                    {
                        foreach (var engineer in AllPlayers)
                        {
                            if (engineer.inVent)
                            {
                                vent.myRend.material.SetFloat("_Outline", 1f);
                                vent.myRend.material.SetColor("_OutlineColor", RoleColor);
                            }
                            else if (vent.myRend.material.GetColor("_AddColor").a == 0f)
                            {
                                vent.myRend.material.SetFloat("_Outline", 0);
                            }
                        }
                    }
                }
                catch { }
            }
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {

        EngineerRepairButton = new CustomButton(
                () =>
                {
                    EngineerRepairButton.Timer = 0f;

                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.EngineerUsedRepair);
                    sender.Write(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);
                    RPCProcedure.EngineerUsedRepair(CachedPlayer.LocalPlayer.PlayerControl.PlayerId);

                    foreach (var task in CachedPlayer.LocalPlayer.PlayerControl.myTasks)
                    {
                        if (task.TaskType == TaskTypes.FixLights)
                        {
                            using var sender2 = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.EngineerFixLights);
                            RPCProcedure.EngineerFixLights();
                        }
                        else if (task.TaskType is TaskTypes.RestoreOxy)
                        {
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, 0 | 64);
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.LifeSupp, 1 | 64);
                        }
                        else if (task.TaskType is TaskTypes.ResetReactor)
                        {
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Reactor, 16);
                        }
                        else if (task.TaskType is TaskTypes.ResetSeismic)
                        {
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Laboratory, 16);
                        }
                        else if (task.TaskType is TaskTypes.FixComms)
                        {
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
                        }
                        else if (task.TaskType is TaskTypes.StopCharles)
                        {
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Reactor, 0 | 16);
                            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Reactor, 1 | 16);
                        }
                        else if (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask)
                        {
                            using var sender3 = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.EngineerFixSubmergedOxygen);
                            RPCProcedure.EngineerFixSubmergedOxygen();
                        }
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Engineer) && Local.RemainingFixes > 0 && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () =>
                {
                    bool sabotageActive = false;
                    foreach (PlayerTask task in CachedPlayer.LocalPlayer.PlayerControl.myTasks)
                    {
                        if (task.TaskType is TaskTypes.FixLights or TaskTypes.RestoreOxy or TaskTypes.ResetReactor or TaskTypes.ResetSeismic or TaskTypes.FixComms or TaskTypes.StopCharles
                        || (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask))
                        {
                            sabotageActive = true;
                        }
                    }

                    return sabotageActive && Local.RemainingFixes > 0 && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                () => { },
                AssetLoader.EmergencyButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.UseButton,
                KeyCode.F
            )
        {
            ButtonText = Tr.Get("RoleText", "RepairText")
        };
    }
    public static void SetButtonCooldowns()
    {
        EngineerRepairButton.MaxTimer = 0f;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}