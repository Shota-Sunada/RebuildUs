namespace RebuildUs.Modules;

public static class ShortcutCommands
{
    public static void HostCommands()
    {
        if (!AmongUsClient.Instance.AmHost) return;

        if (Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F5) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F5))
        {
            GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ForceEnd, false);
        }

        if ((Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F6) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F6)) && MeetingHud.Instance)
        {
            MeetingHud.Instance.RpcClose();
        }

        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && Helpers.IsCountdown)
        {
            GameStartManager.Instance.countDownTimer = 0;
            SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.StopStart);
            }
        }
    }

    public static void OpenAirshipToilet()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Doors, 79);
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Doors, 80);
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Doors, 81);
            ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Doors, 82);
        }
    }
}