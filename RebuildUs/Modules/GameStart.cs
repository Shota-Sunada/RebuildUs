using System.Reflection;

namespace RebuildUs.Modules;

public static class GameStart
{
    public static Dictionary<int, Version> PlayerVersions = [];
    public static float KickingTimer = 0f;
    public static bool VersionSent = false;
    public static string LobbyCodeText = "";

    public static float StartingTimer = 0;
    public static bool SendGamemode = true;

    private static TextMeshPro warningText;
    private static TextMeshPro timerText;
    private static PassiveButton cancelButton;
    private static float timer = 600f;

    public static void OnPlayerJoined()
    {
        if (PlayerControl.LocalPlayer != null)
        {
            // Helpers.ShareGameVersion();
        }
        SendGamemode = true;
    }

    public static void Start(GameStartManager __instance)
    {
        // Trigger version refresh
        VersionSent = false;
        // Reset kicking timer
        KickingTimer = 0f;

        warningText = UnityEngine.Object.Instantiate(__instance.GameStartText, __instance.transform);
        warningText.name = "WarningText";
        warningText.transform.localPosition = new(0f, 0f - __instance.transform.localPosition.y, -1f);
        warningText.gameObject.SetActive(false);

        timerText = UnityEngine.Object.Instantiate(__instance.PlayerCounter, __instance.PlayerCounter.transform.parent);
        timerText.autoSizeTextContainer = true;
        timerText.fontSize = 3.2f;
        timerText.name = "Timer";
        timerText.DestroyChildren();
        timerText.transform.localPosition += new Vector3(0.3f, -3.4f, 0f);
        timerText.gameObject.SetActive(AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame && AmongUsClient.Instance.AmHost);

        cancelButton = UnityEngine.Object.Instantiate(__instance.StartButton, __instance.transform);
        cancelButton.name = "CancelButton";
        var cancelLabel = cancelButton.buttonText;
        cancelLabel.GetComponent<TextTranslatorTMP>()?.Destroy();
        cancelLabel.text = "Cancel";
        cancelButton.transform.localScale = new(0.5f, 0.5f, 1f);
        var cancelButtonInactiveRenderer = cancelButton.inactiveSprites.GetComponent<SpriteRenderer>();
        cancelButtonInactiveRenderer.color = new(0.8f, 0f, 0f, 1f);
        var cancelButtonActiveRenderer = cancelButton.activeSprites.GetComponent<SpriteRenderer>();
        cancelButtonActiveRenderer.color = Color.red;
        var cancelButtonInactiveShine = cancelButton.inactiveSprites.transform.Find("Shine");
        if (cancelButtonInactiveShine)
        {
            cancelButtonInactiveShine.gameObject.SetActive(false);
        }
        cancelButton.activeTextColor = cancelButton.inactiveTextColor = Color.white;
        cancelButton.transform.localPosition = new(2f, 0.13f, 0f);
        cancelButton.OnClick = new();
        cancelButton.OnClick.AddListener((Action)(() =>
        {
            __instance.ResetStartState();
            SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.StopStart);
            }
        }));
        cancelButton.gameObject.SetActive(false);
    }

    public static void UpdatePostfix(GameStartManager __instance)
    {
        if (PlayerControl.LocalPlayer != null && !VersionSent)
        {
            VersionSent = true;
            Helpers.ShareGameVersion((byte)AmongUsClient.Instance.HostId);
        }

        // // Check version handshake infos
        bool versionMismatch = false;
        string message = "";
        foreach (var client in AmongUsClient.Instance.allClients.ToArray())
        {
            if (client.Character == null)
            {
                continue;
            }
            else if (!PlayerVersions.ContainsKey(client.Id))
            {
                versionMismatch = true;
                message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a different or no version of RebuildUs\n</color>";
            }
            else
            {
                Version pV = PlayerVersions[client.Id];
                int diff = RebuildUs.Instance.Version.CompareTo(pV);
                if (diff > 0)
                {
                    message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has an older version of RebuildUs (v{PlayerVersions[client.Id]})\n</color>";
                    versionMismatch = true;
                }
                else if (diff < 0)
                {
                    message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a newer version of RebuildUs (v{PlayerVersions[client.Id]})\n</color>";
                    versionMismatch = true;
                }
            }
        }

        // Display message to the host
        if (AmongUsClient.Instance.AmHost)
        {
            if (versionMismatch)
            {
                warningText.text = message;
                warningText.gameObject.SetActive(true);
            }
            else
            {
                warningText.gameObject.SetActive(false);
            }
        }
        // Client update with handshake infos
        else
        {
            if (!PlayerVersions.ContainsKey(AmongUsClient.Instance.HostId) || RebuildUs.Instance.Version.CompareTo(PlayerVersions[AmongUsClient.Instance.HostId]) != 0)
            {
                KickingTimer += Time.deltaTime;
                if (KickingTimer > 10)
                {
                    KickingTimer = 0;
                    AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                    SceneChanger.ChangeScene("MainMenu");
                }

                warningText.text = $"<color=#FF0000FF>The host has no or a different version of RebuildUs\nYou will be kicked in {Math.Round(10 - KickingTimer)}s</color>";
                warningText.gameObject.SetActive(true);
            }
            else if (versionMismatch)
            {
                warningText.text = $"<color=#FF0000FF>Players With Different Versions:\n</color>" + message;
                warningText.gameObject.SetActive(true);
            }
            else
            {
                warningText.gameObject.SetActive(false);
            }
        }
        // Start Timer
        if (StartingTimer > 0)
        {
            StartingTimer -= Time.deltaTime;
        }

        if (AmongUsClient.Instance.AmHost && cancelButton != null)
        {
            cancelButton.gameObject.SetActive(__instance.startState == GameStartManager.StartingStates.Countdown);
        }

        // Lobby timer
        if (
            !AmongUsClient.Instance.AmHost ||
            !GameData.Instance ||
            AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
        {
            return;
        }

        timer = Mathf.Max(0f, timer -= Time.deltaTime);
        int minutes = (int)timer / 60;
        int seconds = (int)timer % 60;
        string countDown = $"{minutes:00}:{seconds:00}";
        if (timer <= 60) countDown = Helpers.Cs(Color.red, countDown);
        timerText.text = countDown;

        if (!GameData.Instance || !__instance.PlayerCounter) return; // No instance

        if (!AmongUsClient.Instance) return;

        if (AmongUsClient.Instance.AmHost && SendGamemode && PlayerControl.LocalPlayer != null)
        {
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareGamemode);
                sender.Write((byte)ModMapOptions.GameMode);
                RPCProcedure.ShareGamemode((byte)ModMapOptions.GameMode);
            }
            SendGamemode = false;
        }
    }

    public static bool BeginGame(GameStartManager __instance)
    {
        // Block game start if not everyone has the same mod version
        var continueStart = true;

        if (AmongUsClient.Instance.AmHost)
        {
            foreach (InnerNet.ClientData client in AmongUsClient.Instance.allClients.GetFastEnumerator())
            {
                if (client.Character == null) continue;
                var dummyComponent = client.Character.GetComponent<DummyBehaviour>();
                if (dummyComponent != null && dummyComponent.enabled)
                {
                    continue;
                }

                if (!PlayerVersions.ContainsKey(client.Id))
                {
                    continueStart = false;
                    break;
                }

                Version pV = PlayerVersions[client.Id];
                int diff = RebuildUs.Instance.Version.CompareTo(pV);
                if (diff != 0)
                {
                    continueStart = false;
                    break;
                }
            }

            if (!continueStart)
            {
                __instance.ResetStartState();
            }

            if (CustomOptionHolder.RandomMap.GetBool() && continueStart)
            {
                // 0 = Skeld
                // 1 = Mira HQ
                // 2 = Polus
                // 3 = Dleks - deactivated
                // 4 = Airship
                // 5 = Submerged
                byte chosenMapId = 0;
                float[] probabilities =
                [
                    CustomOptionHolder.RandomMapEnableSkeld.GetSelection() / 10f,
                    CustomOptionHolder.RandomMapEnableMiraHQ.GetSelection() / 10f,
                    CustomOptionHolder.RandomMapEnablePolus.GetSelection() / 10f,
                    CustomOptionHolder.RandomMapEnableAirShip.GetSelection() / 10f,
                    CustomOptionHolder.RandomMapEnableFungle.GetSelection() / 10f,
                    CustomOptionHolder.RandomMapEnableSubmerged.GetSelection() / 10f,
                ];

                // if any map is at 100%, remove all maps that are not!
                if (probabilities.Contains(1.0f))
                {
                    for (int i = 0; i < probabilities.Length; i++)
                    {
                        if (probabilities[i] != 1.0) probabilities[i] = 0;
                    }
                }

                float sum = probabilities.Sum();
                // All maps set to 0, why are you doing this???
                if (sum == 0) return continueStart;
                for (int i = 0; i < probabilities.Length; i++)
                {
                    // Normalize to [0,1]
                    probabilities[i] /= sum;
                }
                float selection = (float)RebuildUs.Instance.Rnd.NextDouble();
                float cumSum = 0;
                for (byte i = 0; i < probabilities.Length; i++)
                {
                    cumSum += probabilities[i];
                    if (cumSum > selection)
                    {
                        chosenMapId = i;
                        break;
                    }
                }

                {
                    using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.DynamicMapOption);
                    sender.Write(chosenMapId);
                    RPCProcedure.DynamicMapOption(chosenMapId);
                }
            }
        }
        return continueStart;
    }
}

