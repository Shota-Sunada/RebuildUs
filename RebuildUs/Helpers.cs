using Object = UnityEngine.Object;

namespace RebuildUs;

internal enum MurderAttemptResult
{
    PerformKill,
    SuppressKill,
    BlankKill,
    DelayVampireKill,
}

internal static class Helpers
{
    private static readonly StringBuilder ColorStringBuilder = new();

    private static readonly StringBuilder InfoStringBuilder = new();

    private static readonly Dictionary<byte, PlayerControl> PlayerByIdCache = [];
    private static int _lastCacheFrame = -1;

    private static readonly Vector3 ColorBlindMeetingPos = new(0.3384f, 0.23334f, -0.11f);
    private static readonly Vector3 ColorBlindMeetingScale = new(0.72f, 0.8f, 0.8f);
    private static readonly Dictionary<byte, PlayerVoteArea> VoteAreaStates = [];

    private static readonly StringBuilder RoleStringBuilder = new();

    internal static bool ShowButtons
    {
        get => !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && !MeetingHud.Instance && !ExileController.Instance;
    }

    internal static bool ShowMeetingText
    {
        get => MeetingHud.Instance != null
               && MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion;
    }

    internal static bool GameStarted
    {
        get => AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Started;
    }

    internal static bool RolesEnabled
    {
        get => CustomOptionHolder.ActivateRoles.GetBool();
    }

    internal static bool RefundVotes
    {
        get => CustomOptionHolder.RefundVotesOnDeath.GetBool();
    }

    internal static bool IsSkeld
    {
        get => ByteOptionNames.MapId.Get() == 0;
    }

    internal static bool IsMiraHq
    {
        get => ByteOptionNames.MapId.Get() == 1;
    }

    internal static bool IsPolus
    {
        get => ByteOptionNames.MapId.Get() == 2;
    }

    internal static bool IsAirship
    {
        get => ByteOptionNames.MapId.Get() == 4;
    }

    internal static bool IsFungle
    {
        get => ByteOptionNames.MapId.Get() == 5;
    }

    internal static bool IsHideNSeekMode
    {
        get => GameManager.Instance.IsHideAndSeek();
    }

    internal static bool IsNormal
    {
        get => GameManager.Instance.IsNormal();
    }

    internal static bool IsCountdown
    {
        get => GameStartManager.InstanceExists && FastDestroyableSingleton<GameStartManager>.Instance.startState is GameStartManager.StartingStates.Countdown;
    }

    internal static void DestroyList<T>(Il2CppSystem.Collections.Generic.List<T> items) where T : Object
    {
        if (items == null)
        {
            return;
        }
        foreach (var item in items)
        {
            if (item != null)
            {
                Object.Destroy(item);
            }
        }
        items.Clear();
    }

    internal static T[] CastArray<T>(object[] items)
    {
        if (items == null)
        {
            return [];
        }

        var result = new T[items.Length];
        for (var i = 0; i < items.Length; i++)
        {
            result[i] = (T)items[i];
        }

        return result;
    }

    internal static void DestroyList<T>(IEnumerable<T> items) where T : Object
    {
        if (items == null)
        {
            return;
        }
        foreach (var item in items)
        {
            Object.Destroy(item);
        }
    }

