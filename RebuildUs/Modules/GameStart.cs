namespace RebuildUs.Modules;

internal static class GameStart
{
    internal static Dictionary<int, PlayerVersion> PlayerVersions = [];
    internal static float KickingTimer;
    internal static bool VersionSent;
    internal static string LobbyCodeText = "";

    internal static float StartingTimer;
    internal static bool SendGamemode = true;

    private static TextMeshPro _warningText;
    private static TextMeshPro _timerText;
    private static PassiveButton _cancelButton;
    private static float _timer = 600f;

    private static readonly StringBuilder InfoStringBuilder = new();
    private static string _lastCountDown = "";
    private static string _lastWarningMessage = "";

    internal static void OnPlayerJoined()
    {
        if (PlayerControl.LocalPlayer != null)
        {
            Helpers.ShareGameVersion();
        }

        SendGamemode = true;
    }

    internal static void Start(GameStartManager __instance)
    {
        // Reset dictionaries and flags
        PlayerVersions.Clear();
        VersionSent = false;
        // Reset kicking timer
        KickingTimer = 0f;

        _warningText = UnityObject.Instantiate(__instance.GameStartText, __instance.transform);
        _warningText.name = "WarningText";
        _warningText.transform.localPosition = new(0f, 0f - __instance.transform.localPosition.y, -1f);
        _warningText.gameObject.SetActive(false);

        _timerText = UnityObject.Instantiate(__instance.PlayerCounter, __instance.PlayerCounter.transform.parent);
        _timerText.autoSizeTextContainer = true;
        _timerText.fontSize = 3.2f;
        _timerText.name = "Timer";
        _timerText.DestroyChildren();
        _timerText.transform.localPosition += new Vector3(0.3f, -3.4f, 0f);
        _timerText.gameObject.SetActive(AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame && AmongUsClient.Instance.AmHost);

        _cancelButton = UnityObject.Instantiate(__instance.StartButton, __instance.transform);
        _cancelButton.name = "CancelButton";
        TextMeshPro cancelLabel = _cancelButton.buttonText;
        cancelLabel.GetComponent<TextTranslatorTMP>()?.Destroy();
        cancelLabel.text = "Cancel";
        _cancelButton.transform.localScale = new(0.5f, 0.5f, 1f);
        SpriteRenderer cancelButtonInactiveRenderer = _cancelButton.inactiveSprites.GetComponent<SpriteRenderer>();
        cancelButtonInactiveRenderer.color = new(0.8f, 0f, 0f, 1f);
        SpriteRenderer cancelButtonActiveRenderer = _cancelButton.activeSprites.GetComponent<SpriteRenderer>();
        cancelButtonActiveRenderer.color = Color.red;
        Transform cancelButtonInactiveShine = _cancelButton.inactiveSprites.transform.Find("Shine");
        if (cancelButtonInactiveShine)
        {
            cancelButtonInactiveShine.gameObject.SetActive(false);
        }

        _cancelButton.activeTextColor = _cancelButton.inactiveTextColor = Color.white;
        _cancelButton.transform.localPosition = new(2f, 0.13f, 0f);
        _cancelButton.OnClick = new();
        _cancelButton.OnClick.AddListener((Action)(() =>
        {
            __instance.ResetStartState();
            SoundManager.Instance.StopSound(GameStartManager.Instance.gameStartSound);
            {
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.StopStart);
            }
        }));
        _cancelButton.gameObject.SetActive(false);
    }

    internal static void OnPlayerLeft(int clientId)
    {
        PlayerVersions.Remove(clientId);
        if (PlayerVersions.Count == 0)
        {
            VersionSent = false;
        }
    }

    internal static void UpdatePostfix(GameStartManager __instance)
    {
        if (PlayerControl.LocalPlayer != null && !VersionSent && AmongUsClient.Instance.ClientId >= 0)
        {
            VersionSent = true;
            Helpers.ShareGameVersion();
        }

        // Check version handshake infos
        bool versionMismatch = false;
        InfoStringBuilder.Clear();

        Il2CppArrayBase<ClientData> clients = AmongUsClient.Instance.allClients.ToArray();
        foreach (ClientData client in clients)
        {
            if (client == null || client.Character == null || client.Character.Data == null)
            {
                continue;
            }

            // Skip Dummies
            DummyBehaviour dummyComponent = client.Character.GetComponent<DummyBehaviour>();
            if (dummyComponent != null && dummyComponent.enabled)
            {
                continue;
            }

            if (!PlayerVersions.TryGetValue(client.Id, out PlayerVersion pV))
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
            if (!PlayerVersions.TryGetValue(AmongUsClient.Instance.HostId, out PlayerVersion hostVersion)
                || !hostVersion.Matches(RebuildUs.Instance.Version))
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
                    _warningText.text = warning;
                    _lastWarningMessage = warning;
                }

                _warningText.gameObject.SetActive(true);
            }
            else if (versionMismatch)
            {
                InfoStringBuilder.Clear();
                InfoStringBuilder.Append("<color=#FF0000FF>Players With Different Versions:\n</color>");
                InfoStringBuilder.Append(message);
                string warning = InfoStringBuilder.ToString();
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
        if (StartingTimer > 0)
        {
            StartingTimer -= Time.deltaTime;
        }

        if (AmongUsClient.Instance.AmHost && _cancelButton != null)
        {
            _cancelButton.gameObject.SetActive(__instance.startState == GameStartManager.StartingStates.Countdown);
        }

        // Lobby timer
        if (!AmongUsClient.Instance.AmHost || !GameData.Instance || AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame)
        {
            return;
        }

        _timer = Mathf.Max(0f, _timer - Time.deltaTime);
        int totalSeconds = (int)_timer;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        InfoStringBuilder.Clear();
        if (minutes < 10)
        {
            InfoStringBuilder.Append('0');
        }
        InfoStringBuilder.Append(minutes);
        InfoStringBuilder.Append(':');
        if (seconds < 10)
        {
            InfoStringBuilder.Append('0');
        }
        InfoStringBuilder.Append(seconds);

        string countDown = InfoStringBuilder.ToString();
        if (_timer <= 60)
        {
            countDown = Helpers.Cs(Color.red, countDown);
        }

        if (_lastCountDown != countDown)
        {
            _timerText.text = countDown;
            _lastCountDown = countDown;
        }

        if (!GameData.Instance || !__instance.PlayerCounter)
        {
            return; // No instance
        }

        if (!AmongUsClient.Instance)
        {
            return;
        }

        if (!AmongUsClient.Instance.AmHost || !SendGamemode || PlayerControl.LocalPlayer == null)
        {
            return;
        }
        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareGamemode);
            sender.Write((byte)MapSettings.GameMode);
            RPCProcedure.ShareGamemode((byte)MapSettings.GameMode);
        }
        SendGamemode = false;
    }

    internal static bool BeginGame(GameStartManager __instance)
    {
        // Block game start if not everyone has the same mod version
        bool continueStart = true;

        if (!AmongUsClient.Instance.AmHost)
        {
            return continueStart;
        }
        foreach (ClientData client in AmongUsClient.Instance.allClients.GetFastEnumerator())
        {
            if (client.Character == null)
            {
                continue;
            }
            DummyBehaviour dummyComponent = client.Character.GetComponent<DummyBehaviour>();
            if (dummyComponent != null && dummyComponent.enabled)
            {
                continue;
            }

            if (PlayerVersions.TryGetValue(client.Id, out PlayerVersion pV) && pV.Matches(RebuildUs.Instance.Version))
            {
                continue;
            }
            continueStart = false;
            break;
        }

        if (!continueStart)
        {
            __instance.ResetStartState();
        }
        else
        {
            CustomOption.CoSpawnSyncSettings();
        }

        if (!CustomOptionHolder.RandomMap.GetBool() || !continueStart)
        {
            return continueStart;
        }
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
            CustomOptionHolder.RandomMapEnableMiraHq.GetSelection() / 10f,
            CustomOptionHolder.RandomMapEnablePolus.GetSelection() / 10f,
            CustomOptionHolder.RandomMapEnableAirShip.GetSelection() / 10f,
            CustomOptionHolder.RandomMapEnableFungle.GetSelection() / 10f,
            CustomOptionHolder.RandomMapEnableSubmerged.GetSelection() / 10f,
        ];

        // if any map is at 100%, remove all maps that are not!
        bool hasEnsured = false;
        foreach (float t in probabilities)
        {
            if (t >= 1.0f)
            {
                hasEnsured = true;
                break;
            }
        }

        if (hasEnsured)
        {
            for (int i = 0; i < probabilities.Length; i++)
            {
                if (probabilities[i] < 1.0f)
                {
                    probabilities[i] = 0;
                }
            }
        }

        float sum = 0;
        foreach (float t in probabilities)
        {
            sum += t;
        }

        // All maps set to 0, why are you doing this???
        if (sum <= 0)
        {
            return true;
        }
        for (int i = 0; i < probabilities.Length; i++)
            // Normalize to [0,1]
        {
            probabilities[i] /= sum;
        }

        float selection = (float)RebuildUs.Rnd.NextDouble();
        float cumSum = 0;
        for (byte i = 0; i < (byte)probabilities.Length; i++)
        {
            cumSum += probabilities[i];
            if (!(cumSum > selection))
            {
                continue;
            }
            chosenMapId = i;
            break;
        }

        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.DynamicMapOption);
            sender.Write(chosenMapId);
            RPCProcedure.DynamicMapOption(chosenMapId);
        }

        return true;
    }

    internal struct PlayerVersion
    {
        private readonly int _major;
        private readonly int _minor;
        private readonly int _build;
        private readonly int _revision;
        private readonly Guid _guid;

        internal PlayerVersion(int major, int minor, int build, int revision, Guid guid)
        {
            _major = major;
            _minor = minor;
            _build = build;
            _revision = revision;
            _guid = guid;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append(_major);
            sb.Append('.');
            sb.Append(_minor);
            sb.Append('.');
            sb.Append(_build);
            if (_revision < 0)
            {
                return sb.ToString();
            }
            sb.Append('.');
            sb.Append(_revision);

            return sb.ToString();
        }

        internal bool Matches(Version version)
        {
            return _major == version.Major && _minor == version.Minor && _build == version.Build;
        }

        internal bool GuidMatches(Guid guid)
        {
            return _guid == guid;
        }
    }
}