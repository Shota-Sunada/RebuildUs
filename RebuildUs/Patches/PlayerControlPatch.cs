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
        if (GameOptions.Get(FloatOptionNames.KillCooldown) <= 0f) return false;
        if (GameOptions.IsHideNSeekMode) return true;

        var multiplier = 1f;
        var addition = 0f;
        // if (Mini.mini != null && PlayerControl.LocalPlayer == Mini.mini) multiplier = Mini.isGrownUp() ? 0.66f : 2f;
        if (PlayerControl.LocalPlayer.IsRole(ERoleType.BountyHunter)) addition = BountyHunter.PunishmentTime;

        __instance.killTimer = Mathf.Clamp(time, 0f, GameOptions.Get(FloatOptionNames.KillCooldown) * multiplier + addition);
        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, GameOptions.Get(FloatOptionNames.KillCooldown) * multiplier + addition);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public static void FixedUpdatePostfix(PlayerControl __instance)
    {
        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || GameOptions.IsHideNSeekMode) return;

        if (PlayerControl.LocalPlayer == __instance)
        {
            Helpers.SetBasePlayerOutlines();
            Helpers.RefreshRoleDescription(__instance);
            Helpers.UpdatePlayerInfo();
            Helpers.SetPetVisibility();

            // Update player outlines
            Update.setBasePlayerOutlines();

            Update.setPetVisibility();

            // Update Role Description
            Helpers.RefreshRoleDescription(__instance);

            // Update Player Info
            Update.updatePlayerInfo();

            Update.impostorSetTarget();
        }

        RebuildUs.FixedUpdate(__instance);

        Usables.FixedUpdate(__instance);
        Update.StopCooldown(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public static void StartMeeting(PlayerControl __instance, NetworkedPlayerInfo meetingTarget)
    {
        Meeting.StartMeetingPrefix(__instance, meetingTarget);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public static bool CmdReportDeadBodyPrefix(PlayerControl __instance, NetworkedPlayerInfo target)
    {
        Helpers.handleVampireBiteOnBodyReport();

        if (__instance.isGM())
        {
            return false;
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public static void CmdReportDeadBodyPostfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo target)
    {
        Logger.info($"{__instance.getNameWithRole()} => {target?.getNameWithRole() ?? "null"}", "ReportDeadBody");
        // Medic or Detective report
        bool isMedicReport = Medic.medic != null && Medic.medic == CachedPlayer.LocalPlayer.PlayerControl && __instance.PlayerId == Medic.medic.PlayerId;
        bool isDetectiveReport = Detective.detective != null && Detective.detective == CachedPlayer.LocalPlayer.PlayerControl && __instance.PlayerId == Detective.detective.PlayerId;
        if (isMedicReport || isDetectiveReport)
        {
            DeadPlayer deadPlayer = deadPlayers?.Where(x => x.player?.PlayerId == target?.PlayerId)?.FirstOrDefault();

            if (deadPlayer != null && deadPlayer.killerIfExisting != null)
            {
                float timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.timeOfDeath).TotalMilliseconds;
                string msg = "";

                if (isMedicReport)
                {
                    msg = String.Format(Tr.Get("medicReport"), Math.Round(timeSinceDeath / 1000));
                }
                else if (isDetectiveReport)
                {
                    if (timeSinceDeath < Detective.reportNameDuration * 1000)
                    {
                        msg = String.Format(Tr.Get("detectiveReportName"), deadPlayer.killerIfExisting.Data.PlayerName);
                    }
                    else if (timeSinceDeath < Detective.reportColorDuration * 1000)
                    {
                        var typeOfColor = Helpers.isLighterColor(deadPlayer.killerIfExisting.Data.DefaultOutfit.ColorId) ?
                            Tr.Get("detectiveColorLight") :
                            Tr.Get("detectiveColorDark");
                        msg = String.Format(Tr.Get("detectiveReportColor"), typeOfColor);
                    }
                    else
                    {
                        msg = Tr.Get("detectiveReportNone");
                    }
                }

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    if (AmongUsClient.Instance.AmClient && FastDestroyableSingleton<HudManager>.Instance)
                    {
                        FastDestroyableSingleton<HudManager>.Instance.Chat.AddChat(CachedPlayer.LocalPlayer.PlayerControl, msg);
                    }
                    if (msg.IndexOf("who", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        FastDestroyableSingleton<Assets.CoreScripts.Telemetry>.Instance.SendWho();
                    }
                }
            }
        }
    }

    public static bool resetToCrewmate = false;
    public static bool resetToDead = false;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        // Allow everyone to murder players
        resetToCrewmate = !__instance.Data.Role.IsImpostor;
        resetToDead = __instance.Data.IsDead;
        __instance.Data.Role.TeamType = RoleTeamTypes.Impostor;
        __instance.Data.IsDead = false;

        if (Morphling.morphling != null && target == Morphling.morphling)
        {
            Morphling.resetMorph();
        }

        target.resetMorph();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
    {
        Logger.info($"{__instance.getNameWithRole()} => {target.getNameWithRole()}", "MurderPlayer");
        // Collect dead player info
        DeadPlayer deadPlayer = new(target, DateTime.UtcNow, DeathReason.Kill, __instance);
        GameHistory.deadPlayers.Add(deadPlayer);

        // Reset killer to crewmate if resetToCrewmate
        if (resetToCrewmate) __instance.Data.Role.TeamType = RoleTeamTypes.Crewmate;
        if (resetToDead) __instance.Data.IsDead = true;

        // Remove fake tasks when player dies
        if (target.hasFakeTasks())
            target.clearAllTasks();

        // Sidekick promotion trigger on murder
        if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead && target == Jackal.jackal && Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.sidekickPromotes();
        }

        // Pursuer promotion trigger on murder (the host sends the call such that everyone receives the update before a possible game End)
        if (target == Lawyer.target && AmongUsClient.Instance.AmHost)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.lawyerPromotesToPursuer();
        }

        // Cleaner Button Sync
        if (Cleaner.cleaner != null && CachedPlayer.LocalPlayer.PlayerControl == Cleaner.cleaner && __instance == Cleaner.cleaner && HudManagerStartPatch.cleanerCleanButton != null)
            HudManagerStartPatch.cleanerCleanButton.Timer = Cleaner.cleaner.killTimer;

        // Witch Button Sync
        if (Witch.triggerBothCooldowns && Witch.witch != null && CachedPlayer.LocalPlayer.PlayerControl == Witch.witch && __instance == Witch.witch && HudManagerStartPatch.witchSpellButton != null)
            HudManagerStartPatch.witchSpellButton.Timer = HudManagerStartPatch.witchSpellButton.MaxTimer;

        // Warlock Button Sync
        if (Warlock.warlock != null && CachedPlayer.LocalPlayer.PlayerControl == Warlock.warlock && __instance == Warlock.warlock && HudManagerStartPatch.warlockCurseButton != null)
        {
            if (Warlock.warlock.killTimer > HudManagerStartPatch.warlockCurseButton.Timer)
            {
                HudManagerStartPatch.warlockCurseButton.Timer = Warlock.warlock.killTimer;
            }
        }

        // Assassin Button Sync
        if (Assassin.assassin != null && CachedPlayer.LocalPlayer.PlayerControl == Assassin.assassin && __instance == Assassin.assassin && HudManagerStartPatch.assassinButton != null)
            HudManagerStartPatch.assassinButton.Timer = HudManagerStartPatch.assassinButton.MaxTimer;

        // Seer show flash and add dead player position
        if (Seer.seer != null && CachedPlayer.LocalPlayer.PlayerControl == Seer.seer && !Seer.seer.Data.IsDead && Seer.seer != target && Seer.mode <= 1)
        {
            Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
        }
        if (Seer.deadBodyPositions != null) Seer.deadBodyPositions.Add(target.transform.position);

        // Tracker store body positions
        if (Tracker.deadBodyPositions != null) Tracker.deadBodyPositions.Add(target.transform.position);

        // Medium add body
        if (Medium.deadBodies != null)
        {
            Medium.featureDeadBodies.Add(new Tuple<DeadPlayer, Vector3>(deadPlayer, target.transform.position));
        }

        // Mini set adapted kill cooldown
        if (CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.Mini) && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor && CachedPlayer.LocalPlayer.PlayerControl == __instance)
        {
            var multiplier = Mini.isGrownUp(CachedPlayer.LocalPlayer.PlayerControl) ? 0.66f : 2f;
            CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(PlayerControl.GameOptions.KillCooldown * multiplier);
        }

        // Set bountyHunter cooldown
        if (BountyHunter.bountyHunter != null && CachedPlayer.LocalPlayer.PlayerControl == BountyHunter.bountyHunter && __instance == BountyHunter.bountyHunter)
        {
            if (target == BountyHunter.bounty)
            {
                BountyHunter.bountyHunter.SetKillTimer(BountyHunter.bountyKillCooldown);
                BountyHunter.bountyUpdateTimer = 0f; // Force bounty update
            }
            else
                BountyHunter.bountyHunter.SetKillTimer(PlayerControl.GameOptions.KillCooldown + BountyHunter.punishmentTime);
        }

        // Update arsonist status
        Arsonist.updateStatus();

        // Show flash on bait kill to the killer if enabled
        if (Bait.bait != null && target == Bait.bait && Bait.showKillFlash && __instance != Bait.bait && __instance == CachedPlayer.LocalPlayer.PlayerControl)
        {
            Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
        }

        // impostor promote to last impostor
        if (target.isImpostor() && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }

        // 人形使いのダミー死亡処理
        if (target == Puppeteer.dummy)
        {
            // 蘇生する
            target.Revive();
            // 死体を消す
            DeadBody[] array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
            for (int i = 0; i < array.Length; i++)
            {
                if (GameData.Instance.GetPlayerById(array[i].ParentId).PlayerId == target.PlayerId)
                {
                    array[i].gameObject.active = false;
                }
            }
            Puppeteer.OnDummyDeath(__instance);
        }

        __instance.OnKill(target);
        Sherlock.recordKillLog(__instance, target);
        target.OnDeath(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public static void ExiledPostfix(PlayerControl __instance)
    {
        // Collect dead player info
        DeadPlayer deadPlayer = new(__instance, DateTime.UtcNow, DeathReason.Exile, null);
        GameHistory.deadPlayers.Add(deadPlayer);

        // Remove fake tasks when player dies
        if (__instance.hasFakeTasks())
            __instance.clearAllTasks();

        __instance.OnDeath(killer: null);

        // Sidekick promotion trigger on exile
        if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead && __instance == Jackal.jackal && Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.sidekickPromotes();
        }

        // Pursuer promotion trigger on exile (the host sends the call such that everyone receives the update before a possible game End)
        if (__instance == Lawyer.target && AmongUsClient.Instance.AmHost)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.lawyerPromotesToPursuer();
        }

        // impostor promote to last impostor
        if (__instance.isImpostor() && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
    public static bool CanMovePrefix(PlayerControl __instance, ref bool __result)
    {
        __result = __instance.moveable &&
            !Minigame.Instance &&
            (!DestroyableSingleton<HudManager>.InstanceExists || (!FastDestroyableSingleton<HudManager>.Instance.Chat.IsOpen && !FastDestroyableSingleton<HudManager>.Instance.KillOverlay.IsOpen && !FastDestroyableSingleton<HudManager>.Instance.GameMenu.IsOpen)) &&
            (!MapBehaviour.Instance || !MapBehaviour.Instance.IsOpenStopped) &&
            !MeetingHud.Instance &&
            !ExileController.Instance &&
            !IntroCutscene.Instance;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CheckName))]
    public static bool CheckNamePrefix(PlayerControl __instance, [HarmonyArgument(0)] string name)
    {
        if (CustomOptionHolder.uselessOptions.getBool() && CustomOptionHolder.playerNameDupes.getBool())
        {
            __instance.RpcSetName(name);
            GameData.Instance.UpdateName(__instance.PlayerId, name, false);
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.IsFlashlightEnabled))]
    public static class IsFlashlightEnabledPatch
    {
        public static bool Prefix(ref bool __result)
        {
            if (GameOptions.IsHideNSeekMode) return true;
            __result = false;
            if (!PlayerControl.LocalPlayer.Data.IsDead && Lighter.lighter != null && Lighter.lighter.PlayerId == PlayerControl.LocalPlayer.PlayerId)
            {
                __result = true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.AdjustLighting))]
    public static class AdjustLight
    {
        public static bool Prefix(PlayerControl __instance)
        {
            if (__instance == null || PlayerControl.LocalPlayer == null || Lighter.lighter == null) return true;

            bool hasFlashlight = !PlayerControl.LocalPlayer.Data.IsDead && Lighter.lighter.PlayerId == PlayerControl.LocalPlayer.PlayerId;
            __instance.SetFlashlightInputMethod();
            __instance.lightSource.SetupLightingForGameplay(hasFlashlight, Lighter.flashlightWidth, __instance.TargetFlashlight.transform);

            return false;
        }
    }
}