    internal static object TryCast(this Il2CppObjectBase self, Type type)
    {
        return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, []);
    }

    internal static string Cs(Color c, string s)
    {
        ColorStringBuilder.Clear();
        ColorStringBuilder
            .Append("<color=#")
            .Append(ToByte(c.r).ToString("X2"))
            .Append(ToByte(c.g).ToString("X2"))
            .Append(ToByte(c.b).ToString("X2"))
            .Append(ToByte(c.a).ToString("X2"))
            .Append('>')
            .Append(s)
            .Append("</color>");
        return ColorStringBuilder.ToString();
    }

    private static byte ToByte(float f)
    {
        return (byte)(Mathf.Clamp01(f) * 255);
    }

    internal static bool GetKeysDown(params KeyCode[] keys)
    {
        if (keys.Length == 0)
        {
            return false;
        }
        var allHeld = true;
        var anyJustPressed = false;
        foreach (var key in keys)
        {
            if (!Input.GetKey(key))
            {
                allHeld = false;
                break;
            }

            if (Input.GetKeyDown(key))
            {
                anyJustPressed = true;
            }
        }

        return allHeld && anyJustPressed;
    }

    internal static bool HasFakeTasks(this PlayerControl player)
    {
        return player.IsNeutral() && !player.NeutralHasTasks()
               || player.HasModifier(ModifierType.CreatedMadmate) && !CreatedMadmate.HasTasks
               || player.HasModifier(ModifierType.Madmate) && !Madmate.HasTasks
               || player.IsRole(RoleType.Madmate) && !MadmateRole.CanKnowImpostorAfterFinishTasks
               || player.IsRole(RoleType.Suicider) && !Suicider.CanKnowImpostorAfterFinishTasks
               || player.IsLovers() && Lovers.SeparateTeam && !Lovers.TasksCount;
    }

    internal static PlayerControl PlayerById(byte id)
    {
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.PlayerId == id)
            {
                return player;
            }
        }

        return null;
    }

    internal static Dictionary<byte, PlayerControl> AllPlayersById()
    {
        if (Time.frameCount == _lastCacheFrame)
        {
            return PlayerByIdCache;
        }

        _lastCacheFrame = Time.frameCount;
        PlayerByIdCache.Clear();
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p != null)
            {
                PlayerByIdCache[p.PlayerId] = p;
            }
        }

        return PlayerByIdCache;
    }

    internal static void HandleVampireBiteOnBodyReport()
    {
        // Murder the bitten player and reset bitten (regardless whether the kill was successful or not)
        var killer = Vampire.PlayerControl;
        if (killer != null && Vampire.Bitten != null)
        {
            CheckMurderAttemptAndKill(killer, Vampire.Bitten, true, false);
        }

        using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten))
        {
            sender.Write(byte.MaxValue);
            sender.Write(byte.MaxValue);
        }

        RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);
    }

    internal static bool IsLighterColor(int colorId)
    {
        return CustomColors.LighterColors.Contains(colorId);
    }

    internal static bool MushroomSabotageActive()
    {
        foreach (var task in PlayerControl.LocalPlayer.myTasks)
        {
            if (task.TaskType == TaskTypes.MushroomMixupSabotage)
            {
                return true;
            }
        }

        return false;
    }

    internal static void SetSemiTransparent(this PoolablePlayer player, bool value)
    {
        var alpha = value ? 0.25f : 1f;
        foreach (var r in player.gameObject.GetComponentsInChildren<SpriteRenderer>())
        {
            var c = r.color;
            r.color = new(c.r, c.g, c.b, alpha);
        }

        var nameTxt = player.cosmetics.nameText;
        nameTxt.color = new(nameTxt.color.r, nameTxt.color.g, nameTxt.color.b, alpha);
    }

    internal static bool IsCrewmateAlive()
    {
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsTeamCrewmate()
                && !p.HasModifier(ModifierType.Madmate)
                && !p.IsRole(RoleType.Madmate)
                && !p.IsRole(RoleType.Suicider)
                && p.IsAlive())
            {
                return true;
            }
        }

        return false;
    }

    internal static bool HasImpostorVision(PlayerControl player)
    {
        if (player.IsTeamImpostor())
        {
            return true;
        }

        var isFormerJackal = false;
        foreach (var p in Jackal.FormerJackals)
        {
            if (p.PlayerId == player.PlayerId)
            {
                isFormerJackal = true;
                break;
            }
        }

        return (player.IsRole(RoleType.Jackal) || isFormerJackal) && Jackal.HasImpostorVision
               || player.IsRole(RoleType.Sidekick) && Sidekick.HasImpostorVision
               || player.IsRole(RoleType.Spy) && Spy.HasImpostorVision
               || player.IsRole(RoleType.Madmate) && MadmateRole.HasImpostorVision
               || player.IsRole(RoleType.Suicider) && Suicider.HasImpostorVision
               || player.IsRole(RoleType.Jester) && Jester.HasImpostorVision;
    }

    internal static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
    {
        tie = true;
        var maxKey = byte.MaxValue;
        var maxValue = int.MinValue;

        foreach (var pair in self)
        {
            if (pair.Value > maxValue)
            {
                maxValue = pair.Value;
                maxKey = pair.Key;
                tie = false;
            }
            else if (pair.Value == maxValue)
            {
                tie = true;
            }
        }

        return new(maxKey, maxValue);
    }

    internal static MurderAttemptResult CheckMurderAttempt(PlayerControl killer,
                                                           PlayerControl target,
                                                           bool blockRewind = false,
                                                           bool ignoreBlank = false,
                                                           bool ignoreIfKillerIsDead = false,
                                                           bool ignoreMedic = false)
    {
        if (AmongUsClient.Instance.IsGameOver)
        {
            return MurderAttemptResult.SuppressKill;
        }
        if (killer == null || killer.Data == null || killer.Data.IsDead && !ignoreIfKillerIsDead || killer.Data.Disconnected)
        {
            return MurderAttemptResult.SuppressKill;
        }
        if (target == null || target.Data == null || target.Data.IsDead || target.Data.Disconnected)
        {
            return MurderAttemptResult.SuppressKill;
        }
        if (IsHideNSeekMode)
        {
            return MurderAttemptResult.PerformKill;
        }

        if (Medic.Exists && !ignoreMedic && Medic.Shielded != null && Medic.Shielded == target)
        {
            using (new RPCSender(killer.NetId, CustomRPC.ShieldedMurderAttempt)) RPCProcedure.ShieldedMurderAttempt();

            return MurderAttemptResult.SuppressKill;
        }

        if (Mini.Exists && target.HasModifier(ModifierType.Mini) && !Mini.IsGrownUp(target))
        {
            return MurderAttemptResult.SuppressKill;
        }

        if (TimeMaster.Exists && TimeMaster.ShieldActive && target.IsRole(RoleType.TimeMaster))
        {
            if (!blockRewind)
            {
                using (new RPCSender(killer.NetId, CustomRPC.TimeMasterRewindTime)) RPCProcedure.TimeMasterRewindTime();
            }

            return MurderAttemptResult.SuppressKill;
        }

        return MurderAttemptResult.PerformKill;
    }

    internal static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation)
    {
        using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer))
        {
            sender.Write(killer.PlayerId);
            sender.Write(target.PlayerId);
            sender.Write(showAnimation ? byte.MaxValue : (byte)0);
        }

        RPCProcedure.UncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? byte.MaxValue : (byte)0);
    }

    internal static MurderAttemptResult CheckMurderAttemptAndKill(PlayerControl killer,
                                                                  PlayerControl target,
                                                                  bool isMeetingStart = false,
                                                                  bool showAnimation = true,
                                                                  bool ignoreBlank = false,
                                                                  bool ignoreIfKillerIsDead = false)
    {
        var murder = CheckMurderAttempt(killer, target, isMeetingStart, ignoreBlank, ignoreIfKillerIsDead);
        Logger.LogMessage(Enum.GetName(typeof(MurderAttemptResult), murder));

        if (murder == MurderAttemptResult.PerformKill)
        {
            MurderPlayer(killer, target, showAnimation);
        }
        else if (murder == MurderAttemptResult.DelayVampireKill)
        {
            FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(10f,
                new Action<float>(p =>
                {
                    if (!TransportationToolPatches.IsUsingTransportation(target) && Vampire.Bitten != null)
                    {
                        using (new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.VampireSetBitten))
                            RPCProcedure.VampireSetBitten(byte.MaxValue, byte.MaxValue);

                        MurderPlayer(killer, target, showAnimation);
                    }
                })));
        }

        return murder;
    }

    internal static bool SabotageActive()
    {
        return MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().AnyActive;
    }

    internal static float SabotageTimer()
    {
        return MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>().Timer;
    }

    internal static bool CanUseSabotage()
    {
        var sabSystem = MapUtilities.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        IActivatable doors = null;
        if (MapUtilities.Systems.TryGetValue(SystemTypes.Doors, out var systemType))
        {
            doors = systemType.CastFast<IActivatable>();
        }

        return GameManager.Instance.SabotagesEnabled() && sabSystem.Timer <= 0f && !sabSystem.AnyActive && !(doors != null && doors.IsActive);
    }

    internal static PlayerControl SetTarget(bool onlyCrewmates = false,
                                            bool targetPlayersInVents = false,
                                            List<PlayerControl> untargetablePlayers = null,
                                            PlayerControl targetingPlayer = null,
                                            int killDistanceIdx = -1)
    {
        if (!MapUtilities.CachedShipStatus)
        {
            return null;
        }

        targetingPlayer ??= PlayerControl.LocalPlayer;
        if (targetingPlayer.Data.IsDead || targetingPlayer.inVent || targetingPlayer.IsGm())
        {
            return null;
        }

        var num = NormalGameOptionsV10.KillDistances[Mathf.Clamp(killDistanceIdx == -1 ? Get(Int32OptionNames.KillDistance) : killDistanceIdx,
            0,
            2)];
        untargetablePlayers ??= [];

        var truePosition = targetingPlayer.GetTruePosition();
        PlayerControl result = null;

        foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            if (playerInfo.Disconnected || playerInfo.PlayerId == targetingPlayer.PlayerId || playerInfo.IsDead)
            {
                continue;
            }
            if (onlyCrewmates && playerInfo.Role.IsImpostor)
            {
                continue;
            }

            var obj = playerInfo.Object;
            if (obj == null || obj.inVent && !targetPlayersInVents)
            {
                continue;
            }

            var untargetable = false;
            foreach (var utp in untargetablePlayers)
            {
                if (utp == obj)
                {
                    untargetable = true;
                    break;
                }
            }

            if (untargetable)
            {
                continue;
            }

            var vector = obj.GetTruePosition() - truePosition;
            var magnitude = vector.magnitude;
            if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
            {
                result = obj;
                num = magnitude;
            }
        }

        return result;
    }

    internal static void SetPlayerOutline(PlayerControl target, Color color)
    {
        if (target?.cosmetics?.currentBodySprite?.BodySprite == null)
        {
            return;
        }

        var mat = target.cosmetics.currentBodySprite.BodySprite.material;
        mat.SetFloat("_Outline", 1f);
        mat.SetColor("_OutlineColor", color);
    }

    internal static void SetBasePlayerOutlines()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        foreach (var target in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (target?.cosmetics?.currentBodySprite?.BodySprite == null)
            {
                continue;
            }

            var isMorphedMorphing = target.IsRole(RoleType.Morphing) && Morphing.MorphTarget != null && Morphing.MorphTimer > 0f;
            var hasVisibleShield = false;
            if (Camouflager.CamouflageTimer <= 0f
                && Medic.Shielded != null
                && (target == Medic.Shielded && !isMorphedMorphing || isMorphedMorphing && Morphing.MorphTarget == Medic.Shielded))
            {
                hasVisibleShield = Medic.ShowShielded switch
                {
                    0 => true, // Everyone
                    1 => localPlayer == Medic.Shielded || localPlayer.IsRole(RoleType.Medic), // Shielded + Medic
                    2 => localPlayer.IsRole(RoleType.Medic), // Medic only
                    _ => false,
                };
            }

            var mat = target.cosmetics.currentBodySprite.BodySprite.material;
            if (hasVisibleShield)
            {
                mat.SetFloat("_Outline", 1f);
                mat.SetColor("_OutlineColor", Medic.ShieldedColor);
            }
            else
            {
                mat.SetFloat("_Outline", 0f);
            }
        }
    }

    internal static void ShareGameVersion(int targetId = -1)
    {
        if (AmongUsClient.Instance.ClientId < 0)
        {
            return;
        }

        var ver = RebuildUs.Instance.Version;
        using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.VersionHandshake, targetId))
        {
            sender.Write((byte)ver.Major);
            sender.Write((byte)ver.Minor);
            sender.Write((byte)ver.Build);
            sender.WritePacked(AmongUsClient.Instance.ClientId);
            sender.Write((byte)(ver.Revision < 0 ? 0xFF : ver.Revision));
            sender.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
        }

        if (targetId == -1 || targetId == AmongUsClient.Instance.ClientId)
        {
            RPCProcedure.VersionHandshake(ver.Major,
                ver.Minor,
                ver.Build,
                ver.Revision,
                AmongUsClient.Instance.ClientId,
                Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId);
        }
    }

    internal static string PadRightV2(this object text, int num)
    {
        var bc = 0;
        var t = text.ToString();
        foreach (var c in t)
        {
            bc += Encoding.UTF8.GetByteCount(c.ToString()) == 1 ? 1 : 2;
        }
        return t?.PadRight(num - (bc - t.Length));
    }

    internal static string RemoveHtml(this string text)
    {
        return Regex.Replace(text, "<[^>]*?>", "");
    }

    internal static bool HidePlayerName(PlayerControl target)
    {
        return HidePlayerName(PlayerControl.LocalPlayer, target);
    }

    internal static bool HidePlayerName(PlayerControl source, PlayerControl target)
    {
        if (source == target)
        {
            return false;
        }
        if (source == null || target == null)
        {
            return true;
        }
        if (source.IsDead())
        {
            return false;
        }
        if (target.IsDead())
        {
            return true;
        }
        if (Camouflager.CamouflageTimer > 0f)
        {
            return true;
        }

        if (MapSettings.HideOutOfSightNametags && GameStarted && MapUtilities.CachedShipStatus != null)
        {
            var distMod = 1.025f;
            var distance = Vector3.Distance(source.transform.position, target.transform.position);
            var blocked = PhysicsHelpers.AnythingBetween(source.GetTruePosition(), target.GetTruePosition(), Constants.ShadowMask, false);

            if (distance > MapUtilities.CachedShipStatus.CalculateLightRadius(source.Data) * distMod || blocked)
            {
                return true;
            }
        }

        if (!MapSettings.HidePlayerNames)
        {
            return false;
        }
        if (source.IsTeamImpostor()
            && (target.IsTeamImpostor()
                || target.IsRole(RoleType.Spy)
                || target.IsRole(RoleType.Sidekick) && Sidekick.Instance.WasTeamRed
                || target.IsRole(RoleType.Jackal) && Jackal.Instance.WasTeamRed))
        {
            return false;
        }
        if (source.GetPartner() == target)
        {
            return false;
        }
        if ((source.IsRole(RoleType.Jackal) || source.IsRole(RoleType.Sidekick))
            && (target.IsRole(RoleType.Jackal) || target.IsRole(RoleType.Sidekick) || target == Jackal.Instance.FakeSidekick))
        {
            return false;
        }

        return true;
    }

    internal static void OnObjectDestroy(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }
        var name = obj.name;
        if (name == null)
        {
            return;
        }

        // night vision
        if (name.Contains("FungleSecurity"))
        {
            // SurveillanceMinigamePatch.resetNightVision();
            return;
        }

        // submerged
        if (!SubmergedCompatibility.IsSubmerged)
        {
            return;
        }

        if (!name.Contains("ExileCutscene"))
        {
            return;
        }
        var controller = obj.GetComponent<ExileController>();
        if (controller != null && controller.initData != null)
        {
            ExileControllerPatch.WrapUpPostfix(controller.initData.networkedPlayer?.Object);
        }
    }

    internal static void ShowFlash(Color color, float duration = 1f)
    {
        var hud = FastDestroyableSingleton<HudManager>.Instance;
        if (hud?.FullScreen == null)
        {
            return;
        }

        hud.FullScreen.gameObject.SetActive(true);
        hud.FullScreen.enabled = true;
        hud.StartCoroutine(Effects.Lerp(duration,
            new Action<float>(p =>
            {
                var renderer = hud.FullScreen;
                if (renderer == null)
                {
                    return;
                }

                var alpha = p < 0.5f ? p * 2f * 0.75f : (1f - p) * 2f * 0.75f;
                renderer.color = new(color.r, color.g, color.b, Mathf.Clamp01(alpha));

                if (p == 1f)
                {
                    var reactorActive = false;
                    foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
                    {
                        if (task.TaskType == TaskTypes.StopCharles)
                        {
                            reactorActive = true;
                            break;
                        }
                    }

                    if (!reactorActive && IsAirship)
                    {
                        renderer.color = Color.black;
                    }
                    renderer.gameObject.SetActive(false);
                }
            })));
    }

    internal static List<T> ToSystemList<T>(this Il2CppSystem.Collections.Generic.List<T> iList)
    {
        return [.. iList];
    }

    internal static T Random<T>(this IList<T> self)
    {
        return self.Count > 0 ? self[RebuildUs.Rnd.Next(0, self.Count)] : default;
    }

    internal static void Shuffle<T>(this IList<T> self, int startAt = 0)
    {
        for (var i = startAt; i < self.Count - 1; i++)
        {
            var index = RebuildUs.Rnd.Next(i, self.Count);
            (self[index], self[i]) = (self[i], self[index]);
        }
    }

    internal static PlainShipRoom GetPlainShipRoom(PlayerControl p)
    {
        var buffer = new Collider2D[10];
        ContactFilter2D filter = new()
        {
            layerMask = Constants.PlayersOnlyMask,
            useLayerMask = true,
            useTriggers = false,
        };
        var rooms = MapUtilities.CachedShipStatus?.AllRooms;
        if (rooms == null)
        {
            return null;
        }

        foreach (var room in rooms)
        {
            if (room.roomArea == null)
            {
                continue;
            }
            var hits = room.roomArea.OverlapCollider(filter, buffer);
            for (var i = 0; i < hits; i++)
            {
                if (buffer[i]?.gameObject == p.gameObject)
                {
                    return room;
                }
            }
        }

        return null;
    }

    internal static bool IsOnElecTask()
    {
        return Camera.main.gameObject.GetComponentInChildren<SwitchMinigame>() != null;
    }

    internal static int Get(this Int32OptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetInt(opt);
    }

    internal static int[] Get(this Int32ArrayOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetIntArray(opt);
    }

    internal static float Get(this FloatOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetFloat(opt);
    }

    internal static float[] Get(this FloatArrayOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetFloatArray(opt);
    }

    internal static bool Get(this BoolOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetBool(opt);
    }

    internal static byte Get(this ByteOptionNames opt)
    {
        return GameOptionsManager.Instance.CurrentGameOptions.GetByte(opt);
    }

    internal static void Set(this Int32OptionNames opt, int value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetInt(opt, value);
    }

    internal static void Set(this FloatOptionNames opt, float value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetFloat(opt, value);
    }

    internal static void Set(this BoolOptionNames opt, bool value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetBool(opt, value);
    }

    internal static void Set(this ByteOptionNames opt, byte value)
    {
        GameOptionsManager.Instance.CurrentGameOptions.SetByte(opt, value);
    }

    internal static Texture2D LoadTextureFromDisk(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                Texture2D texture = new(2, 2, TextureFormat.ARGB32, true);
                var bytes = Il2CppSystem.IO.File.ReadAllBytes(path);
                texture.LoadImage(bytes, false);
                return texture;
            }
        }
        catch (Exception e)
        {
            Logger.LogError("Error loading texture from disk (" + path + "): " + e.Message);
        }

        return null;
    }

    internal static RoleTypes GetOptionIcon(this CustomOption option)
    {
        return option.Type switch
        {
            CustomOptionType.General => RoleTypes.Crewmate,
            CustomOptionType.Crewmate => RoleTypes.Scientist,
            CustomOptionType.Impostor => RoleTypes.Shapeshifter,
            CustomOptionType.Neutral => RoleTypes.Noisemaker,
            CustomOptionType.Modifier => RoleTypes.GuardianAngel,
            _ => RoleTypes.Crewmate,
        };
    }

    internal static bool IsBlocked(PlayerTask task, PlayerControl pc)
    {
        if (task == null || pc == null || pc != PlayerControl.LocalPlayer)
        {
            return false;
        }

        var taskType = task.TaskType;
        var isLights = taskType == TaskTypes.FixLights;
        var isComms = taskType == TaskTypes.FixComms;
        var isReactor = taskType is TaskTypes.StopCharles or TaskTypes.ResetSeismic or TaskTypes.ResetReactor;
        var isO2 = taskType == TaskTypes.RestoreOxy;

        if (pc.IsRole(RoleType.NiceSwapper) && (isLights || isComms))
        {
            return true;
        }

        if (pc.HasModifier(ModifierType.Madmate) && (isLights || isComms && !Madmate.CanFixComm))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Madmate) && (isLights || isComms && !MadmateRole.CanFixComm))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Suicider) && (isLights || isComms && !Suicider.CanFixComm))
        {
            return true;
        }

        if (pc.HasModifier(ModifierType.CreatedMadmate) && (isLights || isComms && !CreatedMadmate.CanFixComm))
        {
            return true;
        }

        if (pc.IsGm() && (isLights || isComms || isReactor || isO2))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Mafioso) && !Mafia.Mafioso.CanRepair && (isLights || isComms))
        {
            return true;
        }

        if (pc.IsRole(RoleType.Janitor) && !Mafia.Janitor.CanRepair && (isLights || isComms))
        {
            return true;
        }

        return false;
    }

    internal static bool IsBlocked(Console console, PlayerControl pc)
    {
        if (console == null || pc == null || pc != PlayerControl.LocalPlayer)
        {
            return false;
        }

        var task = console.FindTask(pc);
        return IsBlocked(task, pc);
    }

    internal static bool IsBlocked(SystemConsole console, PlayerControl pc)
    {
        if (console == null || pc == null || pc != PlayerControl.LocalPlayer)
        {
            return false;
        }

        var name = console.name;
        var isSecurity = name is "task_cams" or "Surv_Panel" or "SurvLogConsole" or "SurvConsole";
        var isVitals = name == "panel_vitals";

        return isSecurity && !MapSettings.CanUseCameras || isVitals && !MapSettings.CanUseVitals;
    }

    internal static bool IsBlocked(IUsable target, PlayerControl pc)
    {
        if (target == null)
        {
            return false;
        }

        var targetConsole = target.TryCast<Console>();
        if (targetConsole != null)
        {
            return IsBlocked(targetConsole, pc);
        }

        var targetSysConsole = target.TryCast<SystemConsole>();
        if (targetSysConsole != null)
        {
            return IsBlocked(targetSysConsole, pc);
        }

        var targetMapConsole = target.TryCast<MapConsole>();
        if (targetMapConsole != null)
        {
            return !MapSettings.CanUseAdmin;
        }

        return false;
    }
}