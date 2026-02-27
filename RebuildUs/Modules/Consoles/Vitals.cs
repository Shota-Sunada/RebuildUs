namespace RebuildUs.Modules.Consoles;

internal static class Vitals
{
    private static float _vitalsTimer;
    private static TextMeshPro _timeRemaining;
    private static List<TextMeshPro> _hackerTexts = [];

    private static readonly StringBuilder VitalsStringBuilder = new();

    internal static void ResetData()
    {
        _vitalsTimer = 0f;
        if (_timeRemaining == null)
        {
            return;
        }
        UnityObject.Destroy(_timeRemaining);
        _timeRemaining = null;
    }

    private static void UseVitalsTime()
    {
        // Don't waste network traffic if we're out of time.
        if (MapSettings.RestrictDevices > 0
            && MapSettings.RestrictVitals
            && MapSettings.RestrictVitalsTime > 0f
            && PlayerControl.LocalPlayer.IsAlive())
        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UseVitalsTime);
            sender.Write(_vitalsTimer);
            RPCProcedure.UseVitalsTime(_vitalsTimer);
        }

        _vitalsTimer = 0f;
    }

    internal static void Begin(VitalsMinigame __instance)
    {
        _vitalsTimer = 0f;

        if (!PlayerControl.LocalPlayer.IsRole(RoleType.Hacker))
        {
            return;
        }
        _hackerTexts = [];
        foreach (VitalsPanel panel in __instance.vitals)
        {
            TextMeshPro text = UnityObject.Instantiate(__instance.SabText, panel.transform);
            _hackerTexts.Add(text);
            UnityObject.DestroyImmediate(text.GetComponent<AlphaBlink>());
            text.gameObject.SetActive(false);
            text.transform.localScale = Vector3.one * 0.75f;
            text.transform.localPosition = new(-0.75f, -0.23f, 0f);
        }
    }

    internal static bool UpdatePrefix(VitalsMinigame __instance)
    {
        _vitalsTimer += Time.deltaTime;
        if (_vitalsTimer > 0.05f)
        {
            UseVitalsTime();
        }

        if (MapSettings.RestrictDevices <= 0 || !MapSettings.RestrictVitals)
        {
            return true;
        }
        if (_timeRemaining == null)
        {
            _timeRemaining = UnityObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
            _timeRemaining.alignment = TextAlignmentOptions.BottomRight;
            _timeRemaining.transform.position = Vector3.zero;
            _timeRemaining.transform.localPosition = new(1.7f, 4.45f);
            _timeRemaining.transform.localScale *= 1.8f;
            _timeRemaining.color = Palette.White;
        }

        if (MapSettings.RestrictVitalsTime <= 0f)
        {
            __instance.Close();
            return false;
        }

        VitalsStringBuilder.Clear();
        TimeSpan ts = TimeSpan.FromSeconds(MapSettings.RestrictVitalsTime);
        if (ts.TotalHours >= 1)
        {
            VitalsStringBuilder.Append((int)ts.TotalHours).Append(':');
        }
        VitalsStringBuilder
            .Append(ts.Minutes.ToString("D2"))
            .Append(':')
            .Append(ts.Seconds.ToString("D2"))
            .Append('.')
            .Append((ts.Milliseconds / 10).ToString("D2"));

        string timeString = VitalsStringBuilder.ToString();
        VitalsStringBuilder.Clear();
        VitalsStringBuilder.Append(string.Format(Tr.Get(TrKey.TimeRemaining), timeString));
        _timeRemaining.text = VitalsStringBuilder.ToString();
        _timeRemaining.gameObject.SetActive(true);

        return true;
    }

    internal static void UpdatePostfix(VitalsMinigame __instance)
    {
        // Hacker show time since death
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && Hacker.HackerTimer > 0)
        {
            for (int k = 0; k < __instance.vitals.Length; k++)
            {
                VitalsPanel vitalsPanel = __instance.vitals[k];
                NetworkedPlayerInfo player = GameData.Instance.AllPlayers[k];

                // Hacker update
                if (!vitalsPanel.IsDead)
                {
                    continue;
                }
                DeadPlayer deadPlayer = GameHistory.GetDeadPlayer(player.PlayerId);
                if (deadPlayer == null || k >= _hackerTexts.Count || _hackerTexts[k] == null)
                {
                    continue;
                }
                float timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.TimeOfDeath).TotalMilliseconds;
                _hackerTexts[k].gameObject.SetActive(true);
                VitalsStringBuilder.Clear();
                VitalsStringBuilder.Append(Math.Round(timeSinceDeath / 1000)).Append('s');
                _hackerTexts[k].text = VitalsStringBuilder.ToString();
            }
        }
        else
        {
            foreach (TextMeshPro text in _hackerTexts)
            {
                if (text != null && text.gameObject != null)
                {
                    text.gameObject.SetActive(false);
                }
            }
        }
    }
}