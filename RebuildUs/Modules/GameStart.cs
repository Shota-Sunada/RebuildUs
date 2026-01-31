using System.Reflection;

namespace RebuildUs.Modules;

public static class GameStart
{
    public struct PlayerVersion
    {
        public int Major;
        public int Minor;
        public int Build;
        public int Revision;
        public Guid Guid;

        public PlayerVersion(int major, int minor, int build, int revision, Guid guid)
        {
            Major = major;
            Minor = minor;
            Build = build;
            Revision = revision;
            Guid = guid;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Major);
            sb.Append('.');
            sb.Append(Minor);
            sb.Append('.');
            sb.Append(Build);
            if (Revision >= 0)
            {
                sb.Append('.');
                sb.Append(Revision);
            }
            return sb.ToString();
        }

        public bool Matches(Version version)
        {
            return Major == version.Major && Minor == version.Minor && Build == version.Build;
        }

        public bool GuidMatches(Guid guid)
        {
            return Guid == guid;
        }
    }

    public static Dictionary<int, PlayerVersion> PlayerVersions = [];
    public static float KickingTimer = 0f;
    public static bool VersionSent = false;
    public static string LobbyCodeText = "";

    public static float StartingTimer = 0;
    public static bool SendGamemode = true;

    private static TextMeshPro WarningText;
    private static TextMeshPro TimerText;
    private static PassiveButton CancelButton;
    private static float Timer = 600f;

    private static readonly StringBuilder InfoStringBuilder = new();
    private static string _lastCountDown = "";
    private static string _lastWarningMessage = "";

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
        // Reset dictionaries and flags
        PlayerVersions.Clear();
        VersionSent = false;
        // Reset kicking timer
        KickingTimer = 0f;

        WarningText = UnityEngine.Object.Instantiate(__instance.GameStartText, __instance.transform);
        WarningText.name = "WarningText";
        WarningText.transform.localPosition = new(0f, 0f - __instance.transform.localPosition.y, -1f);
        WarningText.gameObject.SetActive(false);

        TimerText = UnityEngine.Object.Instantiate(__instance.PlayerCounter, __instance.PlayerCounter.transform.parent);
        TimerText.autoSizeTextContainer = true;
        TimerText.fontSize = 3.2f;
        TimerText.name = "Timer";
        TimerText.DestroyChildren();
        TimerText.transform.localPosition += new Vector3(0.3f, -3.4f, 0f);
        TimerText.gameObject.SetActive(AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame && AmongUsClient.Instance.AmHost);

        CancelButton = UnityEngine.Object.Instantiate(__instance.StartButton, __instance.transform);
        CancelButton.name = "CancelButton";
        var cancelLabel = CancelButton.buttonText;
        cancelLabel.GetComponent<TextTranslatorTMP>()?.Destroy();
        cancelLabel.text = "Cancel";
        CancelButton.transform.localScale = new(0.5f, 0.5f, 1f);
        var cancelButtonInactiveRenderer = CancelButton.inactiveSprites.GetComponent<SpriteRenderer>();
        cancelButtonInactiveRenderer.color = new(0.8f, 0f, 0f, 1f);
        var cancelButtonActiveRenderer = CancelButton.activeSprites.GetComponent<SpriteRenderer>();
        cancelButtonActiveRenderer.color = Color.red;
        var cancelButtonInactiveShine = CancelButton.inactiveSprites.transform.Find("Shine");
        if (cancelButtonInactiveShine)
        {
            cancelButtonInactiveShine.gameObject.SetActive(false);
        }
        CancelButton.activeTextColor = CancelButton.inactiveTextColor = Color.white;
        CancelButton.transform.localPosition = new(2f, 0.13f, 0f);
        CancelButton.OnClick = new();
        CancelButton.OnClick.AddListener((Action)(() =>
        {
            __instance.ResetStartState();
            SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.StopStart);
            }
        }));
        CancelButton.gameObject.SetActive(false);
    }

    public static void OnPlayerLeft(int clientId)
    {
        PlayerVersions.Remove(clientId);
        if (PlayerVersions.Count == 0) VersionSent = false;
    }

    public static void UpdatePostfix(GameStartManager __instance)
    {
        if (PlayerControl.LocalPlayer != null && !VersionSent && AmongUsClient.Instance.ClientId >= 0)
        {
            VersionSent = true;
            Helpers.ShareGameVersion();
        }

        // // Check version handshake infos
        bool versionMismatch = false;
        InfoStringBuilder.Clear();

        var clients = AmongUsClient.Instance.allClients.ToArray();
        foreach (var client in clients)
        {
            if (client == null || client.Character == null || client.Character.Data == null)
            {
                continue;
            }

            // Skip Dummies
            var dummyComponent = client.Character.GetComponent<DummyBehaviour>();
            if (dummyComponent != null && dummyComponent.enabled)
            {
                continue;
            }

            if (!PlayerVersions.TryGetValue(client.Id, out var pV))
            {
                versionMismatch = true;
                InfoStringBuilder.Append("<color=#FF0000FF>");
                InfoStringBuilder.Append(client.Character.Data.PlayerName);
                InfoStringBuilder.Append(" has no version of RebuildUs\n</color>");
            }
            else
            {
                if (!pV.Matches(RebuildUs.Instance.Version))
                {
                    InfoStringBuilder.Append("<color=#FF0000FF>");
                    InfoStringBuilder.Append(client.Character.Data.PlayerName);
                    InfoStringBuilder.Append(" has a different version of RebuildUs (v");
                    InfoStringBuilder.Append(pV);
                    InfoStringBuilder.Append(")\n</color>");
                    versionMismatch = true;
                }
                else if (!pV.GuidMatches(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId))
                {
                    InfoStringBuilder.Append("<color=#FF0000FF>");
                    InfoStringBuilder.Append(client.Character.Data.PlayerName);
                    InfoStringBuilder.Append(" has a different build of RebuildUs (v");
                    InfoStringBuilder.Append(pV);
                    InfoStringBuilder.Append(")\n</color>");
                    versionMismatch = true;
                }
            }
        }

        string message = InfoStringBuilder.ToString();

        // Display message to the host
        if (AmongUsClient.Instance.AmHost)
        {
            if (versionMismatch)
            {
                if (_lastWarningMessage != message)
                {
                    WarningText.text = message;
                    _lastWarningMessage = message;
                }
                WarningText.gameObject.SetActive(true);
            }
            else
            {
                WarningText.gameObject.SetActive(false);
                _lastWarningMessage = "";
            }
        }
        // Client update with handshake infos
        else
        {
            if (!PlayerVersions.TryGetValue(AmongUsClient.Instance.HostId, out var hostVersion) || !hostVersion.Matches(RebuildUs.Instance.Version))
            {
                KickingTimer += Time.deltaTime;
                if (KickingTimer > 10)
                {
                    KickingTimer = 0;
                    AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                    SceneChanger.ChangeScene("MainMenu");
                }

                InfoStringBuilder.Clear();
                InfoStringBuilder.Append("<color=#FF0000FF>The host has no or a different version of RebuildUs\nYou will be kicked in ");
                InfoStringBuilder.Append(Math.Max(0, (int)Math.Round(10 - KickingTimer)));
                InfoStringBuilder.Append("s</color>");
                string warning = InfoStringBuilder.ToString();
                if (_lastWarningMessage != warning)
                {
                    WarningText.text = warning;
                    _lastWarningMessage = warning;
                }
                WarningText.gameObject.SetActive(true);
            }
            else if (versionMismatch)
            {
                InfoStringBuilder.Clear();
                InfoStringBuilder.Append("<color=#FF0000FF>Players With Different Versions:\n</color>");
                InfoStringBuilder.Append(message);
                string warning = InfoStringBuilder.ToString();
                if (_lastWarningMessage != warning)
                {
                    WarningText.text = warning;
                    _lastWarningMessage = warning;
                }
                WarningText.gameObject.SetActive(true);
            }
            else
            {
                WarningText.gameObject.SetActive(false);
                _lastWarningMessage = "";
            }
        }
        // Start Timer
        if (StartingTimer > 0)
        {
            StartingTimer -= Time.deltaTime;
        }

        if (AmongUsClient.Instance.AmHost && CancelButton != null)
        {
            CancelButton.gameObject.SetActive(__instance.startState == GameStartManager.StartingStates.Countdown);
        }

        // Lobby timer
        if (
            !AmongUsClient.Instance.AmHost ||
            !GameData.Instance ||
            AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
        {
            return;
        }

        Timer = Mathf.Max(0f, Timer - Time.deltaTime);
        int totalSeconds = (int)Timer;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        InfoStringBuilder.Clear();
        if (minutes < 10) InfoStringBuilder.Append('0');
        InfoStringBuilder.Append(minutes);
        InfoStringBuilder.Append(':');
        if (seconds < 10) InfoStringBuilder.Append('0');
        InfoStringBuilder.Append(seconds);

        string countDown = InfoStringBuilder.ToString();
        if (Timer <= 60) countDown = Helpers.Cs(Color.red, countDown);

        if (_lastCountDown != countDown)
        {
            TimerText.text = countDown;
            _lastCountDown = countDown;
        }

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

                if (!PlayerVersions.TryGetValue(client.Id, out var pV))
                {
                    continueStart = false;
                    break;
                }

                if (!pV.Matches(RebuildUs.Instance.Version))
                {
                    continueStart = false;
                    break;
                }
            }

            if (!continueStart)
            {
                __instance.ResetStartState();
            }
            else
            {
                CustomOption.CoSpawnSyncSettings();
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
                bool hasEnsured = false;
                for (int i = 0; i < probabilities.Length; i++)
                {
                    if (probabilities[i] >= 1.0f)
                    {
                        hasEnsured = true;
                        break;
                    }
                }

                if (hasEnsured)
                {
                    for (int i = 0; i < probabilities.Length; i++)
                    {
                        if (probabilities[i] < 1.0f) probabilities[i] = 0;
                    }
                }

                float sum = 0;
                for (int i = 0; i < probabilities.Length; i++) sum += probabilities[i];

                // All maps set to 0, why are you doing this???
                if (sum <= 0) return continueStart;
                for (int i = 0; i < probabilities.Length; i++)
                {
                    // Normalize to [0,1]
                    probabilities[i] /= sum;
                }
                float selection = (float)RebuildUs.Instance.Rnd.NextDouble();
                float cumSum = 0;
                for (byte i = 0; i < (byte)probabilities.Length; i++)
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