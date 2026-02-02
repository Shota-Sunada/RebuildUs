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

        if (Helpers.GetKeysDown(KeyCode.LeftShift, KeyCode.F3) || Helpers.GetKeysDown(KeyCode.RightShift, KeyCode.F3))
        {
            var linked = new List<string>();
            var unlinked = new List<string>();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.Data == null) continue;
                var name = p.Data.PlayerName;
                var id = DiscordModManager.GetIdentifier(p);
                if (id != null && DiscordModManager.PlayerMappings.ContainsKey(id)) linked.Add(name);
                else unlinked.Add(name);
            }

            if (HudManager.Instance && HudManager.Instance.Chat)
            {
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Discord 連携状況:");
                if (linked.Count > 0) HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"連携済み: {string.Join(", ", linked)}");
                if (unlinked.Count > 0) HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"未連携: {string.Join(", ", unlinked)}");
            }
        }

        if ((Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F6) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F6)) && MeetingHud.Instance && Helpers.GameStarted)
        {
            MeetingHud.Instance.RpcClose();
        }

        if ((Helpers.GetKeysDown(KeyCode.LeftControl, KeyCode.F7) || Helpers.GetKeysDown(KeyCode.RightControl, KeyCode.F7)) && !MeetingHud.Instance && Helpers.GameStarted)
        {
            MapUtilities.CachedShipStatus.StartMeeting(PlayerControl.LocalPlayer, null);
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
            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 79);
            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 80);
            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 81);
            MapUtilities.CachedShipStatus.RpcUpdateSystem(SystemTypes.Doors, 82);
        }
    }
}