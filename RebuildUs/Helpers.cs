using System.Reflection;
using RebuildUs.Players;
using RebuildUs.Roles.Neutral;
using AmongUs.GameOptions;
using RebuildUs.Roles.Crewmate;
using System.Text;
using System.Text.RegularExpressions;

namespace RebuildUs;

public enum MurderAttemptResult
{
    PerformKill,
    SuppressKill,
    BlankKill,
    DelayVampireKill
}

public static class Helpers
{
    public static Dictionary<string, Sprite> CachedSprites = [];

    public static Sprite LoadSpriteFromResources(string path, float pixelsPerUnit, bool cache = true)
    {
        try
        {
            if (cache && CachedSprites.TryGetValue(path + pixelsPerUnit, out var sprite)) return sprite;
            Texture2D texture = LoadTextureFromResources(path);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            if (cache) sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
            if (!cache) return sprite;
            return CachedSprites[path + pixelsPerUnit] = sprite;
        }
        catch
        {
            Logger.LogError("Error loading sprite from path: {0}", [path]);
        }
        return null;
    }

    public static unsafe Texture2D LoadTextureFromResources(string path)
    {
        try
        {
            Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
            var assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(path);
            var length = stream.Length;
            var byteTexture = new Il2CppStructArray<byte>(length);
            stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
            if (path.Contains("HorseHats"))
            {
                byteTexture = new Il2CppStructArray<byte>([.. byteTexture.Reverse()]);
            }
            ImageConversion.LoadImage(texture, byteTexture, false);
            return texture;
        }
        catch
        {
            Logger.LogError("Error loading texture from resources: {0}", [path]);
        }
        return null;
    }

