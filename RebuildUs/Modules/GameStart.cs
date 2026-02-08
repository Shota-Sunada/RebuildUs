using System.Reflection;
using Object = UnityEngine.Object;

namespace RebuildUs.Modules;

public static class GameStart
{
    public static Dictionary<int, PlayerVersion> PlayerVersions = [];
    public static float KickingTimer;
    public static bool VersionSent;
    public static string LobbyCodeText = "";

    public static float StartingTimer;
    public static bool SendGamemode = true;

    private static TextMeshPro _warningText;
    private static TextMeshPro _timerText;
    private static PassiveButton _cancelButton;
    private static float _timer = 600f;

    private static readonly StringBuilder INFO_STRING_BUILDER = new();
    private static string _lastCountDown = "";
    private static string _lastWarningMessage = "";

    public static void OnPlayerJoined()
    {
        if (PlayerControl.LocalPlayer != null) Helpers.ShareGameVersion();
        SendGamemode = true;
    }

    public static void Start(GameStartManager __instance)
    {
        // Reset dictionaries and flags
        PlayerVersions.Clear();
        VersionSent = false;
        // Reset kicking timer
        KickingTimer = 0f;

        _warningText = Object.Instantiate(__instance.GameStartText, __instance.transform);
        _warningText.name = "WarningText";
        _warningText.transform.localPosition = new(0f, 0f - __instance.transform.localPosition.y, -1f);
        _warningText.gameObject.SetActive(false);

        _timerText = Object.Instantiate(__instance.PlayerCounter, __instance.PlayerCounter.transform.parent);
        _timerText.autoSizeTextContainer = true;
        _timerText.fontSize = 3.2f;
        _timerText.name = "Timer";
        _timerText.DestroyChildren();
        _timerText.transform.localPosition += new Vector3(0.3f, -3.4f, 0f);
        _timerText.gameObject.SetActive(AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame && AmongUsClient.Instance.AmHost);

        _cancelButton = Object.Instantiate(__instance.StartButton, __instance.transform);
        _cancelButton.name = "CancelButton";
        var cancelLabel = _cancelButton.buttonText;
        cancelLabel.GetComponent<TextTranslatorTMP>()?.Destroy();
        cancelLabel.text = "Cancel";
        _cancelButton.transform.localScale = new(0.5f, 0.5f, 1f);
        var cancelButtonInactiveRenderer = _cancelButton.inactiveSprites.GetComponent<SpriteRenderer>();
        cancelButtonInactiveRenderer.color = new(0.8f, 0f, 0f, 1f);
        var cancelButtonActiveRenderer = _cancelButton.activeSprites.GetComponent<SpriteRenderer>();
        cancelButtonActiveRenderer.color = Color.red;
        var cancelButtonInactiveShine = _cancelButton.inactiveSprites.transform.Find("Shine");
        if (cancelButtonInactiveShine) cancelButtonInactiveShine.gameObject.SetActive(false);
        _cancelButton.activeTextColor = _cancelButton.inactiveTextColor = Color.white;
        _cancelButton.transform.localPosition = new(2f, 0.13f, 0f);
        _cancelButton.OnClick = new();
        _cancelButton.OnClick.AddListener((Action)(() =>
        {
            __instance.ResetStartState();
            SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.StopStart);
            }
        }));
        _cancelButton.gameObject.SetActive(false);
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
        var versionMismatch = false;
        INFO_STRING_BUILDER.Clear();

        var clients = AmongUsClient.Instance.allClients.ToArray();
        foreach (var client in clients)
        {
            if (client == null || client.Character == null || client.Character.Data == null) continue;

            // Skip Dummies
            var dummyComponent = client.Character.GetComponent<DummyBehaviour>();
            if (dummyComponent != null && dummyComponent.enabled) continue;

            if (!PlayerVersions.TryGetValue(client.Id, out var pV))
            {
                versionMismatch = true;
                INFO_STRING_BUILDER.Append("<color=#FF0000FF>");
                INFO_STRING_BUILDER.Append(client.Character.Data.PlayerName);
                INFO_STRING_BUILDER.Append(" has no version of RebuildUs\n</color>");
            }
            else
            {
                if (!pV.Matches(RebuildUs.Instance.Version))
                {
                    INFO_STRING_BUILDER.Append("<color=#FF0000FF>");
                    INFO_STRING_BUILDER.Append(client.Character.Data.PlayerName);
                    INFO_STRING_BUILDER.Append(" has a different version of RebuildUs (v");
                    INFO_STRING_BUILDER.Append(pV);
                    INFO_STRING_BUILDER.Append(")\n</color>");
                    versionMismatch = true;
                }
                else if (!pV.GuidMatches(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId))
                {
                    INFO_STRING_BUILDER.Append("<color=#FF0000FF>");
                    INFO_STRING_BUILDER.Append(client.Character.Data.PlayerName);
                    INFO_STRING_BUILDER.Append(" has a different build of RebuildUs (v");
                    INFO_STRING_BUILDER.Append(pV);
                    INFO_STRING_BUILDER.Append(")\n</color>");
                    versionMismatch = true;
                }
            }
        }

        var message = INFO_STRING_BUILDER.ToString();

        // Display message to the host
        if (AmongUsClient.Instance.AmHost)
        {
            if (versionMismatch)
            {
                if (_lastWarningMessage != message)
                {
                    _warningText.text = message;
                    _lastWarningMessage = message;
                }

                _warningText.gameObject.SetActive(true);
            }
            else
            {
                _warningText.gameObject.SetActive(false);
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

                INFO_STRING_BUILDER.Clear();
                INFO_STRING_BUILDER.Append("<color=#FF0000FF>The host has no or a different version of RebuildUs\nYou will be kicked in ");
                INFO_STRING_BUILDER.Append(Math.Max(0, (int)Math.Round(10 - KickingTimer)));
                INFO_STRING_BUILDER.Append("s</color>");
                var warning = INFO_STRING_BUILDER.ToString();
                if (_lastWarningMessage != warning)
                {
                    _warningText.text = warning;
                    _lastWarningMessage = warning;
                }

                _warningText.gameObject.SetActive(true);
            }
            else if (versionMismatch)
            {
                INFO_STRING_BUILDER.Clear();
                INFO_STRING_BUILDER.Append("<color=#FF0000FF>Players With Different Versions:\n</color>");
                INFO_STRING_BUILDER.Append(message);
                var warning = INFO_STRING_BUILDER.ToString();
                if (_lastWarningMessage != warning)
                {
                    _warningText.text = warning;
                    _lastWarningMessage = warning;
                }

                _warningText.gameObject.SetActive(true);
            }
            else
            {
                _warningText.gameObject.SetActive(false);
                _lastWarningMessage = "";
            }
        }

        // Start Timer
        if (StartingTimer > 0) StartingTimer -= Time.deltaTime;

        if (AmongUsClient.Instance.AmHost && _cancelButton != null) _cancelButton.gameObject.SetActive(__instance.startState == GameStartManager.StartingStates.Countdown);

        // Lobby timer
        if (!AmongUsClient.Instance.AmHost || !GameData.Instance || AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
            return;

        _timer = Mathf.Max(0f, _timer - Time.deltaTime);
        var totalSeconds = (int)_timer;
        var minutes = totalSeconds / 60;
        var seconds = totalSeconds % 60;

        INFO_STRING_BUILDER.Clear();
        if (minutes < 10) INFO_STRING_BUILDER.Append('0');
        INFO_STRING_BUILDER.Append(minutes);
        INFO_STRING_BUILDER.Append(':');
        if (seconds < 10) INFO_STRING_BUILDER.Append('0');
        INFO_STRING_BUILDER.Append(seconds);

        var countDown = INFO_STRING_BUILDER.ToString();
        if (_timer <= 60) countDown = Helpers.Cs(Color.red, countDown);

        if (_lastCountDown != countDown)
        {
            _timerText.text = countDown;
            _lastCountDown = countDown;
        }

        if (!GameData.Instance || !__instance.PlayerCounter) return; // No instance

        if (!AmongUsClient.Instance) return;

        if (AmongUsClient.Instance.AmHost && SendGamemode && PlayerControl.LocalPlayer != null)
        {
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareGamemode);
                sender.Write((byte)MapSettings.GameMode);
                RPCProcedure.ShareGamemode((byte)MapSettings.GameMode);
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
            foreach (var client in AmongUsClient.Instance.allClients.GetFastEnumerator())
            {
                if (client.Character == null) continue;
                var dummyComponent = client.Character.GetComponent<DummyBehaviour>();
                if (dummyComponent != null && dummyComponent.enabled) continue;

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
                __instance.ResetStartState();
            else
                CustomOption.CoSpawnSyncSettings();

            if (CustomOptionHolder.RandomMap.GetBool() && continueStart)
            {
                // 0 = Skeld
                // 1 = Mira HQ
                // 2 = Polus
                // 3 = Dleks - deactivated
                // 4 = Airship
                // 5 = Submerged
                byte chosenMapId = 0;
                float[] probabilities = [CustomOptionHolder.RandomMapEnableSkeld.GetSelection() / 10f, CustomOptionHolder.RandomMapEnableMiraHq.GetSelection() / 10f, CustomOptionHolder.RandomMapEnablePolus.GetSelection() / 10f, CustomOptionHolder.RandomMapEnableAirShip.GetSelection() / 10f, CustomOptionHolder.RandomMapEnableFungle.GetSelection() / 10f, CustomOptionHolder.RandomMapEnableSubmerged.GetSelection() / 10f];

                // if any map is at 100%, remove all maps that are not!
                var hasEnsured = false;
                for (var i = 0; i < probabilities.Length; i++)
                {
                    if (probabilities[i] >= 1.0f)
                    {
                        hasEnsured = true;
                        break;
                    }
                }

                if (hasEnsured)
                {
                    for (var i = 0; i < probabilities.Length; i++)
                    {
                        if (probabilities[i] < 1.0f)
                            probabilities[i] = 0;
                    }
                }

                float sum = 0;
                for (var i = 0; i < probabilities.Length; i++) sum += probabilities[i];

                // All maps set to 0, why are you doing this???
                if (sum <= 0) return continueStart;
                for (var i = 0; i < probabilities.Length; i++)
                    // Normalize to [0,1]
                    probabilities[i] /= sum;
                var selection = (float)RebuildUs.Instance.Rnd.NextDouble();
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
}
