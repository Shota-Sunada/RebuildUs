using AmongUs.Data;
using Assets.CoreScripts;
using BepInEx.Unity.IL2CPP.Utils.Collections;

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
        if (FastDestroyableSingleton<HudManager>.Instance)
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

            _timer += Time.fixedDeltaTime;
            if (_timer >= 0.1f)
            {
                _timer = 0f;
                Helpers.RefreshRoleDescription(__instance);
                Helpers.UpdatePlayerInfo();
                Helpers.SetPetVisibility();
            }

            Update.ImpostorSetTarget();
            Update.PlayerSizeUpdate(__instance);

            Garlic.UpdateAll();
        }

        RebuildUs.FixedUpdate(__instance);

        AllPlayers.Update(__instance);

        Usables.FixedUpdate(__instance);
        Update.StopCooldown(__instance);
    }

    private static float _timer = 0f;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public static bool StartMeeting(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        return Meeting.StartMeetingPrefix(__instance, target);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
    public static bool ReportDeadBodyPrefix(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        Logger.LogInfo("ReportDeadBody Prefix");

        Helpers.HandleVampireBiteOnBodyReport();

        if (__instance.IsGM())
        {
            return false;
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
    public static void ReportDeadBodyPostfix(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        Logger.LogInfo("ReportDeadBody Postfix");

        var sb = new StringBuilder();
        sb.Append(__instance.GetNameWithRole());
        sb.Append(" => ");
        sb.Append(target?.Object?.GetNameWithRole() ?? "null");
        Logger.LogInfo(sb.ToString(), "ReportDeadBody");
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static bool MurderPlayerPrefix(PlayerControl __instance, PlayerControl target, MurderResultFlags resultFlags)
    {
        GameHistory.OnMurderPlayerPrefix(__instance, target);

        // ORIGINAL MURDER_PLAYER
        __instance.isKilling = false;
        __instance.logger.Debug($"{__instance.PlayerId} trying to murder {target.PlayerId}");
        NetworkedPlayerInfo data = target.Data;
        if (resultFlags.HasFlag(MurderResultFlags.FailedError)) return false;
        if (resultFlags.HasFlag(MurderResultFlags.FailedProtected) || resultFlags.HasFlag(MurderResultFlags.DecisionByHost) && target.protectedByGuardianId > -1)
        {
            target.protectedByGuardianThisRound = true;
            bool flag = PlayerControl.LocalPlayer.Data.Role.Role == RoleTypes.GuardianAngel;
            if (flag && PlayerControl.LocalPlayer.Data.PlayerId == target.protectedByGuardianId)
            {
                DataManager.Player.Stats.IncrementStat(StatID.Role_GuardianAngel_CrewmatesProtected);
                DestroyableSingleton<AchievementManager>.Instance.OnProtectACrewmate();
            }
            if (__instance.AmOwner | flag)
            {
                target.ShowFailedMurder();
                __instance.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown) / 2f);
            }
            else
                target.RemoveProtection();
            __instance.logger.Debug($"{__instance.PlayerId} failed to murder {target.PlayerId} due to guardian angel protection");
        }
        else
        {
            if (!resultFlags.HasFlag(MurderResultFlags.Succeeded) && !resultFlags.HasFlag(MurderResultFlags.DecisionByHost)) return false;
            DestroyableSingleton<DebugAnalytics>.Instance.Analytics.Kill(target.Data, __instance.Data);
            if (__instance.AmOwner)
            {
                if (GameManager.Instance.IsHideAndSeek())
                    DataManager.Player.Stats.IncrementStat(StatID.HideAndSeek_ImpostorKills);
                else
                    DataManager.Player.Stats.IncrementStat(StatID.ImpostorKills);
                if (__instance.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
                    DataManager.Player.Stats.IncrementStat(StatID.Role_Shapeshifter_ShiftedKills);
                if (Constants.ShouldPlaySfx())
                    SoundManager.Instance.PlaySound(__instance.KillSfx, false, 0.8f);
                __instance.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown));
            }
            DestroyableSingleton<UnityTelemetry>.Instance.WriteMurder();
            target.gameObject.layer = LayerMask.NameToLayer("Ghost");
            if (target.AmOwner)
            {
                DataManager.Player.Stats.IncrementStat(StatID.TimesMurdered);
                if (Minigame.Instance)
                {
                    try
                    {
                        Minigame.Instance.Close();
                        Minigame.Instance.Close();
                    }
                    catch
                    {
                    }
                }
                DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(__instance.Data, data);
                target.cosmetics.SetNameMask(false);
                target.RpcSetScanner(false);
            }
            DestroyableSingleton<AchievementManager>.Instance.OnMurder(__instance.AmOwner, target.AmOwner, __instance.CurrentOutfitType == PlayerOutfitType.Shapeshifted, __instance.shapeshiftTargetPlayerId, target.PlayerId);
            // DISABLE ORIGINAL CO_PERFORM_KILL
            // __instance.MyPhysics.StartCoroutine(__instance.KillAnimations.Random().CoPerformKill(__instance, target));
            __instance.MyPhysics.StartCoroutine(KillAnimationPatch.CoPerformKill(__instance.KillAnimations.Random(), __instance, target).WrapToIl2Cpp());
            __instance.logger.Debug($"{__instance.PlayerId} succeeded in murdering {target.PlayerId}");
        }
        // ORIGINAL MURDER_PLAYER

        GameHistory.OnMurderPlayerPostfix(__instance, target);
        return false;
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