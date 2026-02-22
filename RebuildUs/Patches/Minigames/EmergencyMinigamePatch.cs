namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class EmergencyMinigamePatch
{
    private static readonly StringBuilder EmergencyStringBuilder = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
    internal static void UpdatePostfix(EmergencyMinigame __instance)
    {
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (lp == null) return;

        bool roleCanCallEmergency = true;
        TrKey statusTextKey = TrKey.None;

        if (lp.IsRole(RoleType.Jester) && !Jester.CanCallEmergency)
        {
            roleCanCallEmergency = false;
            statusTextKey = TrKey.JesterMeetingButton;
        }

        if (lp.IsRole(RoleType.NiceSwapper) && !Swapper.CanCallEmergency)
        {
            roleCanCallEmergency = false;
            statusTextKey = TrKey.SwapperMeetingButton;
        }

        if (!roleCanCallEmergency)
        {
            __instance.StatusText.text = Tr.Get(statusTextKey);
            __instance.NumberText.text = string.Empty;
            __instance.ClosedLid.gameObject.SetActive(true);
            __instance.OpenLid.gameObject.SetActive(false);
            __instance.ButtonActive = false;
            return;
        }

        // Handle max number of meetings
        if (__instance.state != 1) return;
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
    }
}