    public static Texture2D LoadTextureFromDisk(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
                var byteTexture = Il2CppSystem.IO.File.ReadAllBytes(path);
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            }
        }
        catch
        {
            Logger.LogError("Error loading texture from disk: {0}", [path]);
        }
        return null;
    }

    public static object TryCast(this Il2CppObjectBase self, Type type)
    {
        return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, []);
    }

    public static string Cs(Color c, string s)
    {
        return string.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>{4}</color>", ToByte(c.r), ToByte(c.g), ToByte(c.b), ToByte(c.a), s);
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

    public static int LineCount(string text)
    {
        return text.Count(c => c == '\n');
    }
    public static string PreviousEndGameSummary = "";

    public static bool HasFakeTasks(this PlayerControl player)
    {
        // return (player == Jester.jester || player == Jackal.jackal || player == Sidekick.sidekick || player == Arsonist.arsonist || player == Vulture.vulture || Jackal.formerJackals.Any(x => x == player));
        return false;
    }
    public static bool ZoomOutStatus = false;
    public static void ToggleZoom(bool reset = false)
    {
        float orthographicSize = reset || ZoomOutStatus ? 3f : 12f;

        ZoomOutStatus = !ZoomOutStatus && !reset;
        UnityEngine.Camera.main.orthographicSize = orthographicSize;
        foreach (var cam in UnityEngine.Camera.allCameras)
        {
            if (cam != null && cam.gameObject.name == "UI Camera") cam.orthographicSize = orthographicSize;  // The UI is scaled too, else we cant click the buttons. Downside: map is super small.
        }

        var tzGO = GameObject.Find("TOGGLEZOOMBUTTON");
        if (tzGO != null)
        {
            var rend = tzGO.transform.Find("Inactive").GetComponent<SpriteRenderer>();
            var rendActive = tzGO.transform.Find("Active").GetComponent<SpriteRenderer>();
            rend.sprite = ZoomOutStatus ? Helpers.LoadSpriteFromResources("TheOtherRoles.Resources.Plus_Button.png", 100f) : Helpers.LoadSpriteFromResources("TheOtherRoles.Resources.Minus_Button.png", 100f);
            rendActive.sprite = ZoomOutStatus ? Helpers.LoadSpriteFromResources("TheOtherRoles.Resources.Plus_ButtonActive.png", 100f) : Helpers.LoadSpriteFromResources("TheOtherRoles.Resources.Minus_ButtonActive.png", 100f);
            tzGO.transform.localScale = new Vector3(1.2f, 1.2f, 1f) * (ZoomOutStatus ? 4 : 1);
        }

        ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height, Screen.width, Screen.height, Screen.fullScreen); // This will move button positions to the correct position.
    }

    public static bool RolesEnabled { get { return true; } }
    public static bool RefundVotes { get { return true; } }

    public static PlayerControl PlayerById(byte id)
    {
        foreach (var player in CachedPlayer.AllPlayers)
        {
            if (player.PlayerId == id)
            {
                return player;
            }
        }
        return null;
    }

    public static void SetSemiTransparent(this PoolablePlayer player, bool value)
    {
        var alpha = value ? 0.25f : 1f;
        foreach (SpriteRenderer r in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            r.color = new Color(r.color.r, r.color.g, r.color.b, alpha);
        }
        player.cosmetics.nameText.color = new Color(player.cosmetics.nameText.color.r, player.cosmetics.nameText.color.g, player.cosmetics.nameText.color.b, alpha);
    }

    public static List<byte> GenerateTasks(int numCommon, int numShort, int numLong)
    {
        if (numCommon + numShort + numLong <= 0)
        {
            numShort = 1;
        }

        var tasks = new Il2CppSystem.Collections.Generic.List<byte>();
        var hashSet = new Il2CppSystem.Collections.Generic.HashSet<TaskTypes>();

        var commonTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.CommonTasks.OrderBy(x => RebuildUs.Instance.Rnd.Next())) commonTasks.Add(task);

        var shortTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.ShortTasks.OrderBy(x => RebuildUs.Instance.Rnd.Next())) shortTasks.Add(task);

        var longTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.LongTasks.OrderBy(x => RebuildUs.Instance.Rnd.Next())) longTasks.Add(task);

        int start = 0;
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numCommon, tasks, hashSet, commonTasks);

        start = 0;
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numShort, tasks, hashSet, shortTasks);

        start = 0;
        MapUtilities.CachedShipStatus.AddTasksFromList(ref start, numLong, tasks, hashSet, longTasks);

        return tasks.ToArray().ToList();
    }

    public static void ClearAllTasks(this PlayerControl player)
    {
        if (player == null) return;
        foreach (var playerTask in player.myTasks)
        {
            playerTask.OnRemove();
            UnityEngine.Object.Destroy(playerTask.gameObject);
        }
        player.myTasks.Clear();

        if (player.Data != null && player.Data.Tasks != null)
        {
            player.Data.Tasks.Clear();
        }
    }

    public static bool IsCrewmateAlive()
    {
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.IsTeamCrewmate() && player.IsAlive()) return true;
            // if (p.IsTeamCrewmate() && !p.hasModifier(ModifierType.Madmate) && p.isAlive()) return true;
        }
        return false;
    }

    public static bool HasImpostorVision(PlayerControl player)
    {
        return player.IsTeamImpostor()
        || ((player.IsRole(ERoleType.Jackal) || Jackal.FormerJackals.Any(x => x.PlayerId == player.PlayerId)) && Jackal.HasImpostorVision)
        || (player.IsRole(ERoleType.Sidekick) && Sidekick.HasImpostorVision)
        || (player.IsRole(ERoleType.Spy) && Spy.HasImpostorVision)
        || (player.IsRole(ERoleType.Jester) && Jester.HasImpostorVision);
    }

    public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
    {
        tie = true;
        KeyValuePair<byte, int> result = new(byte.MaxValue, int.MinValue);
        foreach (KeyValuePair<byte, int> keyValuePair in self)
        {
            if (keyValuePair.Value > result.Value)
            {
                result = keyValuePair;
                tie = false;
            }
            else if (keyValuePair.Value == result.Value)
            {
                tie = true;
            }
        }
        return result;
    }

    public static MurderAttemptResult CheckMurderAttempt(PlayerControl killer, PlayerControl target, bool blockRewind = false, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false, bool ignoreMedic = false)
    {
        var targetRole = RoleInfo.GetRoleInfoForPlayer(target, false).FirstOrDefault();
        // Modified vanilla checks
        if (AmongUsClient.Instance.IsGameOver) return MurderAttemptResult.SuppressKill;
        if (killer == null || killer.Data == null || (killer.Data.IsDead && !ignoreIfKillerIsDead) || killer.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow non Impostor kills compared to vanilla code
        if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected) return MurderAttemptResult.SuppressKill; // Allow killing players in vents compared to vanilla code
        if (GameOptionsManager.Instance.currentGameOptions.GameMode == AmongUs.GameOptions.GameModes.HideNSeek) return MurderAttemptResult.PerformKill;

        // Handle first kill attempt
        if (MapOptions.ShieldFirstKill && MapOptions.FirstKillPlayer == target) return MurderAttemptResult.SuppressKill;

        // Block impostor shielded kill
        // if (!ignoreMedic && Medic.shielded != null && Medic.shielded == target)
        // {
        //     MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.ShieldedMurderAttempt, Hazel.SendOption.Reliable, -1);
        //     AmongUsClient.Instance.FinishRpcImmediately(writer);
        //     RPCProcedure.shieldedMurderAttempt();
        //     return MurderAttemptResult.SuppressKill;
        // }

        // Block impostor not fully grown mini kill
        // else if (Mini.mini != null && target == Mini.mini && !Mini.isGrownUp())
        // {
        //     return MurderAttemptResult.SuppressKill;
        // }

        // Block Time Master with time shield kill
        // else if (TimeMaster.shieldActive && TimeMaster.timeMaster != null && TimeMaster.timeMaster == target)
        // {
        //     if (!blockRewind)
        //     {
        //         // Only rewind the attempt was not called because a meeting startet
        //         MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.TimeMasterRewindTime, Hazel.SendOption.Reliable, -1);
        //         AmongUsClient.Instance.FinishRpcImmediately(writer);
        //         RPCProcedure.timeMasterRewindTime();
        //     }
        //     return MurderAttemptResult.SuppressKill;
        // }

        // if (TransportationToolPatches.isUsingTransportation(target) && !blockRewind && killer == Vampire.vampire)
        // {
        //     return MurderAttemptResult.DelayVampireKill;
        // }
        // else if (TransportationToolPatches.isUsingTransportation(target))
        // {
        //     return MurderAttemptResult.SuppressKill;
        // }

        return MurderAttemptResult.PerformKill;
    }

    public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation)
    {
        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
        sender.Write(killer.PlayerId);
        sender.Write(target.PlayerId);
        sender.Write(showAnimation ? byte.MaxValue : 0);
        RPCProcedure.UncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? byte.MaxValue : (byte)0);
    }

    public static MurderAttemptResult CheckMurderAttemptAndKill(PlayerControl killer, PlayerControl target, bool isMeetingStart = false, bool showAnimation = true, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false)
    {
        // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
        // The kill attempt will be shared using a custom RPC, hence combining modded and unmodded versions is impossible
        var murder = CheckMurderAttempt(killer, target, isMeetingStart, ignoreBlank, ignoreIfKillerIsDead);

        if (murder == MurderAttemptResult.PerformKill)
        {
            MurderPlayer(killer, target, showAnimation);
        }
        // else if (murder == MurderAttemptResult.DelayVampireKill)
        // {
        //     HudManager.Instance.StartCoroutine(Effects.Lerp(10f, new Action<float>((p) =>
        //     {
        //         if (!TransportationToolPatches.isUsingTransportation(target) && Vampire.bitten != null)
        //         {
        //             MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
        //             writer.Write(byte.MaxValue);
        //             writer.Write(byte.MaxValue);
        //             AmongUsClient.Instance.FinishRpcImmediately(writer);
        //             RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
        //             MurderPlayer(killer, target, showAnimation);
        //         }
        //     })));
        // }
        return murder;
    }

    public static bool SabotageActive()
    {
        var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        return sabSystem.AnyActive;
    }

    public static float SabotageTimer()
    {
        var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        return sabSystem.Timer;
    }
    public static bool CanUseSabotage()
    {
        var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        ISystemType systemType;
        IActivatable doors = null;
        if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Doors, out systemType))
        {
            doors = systemType.CastFast<IActivatable>();
        }
        return GameManager.Instance.SabotagesEnabled() && sabSystem.Timer <= 0f && !sabSystem.AnyActive && !(doors != null && doors.IsActive);
    }

    public static PlayerControl SetTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null, int killDistance = 3)
    {
        PlayerControl result = null;
        float num = NormalGameOptionsV10.KillDistances[Mathf.Clamp(GameOptionsManager.Instance.CurrentGameOptions.GetInt(Int32OptionNames.KillDistance), 0, 2)];
        if (!MapUtilities.CachedShipStatus) return result;
        if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
        if (targetingPlayer.Data.IsDead || targetingPlayer.inVent) return result;
        if (targetingPlayer.IsGM()) return result;

        untargetablePlayers ??= [];

        // GM is untargetable by anything
        // if (GM.gm != null)
        // {
        //     untargetablePlayers.Add(GM.gm);
        // }

        // Can't target stealthed ninjas if setting on
        // if (!Ninja.canBeTargeted)
        // {
        //     foreach (Ninja n in Ninja.players)
        //     {
        //         if (n.stealthed) untargetablePlayers.Add(n.player);
        //     }
        // }

        var truePosition = targetingPlayer.GetTruePosition();
        foreach (var playerInfo in GameData.Instance.AllPlayers)
        {
            if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && !playerInfo.IsDead && (!onlyCrewmates || !playerInfo.Role.IsImpostor))
            {
                var @object = playerInfo.Object;
                if (untargetablePlayers.Any(x => x == @object))
                {
                    // if that player is not targetable: skip check
                    continue;
                }

                if (@object && (!@object.inVent || targetPlayersInVents))
                {
                    Vector2 vector = @object.GetTruePosition() - truePosition;
                    float magnitude = vector.magnitude;
                    if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                    {
                        result = @object;
                        num = magnitude;
                    }
                }
            }
        }
        return result;
    }

    public static void SetPlayerOutline(PlayerControl target, Color color)
    {
        if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) return;

        target.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 1f);
        target.cosmetics?.currentBodySprite?.BodySprite.material.SetColor("_OutlineColor", color);
    }

    // Update functions

    public static void SetBasePlayerOutlines()
    {
        foreach (PlayerControl target in CachedPlayer.AllPlayers)
        {
            if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

            // bool isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
            // bool hasVisibleShield = false;
            // if (Camouflager.camouflageTimer <= 0f && Medic.shielded != null && ((target == Medic.shielded && !isMorphedMorphling) || (isMorphedMorphling && Morphling.morphTarget == Medic.shielded)))
            // {
            //     hasVisibleShield = Medic.showShielded == 0 // Everyone
            //         || (Medic.showShielded == 1 && (CachedPlayer.LocalPlayer.PlayerControl == Medic.shielded || CachedPlayer.LocalPlayer.PlayerControl == Medic.medic)) // Shielded + Medic
            //         || (Medic.showShielded == 2 && CachedPlayer.LocalPlayer.PlayerControl == Medic.medic); // Medic only
            // }

            // if (hasVisibleShield)
            // {
            //     target.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 1f);
            //     target.cosmetics?.currentBodySprite?.BodySprite.material.SetColor("_OutlineColor", Medic.shieldedColor);
            // }
            // else
            {
                target.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 0f);
            }
        }
    }

    public static void RefreshRoleDescription(PlayerControl player)
    {
        List<RoleInfo> infos = RoleInfo.GetRoleInfoForPlayer(player);
        List<string> taskTexts = new(infos.Count);

        foreach (var roleInfo in infos)
        {
            taskTexts.Add(GetRoleString(roleInfo));
        }

        var toRemove = new List<PlayerTask>();
        foreach (PlayerTask t in player.myTasks.GetFastEnumerator())
        {
            var textTask = t.TryCast<ImportantTextTask>();
            if (textTask == null) continue;

            var currentText = textTask.Text;

            if (taskTexts.Contains(currentText)) taskTexts.Remove(currentText); // TextTask for this RoleInfo does not have to be added, as it already exists
            else toRemove.Add(t); // TextTask does not have a corresponding RoleInfo and will hence be deleted
        }

        foreach (PlayerTask t in toRemove)
        {
            t.OnRemove();
            player.myTasks.Remove(t);
            UnityEngine.Object.Destroy(t.gameObject);
        }

        // Add TextTask for remaining RoleInfos
        foreach (string title in taskTexts)
        {
            var task = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
            task.transform.SetParent(player.transform, false);
            task.Text = title;
            player.myTasks.Insert(0, task);
        }
    }

    internal static string GetRoleString(RoleInfo roleInfo)
    {
        if (roleInfo.Name == "Jackal")
        {
            var getSidekickText = Jackal.CanCreateSidekick ? " and recruit a Sidekick" : "";
            return Cs(roleInfo.Color, $"{roleInfo.Name}: Kill everyone{getSidekickText}");
        }

        return Cs(roleInfo.Color, $"{roleInfo.Name}: {roleInfo.ShortDescription}");
    }

    public static void UpdatePlayerInfo()
    {
        var colorBlindTextMeetingInitialLocalPos = new Vector3(0.3384f, -0.16666f, -0.01f);
        var colorBlindTextMeetingInitialLocalScale = new Vector3(0.9f, 1f, 1f);
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            // Colorblind Text in Meeting
            var playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
            if (playerVoteArea != null && playerVoteArea.ColorBlindName.gameObject.active)
            {
                playerVoteArea.ColorBlindName.transform.localPosition = colorBlindTextMeetingInitialLocalPos + new Vector3(0f, 0.4f, 0f);
                playerVoteArea.ColorBlindName.transform.localScale = colorBlindTextMeetingInitialLocalScale * 0.8f;
            }

            // Colorblind Text During the round
            if (p.cosmetics.colorBlindText != null && p.cosmetics.showColorBlindText && p.cosmetics.colorBlindText.gameObject.active)
            {
                p.cosmetics.colorBlindText.transform.localPosition = new Vector3(0, -1f, 0f);
            }

            p.cosmetics.nameText.transform.parent.SetLocalZ(-0.0001f);  // This moves both the name AND the colorblind text behind objects (if the player is behind the object), like the rock on polus

            if (p == PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data.IsDead)
            {
                var playerInfoTransform = p.cosmetics.nameText.transform.parent.FindChild("Info");
                var playerInfo = playerInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                if (playerInfo == null)
                {
                    playerInfo = UnityEngine.Object.Instantiate(p.cosmetics.nameText, p.cosmetics.nameText.transform.parent);
                    playerInfo.transform.localPosition += Vector3.up * 0.225f;
                    playerInfo.fontSize *= 0.75f;
                    playerInfo.gameObject.name = "Info";
                    playerInfo.color = playerInfo.color.SetAlpha(1f);
                }

                var meetingInfoTransform = playerVoteArea?.NameText.transform.parent.FindChild("Info");
                var meetingInfo = meetingInfoTransform?.GetComponent<TMPro.TextMeshPro>();
                if (meetingInfo == null && playerVoteArea != null)
                {
                    meetingInfo = UnityEngine.Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                    meetingInfo.transform.localPosition += Vector3.down * 0.2f;
                    meetingInfo.fontSize *= 0.60f;
                    meetingInfo.gameObject.name = "Info";
                }

                // Set player name higher to align in middle
                if (meetingInfo != null && playerVoteArea != null)
                {
                    var playerName = playerVoteArea.NameText;
                    playerName.transform.localPosition = new Vector3(0.3384f, 0.0311f, -0.1f);
                }

                var (tasksCompleted, tasksTotal) = TasksHandler.TaskInfo(p.Data);
                string roleNames = RoleInfo.GetRolesString(p, true, false);
                string roleText = RoleInfo.GetRolesString(p, true, MapOptions.GhostsSeeModifier);
                string taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({tasksCompleted}/{tasksTotal})</color>" : "";

                string playerInfoText = "";
                string meetingInfoText = "";
                if (p == PlayerControl.LocalPlayer)
                {
                    if (p.Data.IsDead) roleNames = roleText;
                    playerInfoText = $"{roleNames}";
                    // if (p == Swapper.swapper) playerInfoText = $"{roleNames}" + Helpers.cs(Swapper.color, $" ({Swapper.charges})");
                    if (HudManager.Instance.TaskPanel != null)
                    {
                        var tabText = HudManager.Instance.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TextMeshPro>();
                        tabText.SetText($"Tasks {taskInfo}");
                    }
                    meetingInfoText = $"{roleNames} {taskInfo}".Trim();
                }
                else if (MapOptions.GhostsSeeRoles && MapOptions.GhostsSeeInformation)
                {
                    playerInfoText = $"{roleText} {taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (MapOptions.GhostsSeeInformation)
                {
                    playerInfoText = $"{taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (MapOptions.GhostsSeeRoles)
                {
                    playerInfoText = $"{roleText}";
                    meetingInfoText = playerInfoText;
                }

                playerInfo.text = playerInfoText;
                playerInfo.gameObject.SetActive(p.Visible);
                meetingInfo?.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
            }
        }
    }

    public static void SetPetVisibility()
    {
        var localDead = PlayerControl.LocalPlayer.Data.IsDead;
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            var playerAlive = !player.Data.IsDead;
            player.cosmetics.SetPetVisible((localDead && playerAlive) || !localDead);
        }
    }

    public static void ShareGameVersion()
    {
        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VersionHandshake, SendOption.Reliable, -1);
        writer.Write((byte)RebuildUs.Instance.Version.Major);
        writer.Write((byte)RebuildUs.Instance.Version.Minor);
        writer.Write((byte)RebuildUs.Instance.Version.Build);
        writer.Write(AmongUsClient.Instance.AmHost ? GameStart.Timer : -1f);
        writer.WritePacked(AmongUsClient.Instance.ClientId);
        writer.Write((byte)(RebuildUs.Instance.Version.Revision < 0 ? 0xFF : RebuildUs.Instance.Version.Revision));
        writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
        AmongUsClient.Instance.FinishRpcImmediately(writer);
        RPCProcedure.VersionHandshake(RebuildUs.Instance.Version.Major, RebuildUs.Instance.Version.Minor, RebuildUs.Instance.Version.Build, RebuildUs.Instance.Version.Revision, Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
    }

    public static string PadRightV2(this object text, int num)
    {
        int bc = 0;
        var t = text.ToString();
        foreach (char c in t) bc += Encoding.GetEncoding("UTF-8").GetByteCount(c.ToString()) == 1 ? 1 : 2;
        return t?.PadRight(num - (bc - t.Length));
    }

    public static string RemoveHtml(this string text) => Regex.Replace(text, "<[^>]*?>", "");

}