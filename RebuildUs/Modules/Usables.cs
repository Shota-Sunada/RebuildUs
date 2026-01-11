using RebuildUs.Roles.Neutral;

namespace RebuildUs.Modules;

public static class Usables
{
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