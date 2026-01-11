using System.Reflection;
using RebuildUs.Players;
using RebuildUs.Roles.Neutral;
using RebuildUs.Utilities;

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
        foreach (var task in MapUtilities.CachedShipStatus.CommonTasks.OrderBy(x => RebuildUs.Instance.rnd.Next())) commonTasks.Add(task);

        var shortTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.ShortTasks.OrderBy(x => RebuildUs.Instance.rnd.Next())) shortTasks.Add(task);

        var longTasks = new Il2CppSystem.Collections.Generic.List<NormalPlayerTask>();
        foreach (var task in MapUtilities.CachedShipStatus.LongTasks.OrderBy(x => RebuildUs.Instance.rnd.Next())) longTasks.Add(task);

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
        || ((player.IsRole(ERoleType.Jackal) || Jackal.formerJackals.Any(x => x.PlayerId == player.PlayerId)) && Jackal.hasImpostorVision)
        || (player.IsRole(ERoleType.Sidekick) && Sidekick.hasImpostorVision)
        || (player.IsRole(ERoleType.Jester) && Jester.hasImpostorVision);
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

    public static MurderAttemptResult checkMurderAttempt(PlayerControl killer, PlayerControl target, bool blockRewind = false, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false, bool ignoreMedic = false)
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
        if (!ignoreMedic && Medic.shielded != null && Medic.shielded == target)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.ShieldedMurderAttempt, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.shieldedMurderAttempt();
            return MurderAttemptResult.SuppressKill;
        }

        // Block impostor not fully grown mini kill
        else if (Mini.mini != null && target == Mini.mini && !Mini.isGrownUp())
        {
            return MurderAttemptResult.SuppressKill;
        }

        // Block Time Master with time shield kill
        else if (TimeMaster.shieldActive && TimeMaster.timeMaster != null && TimeMaster.timeMaster == target)
        {
            if (!blockRewind)
            {
                // Only rewind the attempt was not called because a meeting startet
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(killer.NetId, (byte)CustomRPC.TimeMasterRewindTime, Hazel.SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                RPCProcedure.timeMasterRewindTime();
            }
            return MurderAttemptResult.SuppressKill;
        }

        if (TransportationToolPatches.isUsingTransportation(target) && !blockRewind && killer == Vampire.vampire)
        {
            return MurderAttemptResult.DelayVampireKill;
        }
        else if (TransportationToolPatches.isUsingTransportation(target))
        {
            return MurderAttemptResult.SuppressKill;
        }
        return MurderAttemptResult.PerformKill;
    }

    public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool showAnimation)
    {
        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedMurderPlayer);
        sender.Write(killer.PlayerId);
        sender.Write(target.PlayerId);
        sender.Write(showAnimation ? byte.MaxValue : 0);
        RPCProcedure.uncheckedMurderPlayer(killer.PlayerId, target.PlayerId, showAnimation ? byte.MaxValue : (byte)0);
    }

    public static MurderAttemptResult checkMurderAttemptAndKill(PlayerControl killer, PlayerControl target, bool isMeetingStart = false, bool showAnimation = true, bool ignoreBlank = false, bool ignoreIfKillerIsDead = false)
    {
        // The local player checks for the validity of the kill and performs it afterwards (different to vanilla, where the host performs all the checks)
        // The kill attempt will be shared using a custom RPC, hence combining modded and unmodded versions is impossible
        var murder = checkMurderAttempt(killer, target, isMeetingStart, ignoreBlank, ignoreIfKillerIsDead);

        if (murder == MurderAttemptResult.PerformKill)
        {
            MurderPlayer(killer, target, showAnimation);
        }
        else if (murder == MurderAttemptResult.DelayVampireKill)
        {
            HudManager.Instance.StartCoroutine(Effects.Lerp(10f, new Action<float>((p) =>
            {
                if (!TransportationToolPatches.isUsingTransportation(target) && Vampire.bitten != null)
                {
                    MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VampireSetBitten, Hazel.SendOption.Reliable, -1);
                    writer.Write(byte.MaxValue);
                    writer.Write(byte.MaxValue);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    RPCProcedure.vampireSetBitten(byte.MaxValue, byte.MaxValue);
                    MurderPlayer(killer, target, showAnimation);
                }
            })));
        }
        return murder;
    }

    public static bool sabotageActive()
    {
        var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        return sabSystem.AnyActive;
    }

    public static float sabotageTimer()
    {
        var sabSystem = ShipStatus.Instance.Systems[SystemTypes.Sabotage].CastFast<SabotageSystemType>();
        return sabSystem.Timer;
    }
    public static bool canUseSabotage()
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
}