using System.Text;
namespace RebuildUs.Patches;

[HarmonyPatch]
public static class PlayerControlPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    public static void HandleRpcPostfix(byte callId, MessageReader reader)
    {
        RPCProcedure.Handle((CustomRPC)callId, reader);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSyncSettings))]
    public static void RpcSyncSettingsPostfix()
    {
        // CustomOption.SyncVanillaSettings();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckColor))]
    public static bool CheckColorPrefix(PlayerControl __instance, [HarmonyArgument(0)] byte bodyColor)
    {
        return CustomColors.CheckColor(__instance, bodyColor);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    public static bool SetKillTimerPrefix(PlayerControl __instance, [HarmonyArgument(0)] float time)
    {
        float baseCooldown = Helpers.GetOption(FloatOptionNames.KillCooldown);
        if (baseCooldown <= 0f) return false;
        if (Helpers.IsHideNSeekMode) return true;

        var multiplier = 1f;
        var addition = 0f;
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer != null)
        {
            if (Mini.Exists && localPlayer.HasModifier(ModifierType.Mini))
            {
                multiplier = Mini.IsGrownUp(localPlayer) ? 0.66f : 2f;
            }
            if (localPlayer.IsRole(RoleType.BountyHunter))
            {
                addition = BountyHunter.PunishmentTime;
            }
        }

        float maxCooldown = baseCooldown * multiplier + addition;
        __instance.killTimer = Mathf.Clamp(time, 0f, maxCooldown);
        if (FastDestroyableSingleton<HudManager>.InstanceExists)
        {
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, maxCooldown);
        }
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static void FixedUpdatePostfix(PlayerControl __instance)
    {
        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || Helpers.IsHideNSeekMode) return;

        if (PlayerControl.LocalPlayer == __instance)
        {
            Helpers.SetBasePlayerOutlines();
            Helpers.RefreshRoleDescription(__instance);
            Helpers.UpdatePlayerInfo();
            Helpers.SetPetVisibility();
            Update.ImpostorSetTarget();
            Update.PlayerSizeUpdate(__instance);

            Garlic.UpdateAll();
        }

        RebuildUs.FixedUpdate(__instance);

        Usables.FixedUpdate(__instance);
        Update.StopCooldown(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public static bool StartMeeting(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        return Meeting.StartMeetingPrefix(__instance, target);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public static bool CmdReportDeadBodyPrefix(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        Helpers.HandleVampireBiteOnBodyReport();

        if (__instance.IsGM())
        {
            return false;
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public static void CmdReportDeadBodyPostfix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo target)
    {
        var sb = new StringBuilder();
        sb.Append(__instance.GetNameWithRole());
        sb.Append(" => ");
        sb.Append(target?.Object?.GetNameWithRole() ?? "null");
        Logger.LogInfo(sb.ToString(), "ReportDeadBody");

        Meeting.HandleReportDeadBody(__instance, target);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        GameHistory.OnMurderPlayerPrefix(__instance, target);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        GameHistory.OnMurderPlayerPostfix(__instance, target);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public static void ExiledPostfix(PlayerControl __instance)
    {
        GameHistory.OnExiled(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
    public static bool CanMovePrefix(PlayerControl __instance, ref bool __result)
    {
        __result = __instance.moveable &&
            !Minigame.Instance &&
            (!DestroyableSingleton<HudManager>.InstanceExists || (!FastDestroyableSingleton<HudManager>.Instance.Chat.IsOpenOrOpening && !FastDestroyableSingleton<HudManager>.Instance.KillOverlay.IsOpen && !FastDestroyableSingleton<HudManager>.Instance.GameMenu.IsOpen)) &&
            (!MapBehaviour.Instance || !MapBehaviour.Instance.IsOpenStopped) &&
            !MeetingHud.Instance &&
            !ExileController.Instance &&
            !IntroCutscene.Instance;
        return false;
    }
}