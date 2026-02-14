namespace RebuildUs.Modules;

public static class Usables
{
    private static Sprite DefaultVentSprite = null;
    private static Minigame AlignTelescopeMinigame = null;

    public static bool OnConsoleUse(Console __instance)
    {
        if (IsBlocked(__instance, PlayerControl.LocalPlayer))
        {
            return false;
        }

        if (CustomOptionHolder.AirshipReplaceSafeTask.GetBool())
        {
            var playerTask = __instance.FindTask(PlayerControl.LocalPlayer);
            if (playerTask != null && playerTask.MinigamePrefab.name == "SafeGame")
            {
                if (AlignTelescopeMinigame == null)
                {
                    foreach (var task in MapData.PolusShip.ShortTasks)
                    {
                        if (task.name == "AlignTelescope")
                        {
                            AlignTelescopeMinigame = task.MinigamePrefab;
                            break;
                        }
                    }
                }
                playerTask.MinigamePrefab = AlignTelescopeMinigame;
            }
        }

        return true;
    }

    public static bool VentCanUse(Vent __instance, ref float __result, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
    {
        float num = float.MaxValue;
        PlayerControl @object = pc.Object;

        bool roleCouldUse = @object.CanUseVents();
        string name = __instance.name;

        if (name.StartsWith("SealedVent_"))
        {
            canUse = couldUse = false;
            __result = num;
            return false;
        }

        // Submerged Compatibility if needed:
        if (SubmergedCompatibility.IsSubmerged)
        {
            if (SubmergedCompatibility.GetInTransition())
            {
                __result = float.MaxValue;
                canUse = couldUse = false;
                return false;
            }
            switch (__instance.Id)
            {
                case 9:  // Cannot enter vent 9 (Engine Room Exit Only Vent)!
                    if (PlayerControl.LocalPlayer.inVent) break;
                    __result = float.MaxValue;
                    canUse = couldUse = false;
                    return false;
                case 14: // Lower Central
                    __result = float.MaxValue;
                    couldUse = roleCouldUse && !pc.IsDead && (@object.CanMove || @object.inVent);
                    canUse = couldUse;
                    if (canUse)
                    {
                        Vector3 center = @object.Collider.bounds.center;
                        Vector3 position = __instance.transform.position;
                        __result = Vector2.Distance(center, position);
                        canUse &= __result <= __instance.UsableDistance;
                    }
                    return false;
            }
        }

        var usableDistance = __instance.UsableDistance;
        if (name.StartsWith("JackInTheBoxVent_"))
        {
            var lp = PlayerControl.LocalPlayer;
            if (lp != null && !lp.IsRole(RoleType.Trickster) && !lp.IsGM())
            {
                // Only the Trickster can use the Jack-In-The-Boxes!
                canUse = false;
                couldUse = false;
                __result = num;
                return false;
            }
            else
            {
                // Reduce the usable distance to reduce the risk of gettings stuck while trying to jump into the box if it's placed near objects
                usableDistance = 0.4f;
            }
        }

        couldUse = (@object.inVent || roleCouldUse) && !pc.IsDead && (@object.CanMove || @object.inVent);
        canUse = couldUse;
        if (canUse)
        {
            Vector2 truePosition = @object.GetTruePosition();
            Vector3 position = __instance.transform.position;
            num = Vector2.Distance(truePosition, position);

            canUse &= num <= usableDistance && !PhysicsHelpers.AnythingBetween(truePosition, position, Constants.ShipOnlyMask, false);
        }
        __result = num;
        return false;
    }

    public static bool VentUse(Vent __instance)
    {
        var lp = PlayerControl.LocalPlayer;
        if (lp == null) return false;

        __instance.CanUse(lp.Data, out bool canUse, out bool couldUse);
        bool canMoveInVents = !lp.IsRole(RoleType.Spy) && !lp.HasModifier(ModifierType.Madmate) && !lp.IsRole(RoleType.Madmate) && !lp.IsRole(RoleType.Suicider) && !lp.HasModifier(ModifierType.CreatedMadmate);
        if (!canUse) return false; // No need to execute the native method as using is disallowed anyways

        bool isEnter = !lp.inVent;

        if (__instance.name.StartsWith("JackInTheBoxVent_"))
        {
            __instance.SetButtons(isEnter && canMoveInVents);
            {
                using var sender = new RPCSender(lp.NetId, CustomRPC.UseUncheckedVent);
                sender.WritePacked(__instance.Id);
                sender.Write(lp.PlayerId);
                sender.Write(isEnter ? byte.MaxValue : (byte)0);
                RPCProcedure.UseUncheckedVent(__instance.Id, lp.PlayerId, isEnter ? byte.MaxValue : (byte)0);
            }
            return false;
        }

        if (isEnter)
        {
            lp.MyPhysics.RpcEnterVent(__instance.Id);
        }
        else
        {
            lp.MyPhysics.RpcExitVent(__instance.Id);
        }
        __instance.SetButtons(isEnter && canMoveInVents);
        return false;
    }

    private static readonly StringBuilder EmergencyStringBuilder = new();

    public static void FixedUpdate(PlayerControl __instance)
    {
        if (__instance.AmOwner && Helpers.ShowButtons)
        {
            var hudManager = FastDestroyableSingleton<HudManager>.Instance;
            hudManager.ImpostorVentButton.Hide();
            hudManager.SabotageButton.Hide();

            if (Helpers.ShowButtons)
            {
                if (__instance.CanUseVents())
                {
                    hudManager.ImpostorVentButton.Show();
                }

                if (__instance.CanSabotage())
                {
                    hudManager.SabotageButton.Show();
                    hudManager.SabotageButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public static void SabotageButtonRefresh()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null) return;

        // Mafia disable sabotage button for Janitor and sometimes for Mafioso
        bool blockSabotageJanitor = localPlayer.IsRole(RoleType.Janitor);
        bool blockSabotageMafioso = localPlayer.IsRole(RoleType.Mafioso) && !Mafia.IsGodfatherDead;
        if (blockSabotageJanitor || blockSabotageMafioso)
        {
            FastDestroyableSingleton<HudManager>.Instance.SabotageButton.SetDisabled();
        }
    }

    public static bool KillButtonDoClick(KillButton __instance)
    {
        var lp = PlayerControl.LocalPlayer;
        if (!__instance.isActiveAndEnabled || !__instance.currentTarget || __instance.isCoolingDown || lp.Data.IsDead || !lp.CanMove) return false;
        bool showAnimation = true;

        // Use an unchecked kill command, to allow shorter kill cooldowns etc. without getting kicked
        var res = Helpers.CheckMurderAttemptAndKill(lp, __instance.currentTarget, showAnimation: showAnimation);
        // Handle blank kill
        if (res == MurderAttemptResult.BlankKill)
        {
            float cooldown = Helpers.GetOption(FloatOptionNames.KillCooldown);
            lp.SetKillTimer(cooldown);
            if (lp.IsRole(RoleType.Cleaner))
            {
                lp.killTimer = Cleaner.CleanerCleanButton.Timer = Cleaner.CleanerCleanButton.MaxTimer;
            }
            else if (lp.IsRole(RoleType.Warlock))
            {
                lp.killTimer = Warlock.WarlockCurseButton.Timer = Warlock.WarlockCurseButton.MaxTimer;
            }
            else if (lp.HasModifier(ModifierType.Mini) && lp.Data.Role.IsImpostor)
            {
                lp.SetKillTimer(cooldown * (Mini.IsGrownUp(lp) ? 0.66f : 2f));
            }
            else if (lp.IsRole(RoleType.Witch))
            {
                lp.killTimer = Witch.WitchSpellButton.Timer = Witch.WitchSpellButton.MaxTimer;
            }
        }

        __instance.SetTarget(null);

        return false;
    }

    public static bool UseButtonSetTarget(UseButton __instance, IUsable target)
    {
        var pc = PlayerControl.LocalPlayer;
        __instance.enabled = true;

        if (IsBlocked(target, pc))
        {
            __instance.currentTarget = null;
            __instance.buttonLabelText.text = Tr.Get(TrKey.ButtonBlocked);
            __instance.enabled = false;
            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 0f);
            return false;
        }

        __instance.currentTarget = target;
        return true;
    }

    public static void VentButtonSetTarget(VentButton __instance)
    {
        // Trickster render special vent button
        var lp = PlayerControl.LocalPlayer;
        if (lp.IsRole(RoleType.Trickster))
        {
            if (DefaultVentSprite == null) DefaultVentSprite = __instance.graphic.sprite;
            bool isSpecialVent = __instance.currentTarget != null && __instance.currentTarget.gameObject != null && __instance.currentTarget.gameObject.name.StartsWith("JackInTheBoxVent_");
            __instance.graphic.sprite = isSpecialVent ? AssetLoader.TricksterVentButton : DefaultVentSprite;
            __instance.buttonLabelText.enabled = !isSpecialVent;
        }
    }

    public static bool CanUse(ref float __result, Console __instance, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
    {
        canUse = couldUse = false;
        __result = float.MaxValue;

        if (IsBlocked(__instance, pc.Object)) return false;
        if (__instance.AllowImpostor) return true;
        if (!pc.Object.HasFakeTasks()) return true;

        return false;
    }

    public static bool IsBlocked(PlayerTask task, PlayerControl pc)
    {
        if (task == null || pc == null || pc != PlayerControl.LocalPlayer) return false;

        var taskType = task.TaskType;
        bool isLights = taskType == TaskTypes.FixLights;
        bool isComms = taskType == TaskTypes.FixComms;
        bool isReactor = taskType is TaskTypes.StopCharles or TaskTypes.ResetSeismic or TaskTypes.ResetReactor;
        bool isO2 = taskType == TaskTypes.RestoreOxy;

        if (pc.IsRole(RoleType.NiceSwapper) && (isLights || isComms))
        {
            return true;
        }

        if (pc.HasModifier(ModifierType.Madmate) && (isLights || (isComms && !Madmate.CanFixComm)))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Madmate) && (isLights || (isComms && !MadmateRole.CanFixComm)))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Suicider) && (isLights || (isComms && !Suicider.CanFixComm)))
        {
            return true;
        }

