namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Engineer : RoleBase<Engineer>
{
    public static Color RoleColor = new Color32(0, 40, 245, byte.MaxValue);
    private static CustomButton engineerRepairButton;

    // write configs here
    public int remainingFixes = 1;
    public static int numberOfFixes { get { return (int)CustomOptionHolder.engineerNumberOfFixes.GetFloat(); } }
    public static bool highlightForImpostors { get { return CustomOptionHolder.engineerHighlightForImpostors.GetBool(); } }
    public static bool highlightForTeamJackal { get { return CustomOptionHolder.engineerHighlightForTeamJackal.GetBool(); } }

    public Engineer()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = ERoleType.Engineer;
        remainingFixes = numberOfFixes;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void FixedUpdate()
    {
        var jackalHighlight = highlightForTeamJackal && (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Jackal) || CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Sidekick));
        var impostorHighlight = highlightForImpostors && CachedPlayer.LocalPlayer.PlayerControl.IsTeamImpostor();
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

        engineerRepairButton = new CustomButton(
                () =>
                {
                    engineerRepairButton.Timer = 0f;

                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.EngineerUsedRepair);
                    RPCProcedure.engineerUsedRepair();

                    foreach (var task in CachedPlayer.LocalPlayer.PlayerControl.myTasks)
                    {
                        if (task.TaskType == TaskTypes.FixLights)
                        {
                            using var sender2 = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.EngineerFixLights);
                            RPCProcedure.engineerFixLights();
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
                            RPCProcedure.engineerFixSubmergedOxygen();
                        }
                    }
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Engineer) && Local.remainingFixes > 0 && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
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

                    return sabotageActive && Local.remainingFixes > 0 && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                () => { },
                getButtonSprite(),
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.UseButton,
                KeyCode.F
            )
        {
            buttonText = Tr.Get("RepairText")
        };
    }
    public static void SetButtonCooldowns()
    {
        engineerRepairButton.MaxTimer = 0f;
    }

    // write functions here
    private static Sprite buttonSprite;
    public static Sprite getButtonSprite()
    {
        if (buttonSprite) return buttonSprite;
        buttonSprite = Helpers.LoadSpriteFromResources("RebuildUs.Resources.EmergencyButton.png", 550f);
        return buttonSprite;
    }

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}