using Object = UnityEngine.Object;

namespace RebuildUs.Modules.Consoles;

public static class Vitals
{
    private static float _vitalsTimer;
    private static TextMeshPro _timeRemaining;
    private static List<TextMeshPro> _hackerTexts = [];

    private static readonly StringBuilder VITALS_STRING_BUILDER = new();

    public static void ResetData()
    {
        _vitalsTimer = 0f;
        if (_timeRemaining != null)
        {
            Object.Destroy(_timeRemaining);
            _timeRemaining = null;
        }
    }

    private static void UseVitalsTime()
    {
        // Don't waste network traffic if we're out of time.
        if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictVitals && MapSettings.RestrictVitalsTime > 0f && PlayerControl.LocalPlayer.IsAlive())
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UseVitalsTime);
            sender.Write(_vitalsTimer);
            RPCProcedure.UseVitalsTime(_vitalsTimer);
        }

        _vitalsTimer = 0f;
    }

    public static void Begin(VitalsMinigame __instance)
    {
        _vitalsTimer = 0f;

        if (PlayerControl.LocalPlayer.IsRole(RoleType.Hacker))
        {
            _hackerTexts = [];
            foreach (var panel in __instance.vitals)
            {
                var text = Object.Instantiate(__instance.SabText, panel.transform);
                _hackerTexts.Add(text);
                Object.DestroyImmediate(text.GetComponent<AlphaBlink>());
                text.gameObject.SetActive(false);
                text.transform.localScale = Vector3.one * 0.75f;
                text.transform.localPosition = new(-0.75f, -0.23f, 0f);
            }
        }
    }

    public static bool UpdatePrefix(VitalsMinigame __instance)
    {
        _vitalsTimer += Time.deltaTime;
        if (_vitalsTimer > 0.05f)
            UseVitalsTime();

        if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictVitals)
        {
            if (_timeRemaining == null)
            {
                _timeRemaining = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
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

            VITALS_STRING_BUILDER.Clear();
            var ts = TimeSpan.FromSeconds(MapSettings.RestrictVitalsTime);
            if (ts.TotalHours >= 1) VITALS_STRING_BUILDER.Append((int)ts.TotalHours).Append(':');
            VITALS_STRING_BUILDER.Append(ts.Minutes.ToString("D2")).Append(':').Append(ts.Seconds.ToString("D2")).Append('.').Append((ts.Milliseconds / 10).ToString("D2"));

            var timeString = VITALS_STRING_BUILDER.ToString();
            VITALS_STRING_BUILDER.Clear();
            VITALS_STRING_BUILDER.Append(string.Format(Tr.Get(TrKey.TimeRemaining), timeString));
            _timeRemaining.text = VITALS_STRING_BUILDER.ToString();
            _timeRemaining.gameObject.SetActive(true);
        }

        return true;
    }

    public static void UpdatePostfix(VitalsMinigame __instance)
    {
        // Hacker show time since death
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && Hacker.HackerTimer > 0)
        {
            for (var k = 0; k < __instance.vitals.Length; k++)
            {
                var vitalsPanel = __instance.vitals[k];
                var player = GameData.Instance.AllPlayers[k];

                // Hacker update
                if (vitalsPanel.IsDead)
                {
                    var deadPlayer = GameHistory.GetDeadPlayer(player.PlayerId);
                    if (deadPlayer != null && k < _hackerTexts.Count && _hackerTexts[k] != null)
                    {
                        var timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.TimeOfDeath).TotalMilliseconds;
                        _hackerTexts[k].gameObject.SetActive(true);
                        VITALS_STRING_BUILDER.Clear();
                        VITALS_STRING_BUILDER.Append(Math.Round(timeSinceDeath / 1000)).Append('s');
                        _hackerTexts[k].text = VITALS_STRING_BUILDER.ToString();
                    }
                }
            }
        }
        else
        {
            foreach (var text in _hackerTexts)
            {
                if (text != null && text.gameObject != null)
                    text.gameObject.SetActive(false);
            }
        }
    }
}