        if (pc.HasModifier(ModifierType.CreatedMadmate) && (isLights || (isComms && !CreatedMadmate.CanFixComm)))
        {
            return true;
        }

        if (pc.IsGM() && (isLights || isComms || isReactor || isO2))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Mafioso) && !Mafia.Mafioso.CanRepair && (isLights || isComms))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Janitor) && !Mafia.Janitor.CanRepair && (isLights || isComms))
        {
            return true;
        }

        return false;
    }

    public static bool IsBlocked(Console console, PlayerControl pc)
    {
        if (console == null || pc == null || pc != PlayerControl.LocalPlayer)
        {
            return false;
        }

        PlayerTask task = console.FindTask(pc);
        return IsBlocked(task, pc);
    }

    public static bool IsBlocked(SystemConsole console, PlayerControl pc)
    {
        if (console == null || pc == null || pc != PlayerControl.LocalPlayer)
        {
            return false;
        }

        string name = console.name;
        bool isSecurity = name is "task_cams" or "Surv_Panel" or "SurvLogConsole" or "SurvConsole";
        bool isVitals = name == "panel_vitals";

        if ((isSecurity && !MapSettings.CanUseCameras) || (isVitals && !MapSettings.CanUseVitals)) return true;
        return false;
    }

    public static bool IsBlocked(IUsable target, PlayerControl pc)
    {
        if (target == null) return false;

        var targetConsole = target.TryCast<Console>();
        if (targetConsole != null) return IsBlocked(targetConsole, pc);

        var targetSysConsole = target.TryCast<SystemConsole>();
        if (targetSysConsole != null) return IsBlocked(targetSysConsole, pc);

        var targetMapConsole = target.TryCast<MapConsole>();
        if (targetMapConsole != null) return !MapSettings.CanUseAdmin;

        return false;
    }

    public static void EmergencyMinigameUpdate(EmergencyMinigame __instance)
    {
        var lp = PlayerControl.LocalPlayer;
        if (lp == null) return;

        var roleCanCallEmergency = true;
        var statusTextKey = "";

        if (lp.IsRole(RoleType.Jester) && !Jester.CanCallEmergency)
        {
            roleCanCallEmergency = false;
            statusTextKey = "JesterMeetingButton";
        }

        if (lp.IsRole(RoleType.NiceSwapper) && !Swapper.CanCallEmergency)
        {
            roleCanCallEmergency = false;
            statusTextKey = "SwapperMeetingButton";
        }

        if (!roleCanCallEmergency)
        {
            __instance.StatusText.text = Tr.GetDynamic(statusTextKey);
            __instance.NumberText.text = string.Empty;
            __instance.ClosedLid.gameObject.SetActive(true);
            __instance.OpenLid.gameObject.SetActive(false);
            __instance.ButtonActive = false;
            return;
        }

        // Handle max number of meetings
        if (__instance.state == 1)
        {
            int localRemaining = lp.RemainingEmergencies;
            int teamRemaining = Mathf.Max(0, MapSettings.MaxNumberOfMeetings - MapSettings.MeetingsCount);
            int remaining = Mathf.Min(localRemaining, lp.IsRole(RoleType.Mayor) ? 1 : teamRemaining);

            EmergencyStringBuilder.Clear();
            EmergencyStringBuilder.Append("<size=100%> ");
            EmergencyStringBuilder.Append(string.Format(Tr.Get(TrKey.MeetingStatus), lp.name));
            EmergencyStringBuilder.Append("</size>");
            __instance.StatusText.text = EmergencyStringBuilder.ToString();

            EmergencyStringBuilder.Clear();
            EmergencyStringBuilder.Append(string.Format(Tr.Get(TrKey.MeetingCount), localRemaining.ToString(), teamRemaining.ToString()));
            __instance.NumberText.text = EmergencyStringBuilder.ToString();

            __instance.ButtonActive = remaining > 0;
            __instance.ClosedLid.gameObject.SetActive(!__instance.ButtonActive);
            __instance.OpenLid.gameObject.SetActive(__instance.ButtonActive);
            return;
        }
    }
}