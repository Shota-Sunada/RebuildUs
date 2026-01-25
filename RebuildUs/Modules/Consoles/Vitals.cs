namespace RebuildUs.Modules.Consoles;

public static class Vitals
{
    static float VitalsTimer = 0f;
    static TextMeshPro TimeRemaining;
    private static List<TextMeshPro> HackerTexts = [];

    public static void ResetData()
    {
        VitalsTimer = 0f;
        if (TimeRemaining != null)
        {
            UnityEngine.Object.Destroy(TimeRemaining);
            TimeRemaining = null;
        }
    }

    static void UseVitalsTime()
    {
        // Don't waste network traffic if we're out of time.
        if (ModMapOptions.RestrictDevices > 0 && ModMapOptions.RestrictVitals && ModMapOptions.RestrictVitalsTime > 0f && PlayerControl.LocalPlayer.IsAlive())
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UseVitalsTime);
            sender.Write(VitalsTimer);
            RPCProcedure.UseVitalsTime(VitalsTimer);
        }
        VitalsTimer = 0f;
    }

    public static void Begin(VitalsMinigame __instance)
    {
        VitalsTimer = 0f;

        if (PlayerControl.LocalPlayer.IsRole(RoleType.Hacker))
        {
            HackerTexts = [];
            foreach (VitalsPanel panel in __instance.vitals)
            {
                TextMeshPro text = UnityEngine.Object.Instantiate(__instance.SabText, panel.transform);
                HackerTexts.Add(text);
                UnityEngine.Object.DestroyImmediate(text.GetComponent<AlphaBlink>());
                text.gameObject.SetActive(false);
                text.transform.localScale = Vector3.one * 0.75f;
                text.transform.localPosition = new Vector3(-0.75f, -0.23f, 0f);

            }
        }
    }

    private static readonly StringBuilder VitalsStringBuilder = new();

    public static bool UpdatePrefix(VitalsMinigame __instance)
    {
        VitalsTimer += Time.deltaTime;
        if (VitalsTimer > 0.05f)
            UseVitalsTime();

        if (ModMapOptions.RestrictDevices > 0 && ModMapOptions.RestrictVitals)
        {
            if (TimeRemaining == null)
            {
                TimeRemaining = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
                TimeRemaining.alignment = TMPro.TextAlignmentOptions.BottomRight;
                TimeRemaining.transform.position = Vector3.zero;
                TimeRemaining.transform.localPosition = new Vector3(1.7f, 4.45f);
                TimeRemaining.transform.localScale *= 1.8f;
                TimeRemaining.color = Palette.White;
            }

            if (ModMapOptions.RestrictVitalsTime <= 0f)
            {
                __instance.Close();
                return false;
            }

            VitalsStringBuilder.Clear();
            var ts = TimeSpan.FromSeconds(ModMapOptions.RestrictVitalsTime);
            if (ts.TotalHours >= 1) VitalsStringBuilder.Append((int)ts.TotalHours).Append(':');
            VitalsStringBuilder.Append(ts.Minutes.ToString("D2")).Append(':')
                               .Append(ts.Seconds.ToString("D2")).Append('.')
                               .Append((ts.Milliseconds / 10).ToString("D2"));

            string timeString = VitalsStringBuilder.ToString();
            VitalsStringBuilder.Clear();
            VitalsStringBuilder.Append(string.Format(Tr.Get("Hud.TimeRemaining"), timeString));
            TimeRemaining.text = VitalsStringBuilder.ToString();
            TimeRemaining.gameObject.SetActive(true);
        }

        return true;
    }

    public static void UpdatePostfix(VitalsMinigame __instance)
    {
        // Hacker show time since death
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && Hacker.HackerTimer > 0)
        {
            for (int k = 0; k < __instance.vitals.Length; k++)
            {
                VitalsPanel vitalsPanel = __instance.vitals[k];
                NetworkedPlayerInfo player = GameData.Instance.AllPlayers[k];

                // Hacker update
                if (vitalsPanel.IsDead)
                {
                    var deadPlayer = GameHistory.GetDeadPlayer(player.PlayerId);
                    if (deadPlayer != null && k < HackerTexts.Count && HackerTexts[k] != null)
                    {
                        float timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.TimeOfDeath).TotalMilliseconds;
                        HackerTexts[k].gameObject.SetActive(true);
                        VitalsStringBuilder.Clear();
                        VitalsStringBuilder.Append(Math.Round(timeSinceDeath / 1000)).Append('s');
                        HackerTexts[k].text = VitalsStringBuilder.ToString();
                    }
                }
            }
        }
        else
        {
            foreach (TextMeshPro text in HackerTexts)
            {
                if (text != null && text.gameObject != null)
                {
                    text.gameObject.SetActive(false);
                }
            }
        }
    }
}