using RebuildUs.Modules;
using RebuildUs.Modules.RPC;
using RebuildUs.Roles.Impostor;

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
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == AmongUs.GameOptions.GameModes.HideNSeek) return true;
        if (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown <= 0f) return false;
        var multiplier = 1f;
        var addition = 0f;
        // if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini) multiplier = Mini.isGrownUp() ? 0.66f : 2f;
        if (PlayerControl.LocalPlayer.IsRole(ERoleType.BountyHunter)) addition = BountyHunter.punishmentTime;

        __instance.killTimer = Mathf.Clamp(time, 0f, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier + addition);
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * multiplier + addition);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static void FixedUpdatePostfix(PlayerControl __instance)
    {
        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || GameOptionsManager.Instance.currentGameOptions.GameMode == AmongUs.GameOptions.GameModes.HideNSeek) return;

        if (PlayerControl.LocalPlayer == __instance)
        {
            Helpers.setBasePlayerOutlines();
            Helpers.refreshRoleDescription(__instance);
            Helpers.updatePlayerInfo();
            Helpers.setPetVisibility();
        }

        RebuildUs.FixedUpdate(__instance);
    }
}