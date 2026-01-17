using RebuildUs.Roles.Neutral;

namespace RebuildUs.Modules;

public static class Usables
{
    private static Sprite defaultVentSprite = null;

    public static void FixedUpdate(PlayerControl __instance)
    {
        if (__instance.AmOwner && Helpers.ShowButtons)
        {
            FastDestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Hide();
            FastDestroyableSingleton<HudManager>.Instance.SabotageButton.Hide();

            if (Helpers.ShowButtons)
            {
                if (__instance.roleCanUseVents())
                {
                    FastDestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Show();
                }

                if (__instance.roleCanSabotage())
                {
                    FastDestroyableSingleton<HudManager>.Instance.SabotageButton.Show();
                    FastDestroyableSingleton<HudManager>.Instance.SabotageButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public static bool KillButtonDoClick(KillButton __instance)
    {
        if (__instance.isActiveAndEnabled && __instance.currentTarget && !__instance.isCoolingDown && CachedPlayer.LocalPlayer.PlayerControl.isAlive() && CachedPlayer.LocalPlayer.PlayerControl.CanMove)
        {
            bool showAnimation = true;
            if (CachedPlayer.LocalPlayer.PlayerControl.isRole(ERoleType.Ninja) && Ninja.isStealthed(CachedPlayer.LocalPlayer.PlayerControl))
            {
                showAnimation = false;
            }

            // Use an unchecked kill command, to allow shorter kill cooldowns etc. without getting kicked
            var res = Helpers.checkMuderAttemptAndKill(CachedPlayer.LocalPlayer.PlayerControl, __instance.currentTarget, showAnimation: showAnimation);
            // Handle blank kill
            if (res == MurderAttemptResult.BlankKill)
            {
                CachedPlayer.LocalPlayer.PlayerControl.killTimer = PlayerControl.GameOptions.KillCooldown;
                if (CachedPlayer.LocalPlayer.PlayerControl == Cleaner.cleaner)
                    Cleaner.cleaner.killTimer = HudManagerStartPatch.cleanerCleanButton.Timer = HudManagerStartPatch.cleanerCleanButton.MaxTimer;
                else if (CachedPlayer.LocalPlayer.PlayerControl == Warlock.warlock)
                    Warlock.warlock.killTimer = HudManagerStartPatch.warlockCurseButton.Timer = HudManagerStartPatch.warlockCurseButton.MaxTimer;
                else if (CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.Mini) && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor)
                    CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(PlayerControl.GameOptions.KillCooldown * (Mini.isGrownUp(CachedPlayer.LocalPlayer.PlayerControl) ? 0.66f : 2f));
                else if (CachedPlayer.LocalPlayer.PlayerControl == Witch.witch)
                    Witch.witch.killTimer = HudManagerStartPatch.witchSpellButton.Timer = HudManagerStartPatch.witchSpellButton.MaxTimer;
                else if (CachedPlayer.LocalPlayer.PlayerControl == Assassin.assassin)
                    Assassin.assassin.killTimer = HudManagerStartPatch.assassinButton.Timer = HudManagerStartPatch.assassinButton.MaxTimer;
            }

            __instance.SetTarget(null);
        }
        return false;
    }

    public static bool UseButtonSetTarget(UseButton __instance, IUsable target)
    {
        var pc = CachedPlayer.LocalPlayer.PlayerControl;
        __instance.enabled = true;

        if (IsBlocked(target, pc))
        {
            __instance.currentTarget = null;
            __instance.buttonLabelText.text = Tr.Get("buttonBlocked");
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
        if (Trickster.trickster != null && Trickster.trickster == CachedPlayer.LocalPlayer.PlayerControl)
        {
            if (defaultVentSprite == null) defaultVentSprite = __instance.graphic.sprite;
            bool isSpecialVent = __instance.currentTarget != null && __instance.currentTarget.gameObject != null && __instance.currentTarget.gameObject.name.StartsWith("JackInTheBoxVent_");
            __instance.graphic.sprite = isSpecialVent ? Trickster.getTricksterVentButtonSprite() : defaultVentSprite;
            __instance.buttonLabelText.enabled = !isSpecialVent;
        }
    }

    public static bool CanUse(ref float __result, Console __instance, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
    {
        canUse = couldUse = false;
        __result = float.MaxValue;

        //if (IsBlocked(__instance, pc.Object)) return false;
        if (__instance.AllowImpostor) return true;
        if (!pc.Object.HasFakeTasks()) return true;

        return false;
    }

    public static bool IsBlocked(PlayerTask task, PlayerControl pc)
    {
        if (task == null || pc == null || pc != CachedPlayer.LocalPlayer.PlayerControl) return false;

        bool isLights = task.TaskType == TaskTypes.FixLights;
        bool isComms = task.TaskType == TaskTypes.FixComms;
        bool isReactor = task.TaskType is TaskTypes.StopCharles or TaskTypes.ResetSeismic or TaskTypes.ResetReactor;
        bool isO2 = task.TaskType == TaskTypes.RestoreOxy;

        if (pc.isRole(RoleType.Swapper) && (isLights || isComms))
        {
            return true;
        }

        if (pc.hasModifier(ModifierType.Madmate) && (isLights || (isComms && !Madmate.canFixComm)))
        {
            return true;
        }

        if (pc.hasModifier(ModifierType.CreatedMadmate) && (isLights || (isComms && !CreatedMadmate.canFixComm)))
        {
            return true;
        }

        if (pc.isGM() && (isLights || isComms || isReactor || isO2))
        {
            return true;
        }

        if (pc.isRole(RoleType.Mafioso) && !Mafioso.canRepair && (isLights || isComms))
        {
            return true;
        }

        if (pc.isRole(RoleType.Janitor) && !Janitor.canRepair && (isLights || isComms))
        {
            return true;
        }

        if (pc.isRole(RoleType.Fox) && (isLights || isComms || isReactor || isO2))
        {
            if (isReactor)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public static bool IsBlocked(Console console, PlayerControl pc)
    {
        if (console == null || pc == null || pc != CachedPlayer.LocalPlayer.PlayerControl)
        {
            return false;
        }

        PlayerTask task = console.FindTask(pc);
        return IsBlocked(task, pc);
    }

    public static bool IsBlocked(SystemConsole console, PlayerControl pc)
    {
        if (console == null || pc == null || pc != CachedPlayer.LocalPlayer.PlayerControl)
        {
            return false;
        }

        string name = console.name;
        bool isSecurity = name is "task_cams" or "Surv_Panel" or "SurvLogConsole" or "SurvConsole";
        bool isVitals = name == "panel_vitals";
        bool isButton = name is "EmergencyButton" or "EmergencyConsole" or "task_emergency";

        if ((isSecurity && !MapOptions.canUseCameras) || (isVitals && !MapOptions.canUseVitals)) return true;
        return false;
    }

    public static bool IsBlocked(IUsable target, PlayerControl pc)
    {
        if (target == null) return false;

        Console targetConsole = target.TryCast<Console>();
        SystemConsole targetSysConsole = target.TryCast<SystemConsole>();
        MapConsole targetMapConsole = target.TryCast<MapConsole>();

        // Hydeの時にはタスクができない
        if (CachedPlayer.LocalPlayer.PlayerControl.isRole(RoleType.JekyllAndHyde) && !JekyllAndHyde.isJekyll())
        {
            string name = targetSysConsole == null ? "" : targetSysConsole.name;
            bool isSecurity = name is "task_cams" or "Surv_Panel" or "SurvLogConsole" or "SurvConsole";
            bool isVitals = name == "panel_vitals";
            bool isButton = name is "EmergencyButton" or "EmergencyConsole" or "task_emergency";
            PlayerTask task = targetConsole.FindTask(pc);
            bool isLights = task?.TaskType == TaskTypes.FixLights;
            bool isComms = task?.TaskType == TaskTypes.FixComms;
            bool isReactor = task?.TaskType is TaskTypes.StopCharles or TaskTypes.ResetSeismic or TaskTypes.ResetReactor;
            bool isO2 = task?.TaskType == TaskTypes.RestoreOxy;
            if (!isSecurity || !isVitals || !isButton || !isLights || !isComms || !isReactor || !isO2)
            {
                return true;
            }
        }

        if ((targetConsole != null && IsBlocked(targetConsole, pc)) ||
            (targetSysConsole != null && IsBlocked(targetSysConsole, pc)) ||
            (targetMapConsole != null && !MapOptions.canUseAdmin))
        {
            return true;
        }
        return false;
    }

    public static void EmergencyMinigameUpdate(EmergencyMinigame __instance)
    {
        var roleCanCallEmergency = true;
        var statusText = "";

        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Jester) && !Jester.CanCallEmergency)
        {
            roleCanCallEmergency = false;
            statusText = Tr.Get("jesterMeetingButton");
        }

        if (!roleCanCallEmergency)
        {
            __instance.StatusText.text = statusText;
            __instance.NumberText.text = string.Empty;
            __instance.ClosedLid.gameObject.SetActive(true);
            __instance.OpenLid.gameObject.SetActive(false);
            __instance.ButtonActive = false;
            return;
        }

        // Handle max number of meetings
        if (__instance.state == 1)
        {
            int localRemaining = CachedPlayer.LocalPlayer.PlayerControl.RemainingEmergencies;
            int teamRemaining = Mathf.Max(0, MapOptions.MaxNumberOfMeetings - MapOptions.MeetingsCount);
            int remaining = Mathf.Min(localRemaining, CachedPlayer.LocalPlayer.PlayerControl.IsRole(ERoleType.Mayor) ? 1 : teamRemaining);

            __instance.StatusText.text = $"<size=100%> {string.Format(Tr.Get("meetingStatus"), CachedPlayer.LocalPlayer.PlayerControl.name)}</size>";
            __instance.NumberText.text = string.Format(Tr.Get("meetingCount"), localRemaining.ToString(), teamRemaining.ToString());
            __instance.ButtonActive = remaining > 0;
            __instance.ClosedLid.gameObject.SetActive(!__instance.ButtonActive);
            __instance.OpenLid.gameObject.SetActive(__instance.ButtonActive);
            return;
        }
    }
}