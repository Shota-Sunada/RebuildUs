namespace RebuildUs.Modules;

internal static class ShortcutCommands
{
    internal static void HostCommands()
    {
        if (!AmongUsClient.Instance.AmHost) return;

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F5) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F5)) GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ForceEnd, false);

        if ((Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F6) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F6)) && MeetingHud.Instance && Helpers.GameStarted) MeetingHud.Instance.RpcClose();

        if ((Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F7) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F7)) && !MeetingHud.Instance && Helpers.GameStarted) MapUtilities.CachedShipStatus.StartMeeting(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data);

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && Helpers.IsCountdown)
        {
            GameStartManager.Instance.countDownTimer = 0;
            SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
            {
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.StopStart);
            }
        }

#if DEBUG
        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F4) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F4))
        {
            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Reloaded Random Number Generation Algorithm.");
            RebuildUs.RefreshRnd((int)DateTime.Now.Ticks);
        }

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F8) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F8))
        {
            HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Measured Random Number Quality. Check logs for details.");
            RandomMain.LogScore();
        }
#endif
    }

    internal static void DebugCommands()
    {
        bool trigger = Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F9) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F9);
        if (!trigger) return;

        HudManager hud = HudManager.Instance;
        PlayerControl localPlayer = PlayerControl.LocalPlayer;

        if (localPlayer == null || hud?.Chat == null) return;
        if (!Helpers.GameStarted)
        {
            Logger.LogInfo("DeathPopup debug failed: game is not started.");
            return;
        }

        int result = DeathPopup.TryShow(localPlayer, out HideAndSeekDeathPopup popup);
        string reason = DeathPopup.ExplainResult(result);
        if (result != DeathPopup.RESULT_SUCCESS)
        {
            Logger.LogInfo($"DeathPopup debug result={result} ({reason})");
            return;
        }

        if (popup == null)
        {
            Logger.LogInfo("DeathPopup debug success via fallback path (popup instance unavailable for text verification).");
            return;
        }

        Logger.LogInfo("DeathPopup debug success.");
    }

    internal static void OpenAirshipToilet()
    {
        if (!Input.GetKeyDown(KeyCode.P)) return;
        MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 79);
        MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 80);
        MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 81);
        MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 82);
    }
}
