namespace RebuildUs.Modules;

public static class ShortcutCommands
{
    public static void HostCommands()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        if (AmongUsClient.Instance.GameState is not InnerNet.InnerNetClient.GameStates.Started) return;

        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F5))
        {
            GameManager.Instance.RpcEndGame((GameOverReason)ECustomGameOverReason.ForceEnd, false);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F6) && MeetingHud.Instance)
        {
            MeetingHud.Instance.RpcClose();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && Helpers.IsCountdown)
        {
            GameStartManager.Instance.countDownTimer = 0;
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