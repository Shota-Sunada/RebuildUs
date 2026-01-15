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
        foreach (InnerNet.ClientData client in AmongUsClient.Instance.allClients.ToArray())
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

                __instance.GameStartText.text = $"<color=#FF0000FF>The host has no or a different version of RebuildUs\nYou will be kicked in {Math.Round(10 - KickingTimer)}s</color>";
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
        }
        // Start Timer
        if (StartingTimer > 0)
        {
            StartingTimer -= Time.deltaTime;
        }
        // Lobby timer
        if (!GameData.Instance || !__instance.PlayerCounter) return; // No instance

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

                Version pV = PlayerVersions[client.Id];
                int diff = RebuildUs.Instance.Version.CompareTo(pV);
                if (diff != 0)
                {
                    continueStart = false;
                    break;
                }
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

