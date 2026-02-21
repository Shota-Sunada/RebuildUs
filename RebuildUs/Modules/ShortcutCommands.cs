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

    internal static void OpenAirshipToilet()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 79);
            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 80);
            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 81);
            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 82);
        }
    }
}