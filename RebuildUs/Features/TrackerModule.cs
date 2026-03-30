namespace RebuildUs.Features;

internal static class TrackerModule
{
    public static void SetupButtons(MeetingHud __instance)
    {
        var isTrackerButton = EvilTracker.CanSetTargetOnMeeting
                            && EvilTracker.Target == null
                            && PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker)
                            && PlayerControl.LocalPlayer.IsAlive();
        if (!isTrackerButton) return;

        for (var i = 0; i < __instance.playerStates.Length; i++)
        {
            var playerVoteArea = __instance.playerStates[i];
            if (playerVoteArea.AmDead || playerVoteArea.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId)
            {
                continue;
            }
            var template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
            var targetBox = UnityObject.Instantiate(template, playerVoteArea.transform);
            targetBox.name = "EvilTrackerButton";
            targetBox.transform.localPosition = new(-0.95f, 0.03f, -1.3f);
            var renderer = targetBox.GetComponent<SpriteRenderer>();
            renderer.sprite = AssetLoader.Arrow;
            renderer.color = Palette.CrewmateBlue;
            var button = targetBox.GetComponent<PassiveButton>();
            button.OnClick.RemoveAllListeners();
            var copiedIndex = i;
            button.OnClick.AddListener((Action)(() =>
            {
                if (__instance == null || __instance.playerStates == null || copiedIndex < 0 || copiedIndex >= __instance.playerStates.Length)
                {
                    return;
                }
                var focusedTarget = Helpers.PlayerById(__instance.playerStates[copiedIndex].TargetPlayerId);
                EvilTracker.Target = focusedTarget;
                // Reset the GUI
                foreach (var x in __instance.playerStates)
                {
                    var eb = x.transform.FindChild("EvilTrackerButton");
                    if (eb != null)
                    {
                        UnityObject.Destroy(eb.gameObject);
                    }
                }
                var targetMark = UnityObject.Instantiate(template, playerVoteArea.transform);
                targetMark.name = "EvilTrackerMark";
                var passiveButton = targetMark.GetComponent<PassiveButton>();
                targetMark.transform.localPosition = new(1.1f, 0.03f, -20f);
                UnityObject.Destroy(passiveButton);
                var spriteRenderer = targetMark.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = AssetLoader.Arrow;
                spriteRenderer.color = Palette.CrewmateBlue;

                var isGuesserButton = Guesser.IsGuesser(PlayerControl.LocalPlayer.PlayerId) && PlayerControl.LocalPlayer.IsAlive() && Guesser.RemainingShots(PlayerControl.LocalPlayer) > 0;
                if (isGuesserButton)
                {
                    GuesserModule.CreateGuesserButton(__instance);
                }
            }));
        }
    }

    public static bool IsTrackerButtonActive()
    {
        return EvilTracker.CanSetTargetOnMeeting
            && EvilTracker.Target == null
            && PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker)
            && PlayerControl.LocalPlayer.IsAlive();
    }
}