namespace RebuildUs.Modules.Consoles;

public static class Vitals
{
    static float vitalsTimer = 0f;
    static TextMeshPro TimeRemaining;
    private static List<TextMeshPro> hackerTexts = [];

    public static void ResetData()
    {
        vitalsTimer = 0f;
        if (TimeRemaining != null)
        {
            UnityEngine.Object.Destroy(TimeRemaining);
            TimeRemaining = null;
        }
    }

    static void UseVitalsTime()
    {
        // Don't waste network traffic if we're out of time.
        if (ModMapOptions.restrictDevices > 0 && ModMapOptions.restrictVitals && ModMapOptions.restrictVitalsTime > 0f && CachedPlayer.LocalPlayer.PlayerControl.IsAlive())
        {
            using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.UseVitalsTime);
            sender.Write(vitalsTimer);
            RPCProcedure.UseVitalsTime(vitalsTimer);
        }
        vitalsTimer = 0f;
    }

    public static void Begin(VitalsMinigame __instance)
    {
        vitalsTimer = 0f;

        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Hacker))
        {
            hackerTexts = [];
            foreach (VitalsPanel panel in __instance.vitals)
            {
                TextMeshPro text = UnityEngine.Object.Instantiate(__instance.SabText, panel.transform);
                hackerTexts.Add(text);
                UnityEngine.Object.DestroyImmediate(text.GetComponent<AlphaBlink>());
                text.gameObject.SetActive(false);
                text.transform.localScale = Vector3.one * 0.75f;
                text.transform.localPosition = new Vector3(-0.75f, -0.23f, 0f);

            }
        }
    }

    public static bool UpdatePrefix(VitalsMinigame __instance)
    {
        vitalsTimer += Time.deltaTime;
        if (vitalsTimer > 0.05f)
            UseVitalsTime();

        if (ModMapOptions.restrictDevices > 0 && ModMapOptions.restrictVitals)
        {
            if (TimeRemaining == null)
            {
                TimeRemaining = UnityEngine.Object.Instantiate(FastDestroyableSingleton<TaskPanelBehaviour>.Instance.taskText, __instance.transform);
                TimeRemaining.alignment = TMPro.TextAlignmentOptions.BottomRight;
                TimeRemaining.transform.position = Vector3.zero;
                TimeRemaining.transform.localPosition = new Vector3(1.7f, 4.45f);
                TimeRemaining.transform.localScale *= 1.8f;
                TimeRemaining.color = Palette.White;
            }

            if (ModMapOptions.restrictVitalsTime <= 0f)
            {
                __instance.Close();
                return false;
            }

            string timeString = TimeSpan.FromSeconds(ModMapOptions.restrictVitalsTime).ToString(@"mm\:ss\.ff");
            TimeRemaining.text = String.Format(Tr.Get("timeRemaining"), timeString);
            TimeRemaining.gameObject.SetActive(true);
        }

        return true;
    }

    public static void UpdatePostfix(VitalsMinigame __instance)
    {
        // Hacker show time since death
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Hacker) && Hacker.GetRole().hackerTimer > 0)
        {
            for (int k = 0; k < __instance.vitals.Length; k++)
            {
                VitalsPanel vitalsPanel = __instance.vitals[k];
                NetworkedPlayerInfo player = GameData.Instance.AllPlayers[k];

                // Hacker update
                if (vitalsPanel.IsDead)
                {
                    var deadPlayer = GameHistory.DeadPlayers?.Where(x => x.Player?.PlayerId == player?.PlayerId)?.FirstOrDefault();
                    if (deadPlayer != null && k < hackerTexts.Count && hackerTexts[k] != null)
                    {
                        float timeSinceDeath = (float)(DateTime.UtcNow - deadPlayer.TimeOfDeath).TotalMilliseconds;
                        hackerTexts[k].gameObject.SetActive(true);
                        hackerTexts[k].text = Math.Round(timeSinceDeath / 1000) + "s";
                    }
                }
            }
        }
        else
        {
            foreach (TextMeshPro text in hackerTexts)
            {
                if (text != null && text.gameObject != null)
                {
                    text.gameObject.SetActive(false);
                }
            }
        }
    }
}