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
        if (Helpers.GetOption(FloatOptionNames.KillCooldown) <= 0f) return false;
        if (Helpers.IsHideNSeekMode) return true;

        var multiplier = 1f;
        var addition = 0f;
        // if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini) multiplier = Mini.isGrownUp() ? 0.66f : 2f;
        if (PlayerControl.LocalPlayer.IsRole(RoleType.BountyHunter)) addition = BountyHunter.PunishmentTime;

        __instance.killTimer = Mathf.Clamp(time, 0f, Helpers.GetOption(FloatOptionNames.KillCooldown) * multiplier + addition);
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, Helpers.GetOption(FloatOptionNames.KillCooldown) * multiplier + addition);
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
    public static void StartMeeting(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        Meeting.StartMeetingPrefix(__instance, target);
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
        Logger.LogInfo($"{__instance.GetNameWithRole()} => {target.Object?.GetNameWithRole() ?? "null"}", "ReportDeadBody");
        // Medic or Detective report
        bool isMedicReport = PlayerControl.LocalPlayer.IsRole(RoleType.Medic) && __instance.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        bool isDetectiveReport = Detective.Exists && PlayerControl.LocalPlayer.IsRole(RoleType.Detective) && __instance.PlayerId == PlayerControl.LocalPlayer.PlayerId;
        if (isMedicReport || isDetectiveReport)
        {
            var deadPlayer = GameHistory.DeadPlayers?.Where(x => x.Player?.PlayerId == target?.PlayerId)?.FirstOrDefault();

            if (deadPlayer != null && deadPlayer.KillerIfExisting != null)
            {
                float timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.TimeOfDeath).TotalMilliseconds;
                string msg = "";

                if (isMedicReport)
                {
                    msg = string.Format(Tr.Get("Hud.MedicReport"), Math.Round(timeSinceDeath / 1000));
                }
                else if (isDetectiveReport)
                {
                    if (timeSinceDeath < Detective.ReportNameDuration * 1000)
                    {
                        msg = string.Format(Tr.Get("Hud.DetectiveReportName"), deadPlayer.KillerIfExisting.Data.PlayerName);
                    }
                    else if (timeSinceDeath < Detective.ReportColorDuration * 1000)
                    {
                        var typeOfColor = Helpers.IsLighterColor(deadPlayer.KillerIfExisting.Data.DefaultOutfit.ColorId) ?
                            Tr.Get("Color.DetectiveColorLight") :
                            Tr.Get("Color.DetectiveColorDark");
                        msg = string.Format(Tr.Get("Hud.DetectiveReportColor"), typeOfColor);
                    }
                    else
                    {
                        msg = Tr.Get("Hud.DetectiveReportNone");
                    }
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                    {
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, msg);
                    }
                    if (msg.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        FastDestroyableSingleton<Assets.CoreScripts.UnityTelemetry>.Instance.SendWho();
                    }
                }
            }
        }
    }

    public static bool ResetToCrewmate = false;
    public static bool ResetToDead = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        // Allow everyone to murder players
        ResetToCrewmate = !__instance.Data.Role.IsImpostor;
        ResetToDead = __instance.Data.IsDead;
        __instance.Data.Role.TeamType = RoleTeamTypes.Impostor;
        __instance.Data.IsDead = false;

        if (Morphing.Exists && target.IsRole(RoleType.Morphing))
        {
            Morphing.ResetMorph();
        }

        target.ResetMorph();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        Logger.LogInfo($"{__instance.GetNameWithRole()} => {target.GetNameWithRole()}", "MurderPlayer");
        // Collect dead player info
        var deadPlayer = new DeadPlayer(target, DateTime.UtcNow, DeathReason.Kill, __instance);
        GameHistory.DeadPlayers.Add(deadPlayer);

        // Reset killer to crewmate if resetToCrewmate
        if (ResetToCrewmate) __instance.Data.Role.TeamType = RoleTeamTypes.Crewmate;
        if (ResetToDead) __instance.Data.IsDead = true;

        AllPlayers.OnKill(__instance, target, deadPlayer);

        __instance.OnKill(target);
        target.OnDeath(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public static void ExiledPostfix(PlayerControl __instance)
    {
        // Collect dead player info
        var deadPlayer = new DeadPlayer(__instance, DateTime.UtcNow, DeathReason.Exile, null);
        GameHistory.DeadPlayers.Add(deadPlayer);

        // Remove fake tasks when player dies
        if (__instance.HasFakeTasks())
        {
            __instance.ClearAllTasks();
        }

        __instance.OnDeath(killer: null);

        // impostor promote to last impostor
        if (__instance.IsTeamImpostor() && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.PromoteToLastImpostor();
        }
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