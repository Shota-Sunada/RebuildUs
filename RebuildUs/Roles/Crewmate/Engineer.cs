namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Engineer : RoleBase<Engineer>
{
    public static Color NameColor = new Color32(0, 40, 245, byte.MaxValue);
    public override Color RoleColor => NameColor;
    private static CustomButton EngineerRepairButton;

    // write configs here
    public int RemainingFixes = 1;
    public static int NumberOfFixes { get { return (int)CustomOptionHolder.EngineerNumberOfFixes.GetFloat(); } }
    public static bool HighlightForImpostors { get { return CustomOptionHolder.EngineerHighlightForImpostors.GetBool(); } }
    public static bool HighlightForTeamJackal { get { return CustomOptionHolder.EngineerHighlightForTeamJackal.GetBool(); } }

    public Engineer()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Engineer;
        RemainingFixes = NumberOfFixes;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        var local = PlayerControl.LocalPlayer;
        if (local == null) return;

        bool jackalHighlight = HighlightForTeamJackal && (local.IsRole(RoleType.Jackal) || local.IsRole(RoleType.Sidekick));
        bool impostorHighlight = HighlightForImpostors && local.IsTeamImpostor();

        var shipStatus = MapUtilities.CachedShipStatus;
        if ((jackalHighlight || impostorHighlight) && shipStatus != null && shipStatus.AllVents != null)
        {
            var engineers = AllPlayers;
            int engineerCount = engineers.Count;
            var allVents = shipStatus.AllVents;

            for (int i = 0; i < allVents.Length; i++)
            {
                var vent = allVents[i];
                if (vent == null || vent.myRend == null) continue;

                var mat = vent.myRend.material;
                if (mat == null) continue;

                bool anyEngineerInVent = false;
                for (int j = 0; j < engineerCount; j++)
                {
                    if (engineers[j].inVent)
                    {
                        anyEngineerInVent = true;
                        break;
                    }
                }

                if (anyEngineerInVent)
                {
                    mat.SetFloat("_Outline", 1f);
                    mat.SetColor("_OutlineColor", RoleColor);
                }
                else
                {
                    // Only remove outline if it's not being set by something else (Check alpha of AddColor as a proxy)
                    if (mat.HasProperty("_AddColor") && mat.GetColor("_AddColor").a == 0f)
                    {
                        mat.SetFloat("_Outline", 0f);
                    }
                }
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

                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.EngineerUsedRepair);
                sender.Write(PlayerControl.LocalPlayer.PlayerId);
                RPCProcedure.EngineerUsedRepair(PlayerControl.LocalPlayer.PlayerId);

                for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++)
                {
                    var task = PlayerControl.LocalPlayer.myTasks[i];
                    if (task.TaskType == TaskTypes.FixLights)
                    {
                        using var sender2 = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.EngineerFixLights);
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
                        using var sender3 = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.EngineerFixSubmergedOxygen);
                        RPCProcedure.EngineerFixSubmergedOxygen();
                    }
                }
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Engineer) && Local.RemainingFixes > 0 && PlayerControl.LocalPlayer.IsAlive(); },
            () =>
            {
                bool sabotageActive = false;
                for (int i = 0; i < PlayerControl.LocalPlayer.myTasks.Count; i++)
                {
                    var task = PlayerControl.LocalPlayer.myTasks[i];
                    if (task.TaskType is TaskTypes.FixLights or TaskTypes.RestoreOxy or TaskTypes.ResetReactor or TaskTypes.ResetSeismic or TaskTypes.FixComms or TaskTypes.StopCharles
                    || (SubmergedCompatibility.IsSubmerged && task.TaskType == SubmergedCompatibility.RetrieveOxygenMask))
                    {
                        sabotageActive = true;
                        break;
                    }
                }

                return sabotageActive && Local.RemainingFixes > 0 && PlayerControl.LocalPlayer.CanMove;
            },
            () => { },
            AssetLoader.RepairButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            KeyCode.F
        )
        {
            ButtonText = Tr.Get("Hud.RepairText")
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