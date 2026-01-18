namespace RebuildUs.Modules;

public static class Usables
{
    private static Sprite DefaultVentSprite = null;

    public static void FixedUpdate(PlayerControl __instance)
    {
        if (__instance.AmOwner && Helpers.ShowButtons)
        {
            FastDestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Hide();
            FastDestroyableSingleton<HudManager>.Instance.SabotageButton.Hide();

            if (Helpers.ShowButtons)
            {
                if (__instance.CanUseVents())
                {
                    FastDestroyableSingleton<HudManager>.Instance.ImpostorVentButton.Show();
                }

                if (__instance.CanUseVents())
                {
                    FastDestroyableSingleton<HudManager>.Instance.SabotageButton.Show();
                    FastDestroyableSingleton<HudManager>.Instance.SabotageButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public static bool KillButtonDoClick(KillButton __instance)
    {
        if (__instance.isActiveAndEnabled && __instance.currentTarget && !__instance.isCoolingDown && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && CachedPlayer.LocalPlayer.PlayerControl.CanMove)
        {
            bool showAnimation = true;

            // Use an unchecked kill command, to allow shorter kill cooldowns etc. without getting kicked
            var res = Helpers.CheckMurderAttemptAndKill(CachedPlayer.LocalPlayer.PlayerControl, __instance.currentTarget, showAnimation: showAnimation);
            // Handle blank kill
            if (res == MurderAttemptResult.BlankKill)
            {
                CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(Helpers.GetOption(FloatOptionNames.KillCooldown));
                if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Cleaner))
                {
                    CachedPlayer.LocalPlayer.PlayerControl.killTimer = Cleaner.CleanerCleanButton.Timer = Cleaner.CleanerCleanButton.MaxTimer;
                }
                else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Warlock))
                {
                    CachedPlayer.LocalPlayer.PlayerControl.killTimer = Warlock.WarlockCurseButton.Timer = Warlock.WarlockCurseButton.MaxTimer;
                }
                else if (CachedPlayer.LocalPlayer.PlayerControl.HasModifier(ModifierType.Mini) && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor)
                {
                    CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(Helpers.GetOption(FloatOptionNames.KillCooldown) * (Mini.IsGrownUp(CachedPlayer.LocalPlayer.PlayerControl) ? 0.66f : 2f));
                }
                else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Witch))
                {
                    CachedPlayer.LocalPlayer.PlayerControl.killTimer = Witch.WitchSpellButton.Timer = Witch.WitchSpellButton.MaxTimer;
                }
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
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Trickster))
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

        if (pc.IsRole(RoleType.Swapper) && (isLights || isComms))
        {
            return true;
        }

        if (pc.HasModifier(ModifierType.Madmate) && (isLights || (isComms && !Madmate.CanFixComm)))
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

        if ((isSecurity && !ModMapOptions.CanUseCameras) || (isVitals && !ModMapOptions.CanUseVitals)) return true;
        return false;
    }

    public static bool IsBlocked(IUsable target, PlayerControl pc)
    {
        if (target == null) return false;

        var targetConsole = target.TryCast<Console>();
        var targetSysConsole = target.TryCast<SystemConsole>();
        var targetMapConsole = target.TryCast<MapConsole>();

        return (targetConsole != null && IsBlocked(targetConsole, pc)) ||
            (targetSysConsole != null && IsBlocked(targetSysConsole, pc)) ||
            (targetMapConsole != null && !ModMapOptions.CanUseAdmin);
    }

    public static void EmergencyMinigameUpdate(EmergencyMinigame __instance)
    {
        var roleCanCallEmergency = true;
        var statusText = "";

        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Jester) && !Jester.CanCallEmergency)
        {
            roleCanCallEmergency = false;
            statusText = Tr.Get("jesterMeetingButton");
        }

        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Swapper) && !Swapper.CanCallEmergency)
        {
            roleCanCallEmergency = false;
            statusText = Tr.Get("swapperMeetingButton");
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
            int teamRemaining = Mathf.Max(0, ModMapOptions.MaxNumberOfMeetings - ModMapOptions.MeetingsCount);
            int remaining = Mathf.Min(localRemaining, CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Mayor) ? 1 : teamRemaining);

            __instance.StatusText.text = $"<size=100%> {string.Format(Tr.Get("meetingStatus"), CachedPlayer.LocalPlayer.PlayerControl.name)}</size>";
            __instance.NumberText.text = string.Format(Tr.Get("meetingCount"), localRemaining.ToString(), teamRemaining.ToString());
            __instance.ButtonActive = remaining > 0;
            __instance.ClosedLid.gameObject.SetActive(!__instance.ButtonActive);
            __instance.OpenLid.gameObject.SetActive(__instance.ButtonActive);
            return;
        }
    }
}