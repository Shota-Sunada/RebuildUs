using System.Reflection;

namespace RebuildUs.Modules;

public static class GameStart
{
    public static Dictionary<int, PlayerVersion> PlayerVersions = [];
    public static float Timer = 600f;
    public static float KickingTimer = 0f;
    public static bool VersionSent = false;
    public static string LobbyCodeText = "";

    public static float StartingTimer = 0;
    public static bool Update = false;
    public static string CurrentText = "";
    public static GameObject CopiedStartButton;
    public static bool SendGamemode = true;

    public static void OnPlayerJoined()
    {
        if (PlayerControl.LocalPlayer != null)
        {
            Helpers.ShareGameVersion();
        }
        SendGamemode = true;
    }

    public static void Start(GameStartManager __instance)
    {
        // Trigger version refresh
        VersionSent = false;
        // Reset lobby countdown timer
        Timer = 600f;
        // Reset kicking timer
        KickingTimer = 0f;
        // Copy lobby code
        var code = InnerNet.GameCode.IntToGameName(AmongUsClient.Instance.GameId);
        GUIUtility.systemCopyBuffer = code;
        LobbyCodeText = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.RoomCode, new Il2CppReferenceArray<Il2CppSystem.Object>(0)) + "\r\n" + code;
    }

    public static void UpdatePostfix(GameStartManager __instance)
    {
        if (PlayerControl.LocalPlayer != null && !VersionSent)
        {
            VersionSent = true;
            Helpers.ShareGameVersion();
        }

        // Check version handshake infos
        bool versionMismatch = false;
        string message = "";
        foreach (InnerNet.ClientData client in AmongUsClient.Instance.allClients.ToArray())
        {
            if (client.Character == null) continue;
            else if (!PlayerVersions.ContainsKey(client.Id))
            {
                versionMismatch = true;
                message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a different or no version of The Other Roles\n</color>";
            }
            else
            {
                PlayerVersion pV = PlayerVersions[client.Id];
                int diff = RebuildUs.Instance.Version.CompareTo(pV.Version);
                if (diff > 0)
                {
                    message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has an older version of The Other Roles (v{PlayerVersions[client.Id].Version})\n</color>";
                    versionMismatch = true;
                }
                else if (diff < 0)
                {
                    message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a newer version of The Other Roles (v{PlayerVersions[client.Id].Version})\n</color>";
                    versionMismatch = true;
                }
                else if (!pV.GuidMatches())
                { // version presumably matches, check if Guid matches
                    message += $"<color=#FF0000FF>{client.Character.Data.PlayerName} has a modified version of TOR v{PlayerVersions[client.Id].Version} <size=30%>({pV.Guid})</size>\n</color>";
                    versionMismatch = true;
                }
            }
        }

        // Display message to the host
        if (AmongUsClient.Instance.AmHost)
        {
            if (versionMismatch)
            {
                __instance.GameStartText.text = message;
                __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                __instance.GameStartTextParent.SetActive(true);
            }
            else
            {
                __instance.GameStartText.transform.localPosition = Vector3.zero;
                __instance.GameStartText.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                if (!__instance.GameStartText.text.StartsWith("Starting"))
                {
                    __instance.GameStartText.text = String.Empty;
                    __instance.GameStartTextParent.SetActive(false);
                }
            }

            if (__instance.startState != GameStartManager.StartingStates.Countdown)
            {
                CopiedStartButton?.Destroy();
            }
            // Make starting info available to clients:
            if (StartingTimer <= 0 && __instance.startState == GameStartManager.StartingStates.Countdown)
            {
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGameStarting, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.SetGameStarting();
                // Activate Stop-Button
                CopiedStartButton = GameObject.Instantiate(__instance.StartButton.gameObject, __instance.StartButton.gameObject.transform.parent);
                CopiedStartButton.transform.localPosition = __instance.StartButton.transform.localPosition;
                CopiedStartButton.SetActive(true);
                var startButtonText = CopiedStartButton.GetComponentInChildren<TMPro.TextMeshPro>();
                startButtonText.text = "";
                startButtonText.fontSize *= 0.8f;
                startButtonText.fontSizeMax = startButtonText.fontSize;
                startButtonText.gameObject.transform.localPosition = Vector3.zero;
                PassiveButton startButtonPassiveButton = CopiedStartButton.GetComponent<PassiveButton>();
                void StopStartFunc()
                {
                    __instance.ResetStartState();
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.StopStart, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    CopiedStartButton.Destroy();
                    StartingTimer = 0;
                    SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
                }
                startButtonPassiveButton.OnClick.AddListener((Action)(() => StopStartFunc()));
                __instance.StartCoroutine(Effects.Lerp(.1f, new Action<float>((p) =>
                {
                    startButtonText.text = "";
                })));
            }
        }

        // Client update with handshake infos
        else
        {
            if (!PlayerVersions.ContainsKey(AmongUsClient.Instance.HostId) || RebuildUs.Instance.Version.CompareTo(PlayerVersions[AmongUsClient.Instance.HostId].Version) != 0)
            {
                KickingTimer += Time.deltaTime;
                if (KickingTimer > 10)
                {
                    KickingTimer = 0;
                    AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                    SceneChanger.ChangeScene("MainMenu");
                }

                __instance.GameStartText.text = $"<color=#FF0000FF>The host has no or a different version of The Other Roles\nYou will be kicked in {Math.Round(10 - KickingTimer)}s</color>";
                __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                __instance.GameStartTextParent.SetActive(true);
            }
            else if (versionMismatch)
            {
                __instance.GameStartText.text = $"<color=#FF0000FF>Players With Different Versions:\n</color>" + message;
                __instance.GameStartText.transform.localPosition = __instance.StartButton.transform.localPosition + Vector3.up * 5;
                __instance.GameStartText.transform.localScale = new Vector3(2f, 2f, 1f);
                __instance.GameStartTextParent.SetActive(true);
            }
            else
            {
                __instance.GameStartText.transform.localPosition = Vector3.zero;
                __instance.GameStartText.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                if (!__instance.GameStartText.text.StartsWith("Starting"))
                {
                    __instance.GameStartText.text = string.Empty;
                    __instance.GameStartTextParent.SetActive(false);
                }
            }

            if (!__instance.GameStartText.text.StartsWith("Starting"))
            {
                CopiedStartButton?.Destroy();
            }
        }
        // Start Timer
        if (StartingTimer > 0)
        {
            StartingTimer -= Time.deltaTime;
        }
        // Lobby timer
        if (!GameData.Instance || !__instance.PlayerCounter) return; // No instance

        if (Update) CurrentText = __instance.PlayerCounter.text;

        Timer = Mathf.Max(0f, Timer -= Time.deltaTime);
        int minutes = (int)Timer / 60;
        int seconds = (int)Timer % 60;
        string suffix = $" ({minutes:00}:{seconds:00})";

        if (!AmongUsClient.Instance) return;

        if (AmongUsClient.Instance.AmHost && SendGamemode && PlayerControl.LocalPlayer != null)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.ShareGamemode, SendOption.Reliable, -1);
            writer.Write((byte)MapOptions.GameMode);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.ShareGamemode((byte)MapOptions.GameMode);
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
                    continue;

                if (!PlayerVersions.ContainsKey(client.Id))
                {
                    continueStart = false;
                    break;
                }

                PlayerVersion pV = PlayerVersions[client.Id];
                int diff = RebuildUs.Instance.Version.CompareTo(pV.Version);
                if (diff != 0 || !pV.GuidMatches())
                {
                    continueStart = false;
                    break;
                }
            }
            if (continueStart && (MapOptions.GameMode == CustomGamemodes.HideNSeek || MapOptions.GameMode == CustomGamemodes.PropHunt) && GameOptionsManager.Instance.CurrentGameOptions.MapId != 6)
            {
                byte mapId = 0;
                // if (MapOptions.GameMode == CustomGamemodes.HideNSeek) mapId = (byte)CustomOptionHolder.hideNSeekMap.getSelection();
                // else if (MapOptions.GameMode == CustomGamemodes.PropHunt) mapId = (byte)CustomOptionHolder.propHuntMap.getSelection();
                if (mapId >= 3) mapId++;
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DynamicMapOption, Hazel.SendOption.Reliable, -1);
                writer.Write(mapId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.DynamicMapOption(mapId);
            }
            else if (CustomOptionHolder.RandomMap.GetBool() && continueStart)
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
                if (sum == 0) return continueStart;  // All maps set to 0, why are you doing this???
                for (int i = 0; i < probabilities.Length; i++)
                {  // Normalize to [0,1]
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

                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.DynamicMapOption, Hazel.SendOption.Reliable, -1);
                writer.Write(chosenMapId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.DynamicMapOption(chosenMapId);
            }
        }
        return continueStart;
    }
}

public class PlayerVersion(Version version, Guid guid)
{
    public readonly Version Version = version;
    public readonly Guid Guid = guid;

    public bool GuidMatches()
    {
        return Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.Equals(Guid);
    }
}