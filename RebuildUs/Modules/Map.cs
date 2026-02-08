namespace RebuildUs.Modules;

public static class Map
{
    public static Dictionary<byte, SpriteRenderer> MapIcons = null;
    public static Dictionary<byte, SpriteRenderer> CorpseIcons = null;

    public static Dictionary<PlainDoor, SpriteRenderer> DoorMarks;
    public static Il2CppArrayBase<PlainDoor> PlainDoors;
    private static Vector3 UseButtonPos;

    public static SpriteRenderer TargetHerePoint;
    public static Dictionary<byte, SpriteRenderer> ImpostorHerePoint;

    private static float _lastBodySearchTime;
    private static DeadBody[] _cachedBodies;

    public static void Reset()
    {
        if (MapIcons != null)
        {
            foreach (SpriteRenderer r in MapIcons.Values)
                if (r != null && r.gameObject != null) UnityEngine.Object.Destroy(r.gameObject);
            MapIcons.Clear();
            MapIcons = null;
        }

        if (CorpseIcons != null)
        {
            foreach (SpriteRenderer r in CorpseIcons.Values)
                if (r != null && r.gameObject != null) UnityEngine.Object.Destroy(r.gameObject);
            CorpseIcons.Clear();
            CorpseIcons = null;
        }

        if (TargetHerePoint != null)
        {
            UnityEngine.Object.Destroy(TargetHerePoint.gameObject);
        }

        if (ImpostorHerePoint != null)
        {
            foreach (SpriteRenderer r in ImpostorHerePoint.Values)
            {
                if (r != null && r.gameObject != null) UnityEngine.Object.Destroy(r.gameObject);
            }
            ImpostorHerePoint.Clear();
            ImpostorHerePoint = null;
        }
        if (DoorMarks != null)
        {
            foreach (var mark in DoorMarks.Values)
            {
                if (mark != null && mark.gameObject != null) UnityEngine.Object.Destroy(mark.gameObject);
            }
            DoorMarks.Clear();
            DoorMarks = null;
        }
        if (PlainDoors != null)
        {
            PlainDoors = null;
        }
    }

    static void InitializeIcons(MapBehaviour __instance, PlayerControl pc = null)
    {
        if (!__instance.HerePoint || !__instance.HerePoint.transform.parent) return;

        List<PlayerControl> players = [];
        if (pc == null)
        {
            MapIcons = [];
            CorpseIcons = [];
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                players.Add(p);
            }
        }
        else
        {
            players.Add(pc);
        }

        foreach (PlayerControl p in players)
        {
            if (!p || p.IsGM()) continue;

            byte id = p.PlayerId;
            MapIcons[id] = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
            p.SetPlayerMaterialColors(MapIcons[id]);

            CorpseIcons[id] = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
            CorpseIcons[id].sprite = AssetLoader.CorpseIcon;
            CorpseIcons[id].transform.localScale = Vector3.one * 0.20f;
            p.SetPlayerMaterialColors(CorpseIcons[id]);
        }
    }

    public static void UpdatePrefix(MapBehaviour __instance)
    {
        if (!MapUtilities.CachedShipStatus || !__instance.HerePoint) return;
        var vector = AntiTeleport.Position;

        float mapScale = MapUtilities.CachedShipStatus.MapScale;
        float flipX = Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);

        vector.x /= mapScale;
        vector.y /= mapScale;
        vector.x *= flipX;
        vector.z = -1f;

        if (__instance.HerePoint.transform.localPosition != vector)
        {
            __instance.HerePoint.transform.localPosition = vector;
        }

        PlayerControl.LocalPlayer.SetPlayerMaterialColors(__instance.HerePoint);
    }

    public static void UpdatePostfix(MapBehaviour __instance)
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (EvilTracker.Exists && localPlayer.IsRole(RoleType.EvilTracker) && EvilTracker.CanSeeTargetPosition)
        {
            EvilTrackerFixedUpdate(__instance);
        }

        if (EvilHacker.Exists && localPlayer.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited())
        {
            EvilHackerFixedUpdate(__instance);
        }

        if (localPlayer.IsGM())
        {
            var shipStatus = MapUtilities.CachedShipStatus;
            if (shipStatus != null)
            {
                float mapScale = shipStatus.MapScale;
                float flipX = Mathf.Sign(shipStatus.transform.localScale.x);

                var allPlayers = PlayerControl.AllPlayerControls;
                foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (player == null || player.IsGM()) continue;

                    byte id = player.PlayerId;
                    if (MapIcons == null || !MapIcons.TryGetValue(id, out var icon))
                    {
                        continue;
                    }

                    bool enabled = !player.Data.IsDead;
                    if (enabled)
                    {
                        Vector3 vector = player.transform.position;
                        vector.x /= mapScale;
                        vector.y /= mapScale;
                        vector.x *= flipX;
                        vector.z = -1f;
                        icon.transform.localPosition = vector;
                    }

                    if (icon.enabled != enabled) icon.enabled = enabled;
                }

                if (CorpseIcons != null)
                {
                    foreach (var r in CorpseIcons.Values) { r.enabled = false; }
                }

                // 死体検索を 0.5秒おきに制限して軽量化
                if (Time.time - _lastBodySearchTime > 0.5f)
                {
                    _lastBodySearchTime = Time.time;
                    _cachedBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                }

                if (_cachedBodies != null && CorpseIcons != null)
                {
                    for (int i = 0; i < _cachedBodies.Length; i++)
                    {
                        var b = _cachedBodies[i];
                        if (b == null) continue;

                        byte id = b.ParentId;
                        if (!CorpseIcons.TryGetValue(id, out var corpseIcon))
                        {
                            continue;
                        }

                        Vector3 vector = b.transform.position;
                        vector.x /= mapScale;
                        vector.y /= mapScale;
                        vector.x *= flipX;
                        vector.z = -1f;

                        corpseIcon.transform.localPosition = vector;
                        if (!corpseIcon.enabled) corpseIcon.enabled = true;
                    }
                }
            }
        }
    }

    public static bool ShowNormalMap(MapBehaviour __instance)
    {
        if (PlayerControl.LocalPlayer.IsTeamImpostor())
        {
            if (__instance.HerePoint && __instance.HerePoint.transform.parent)
            {
                Vector3 pos = __instance.HerePoint.transform.parent.transform.position;
                __instance.HerePoint.transform.parent.transform.position = new Vector3(pos.x, pos.y, -60f);
            }
            ChangeSabotageLayout(__instance);
            if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited()) return EvilHackerShowMap(__instance);
            if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker)) return EvilTrackerShowMap(__instance);
        }
        PlayerControl.LocalPlayer.SetPlayerMaterialColors(__instance.HerePoint);
        __instance.GenericShow();
        if (__instance.taskOverlay) __instance.taskOverlay.Show();
        if (__instance.ColorControl) __instance.ColorControl.SetColor(new Color(0.05f, 0.2f, 1f, 1f));

        var hm = FastDestroyableSingleton<HudManager>.Instance;
        if (hm) hm.SetHudActive(false);

        return false;
    }

    public static void GenericShowPrefix(MapBehaviour __instance)
    {
        if (PlayerControl.LocalPlayer.IsGM())
        {
            var hm = FastDestroyableSingleton<HudManager>.Instance;
            if (hm != null && hm.UseButton != null)
            {
                UseButtonPos = hm.UseButton.transform.localPosition;
            }
        }
        CustomOverlays.HideInfoOverlay();
    }

    public static void GenericShowPostfix(MapBehaviour __instance)
    {
        if (PlayerControl.LocalPlayer.IsGM())
        {
            if (MapIcons == null || CorpseIcons == null)
            {
                InitializeIcons(__instance);
            }

            if (__instance.taskOverlay) __instance.taskOverlay.Hide();
            if (MapIcons != null)
            {
                foreach (var id in MapIcons.Keys)
                {
                    var p = Helpers.PlayerById(id);
                    if (p != null && p.Data != null)
                    {
                        p.SetPlayerMaterialColors(MapIcons[id]);
                        MapIcons[id].enabled = !p.Data.IsDead;
                    }
                }
            }

            if (CorpseIcons != null)
            {
                foreach (var b in UnityEngine.Object.FindObjectsOfType<DeadBody>())
                {
                    byte id = b.ParentId;
                    var p = Helpers.PlayerById(id);
                    if (p != null)
                    {
                        p.SetPlayerMaterialColors(CorpseIcons[id]);
                        CorpseIcons[id].enabled = true;
                    }
                }
            }
        }
    }

    public static void Close(MapBehaviour __instance)
    {
        var hm = FastDestroyableSingleton<HudManager>.Instance;
        if (PlayerControl.LocalPlayer.IsGM())
        {
            if (hm != null && hm.UseButton != null)
            {
                hm.UseButton.transform.localPosition = UseButtonPos;
            }
        }
        if (hm != null && hm.transform != null)
        {
            var taskDisplay = hm.transform.FindChild("TaskDisplay");
            if (taskDisplay != null)
            {
                var taskPanel = taskDisplay.FindChild("TaskPanel");
                taskPanel?.gameObject.SetActive(true);
            }
        }
    }

    public static bool IsOpenStopped(ref bool __result, MapBehaviour __instance)
    {
        if ((PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited()) && CustomOptionHolder.EvilHackerCanMoveEvenIfUsesAdmin.GetBool())
        {
            __result = false;
            return false;
        }
        return true;
    }

    public static bool ShowSabotageMapPrefix(MapBehaviour __instance)
    {
        // サボタージュマップを改変したくない人向け設定
        if (RebuildUs.ForceNormalSabotageMap.Value) return true;

        if (__instance.HerePoint && __instance.HerePoint.transform.parent)
        {
            Vector3 pos = __instance.HerePoint.transform.parent.transform.position;
            __instance.HerePoint.transform.parent.transform.position = new Vector3(pos.x, pos.y, -60f);
        }
        ChangeSabotageLayout(__instance);
        if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited()) return EvilHackerShowMap(__instance);
        if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker)) return EvilTrackerShowMap(__instance);
        return true;
    }

    public static void ShowSabotageMapPostfix(MapBehaviour __instance)
    {
        if (RebuildUs.HideFakeTasks.Value && __instance.taskOverlay)
        {
            __instance.taskOverlay.Hide();
        }
    }

    private static void ShowDoorStatus(MapBehaviour __instance)
    {
        if (!EvilHacker.CanSeeDoorStatus || !__instance.HerePoint || !__instance.HerePoint.transform.parent) return;
        if (MeetingHud.Instance == null && RebuildUs.ForceNormalSabotageMap.Value)
        {
            if (DoorMarks != null)
            {
                foreach (var mark in DoorMarks.Values)
                {
                    if (mark.gameObject.activeSelf) mark.gameObject.SetActive(false);
                }
            }
            return;
        }

        if (DoorMarks == null) DoorMarks = [];
        if (PlainDoors == null) return;

        foreach (var door in PlainDoors)
        {
            if (door == null) continue;
            if (!DoorMarks.TryGetValue(door, out var mark))
            {
                mark = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                DoorMarks.Add(door, mark);
            }

            if (!door.Open)
            {
                if (!mark.gameObject.activeSelf) mark.gameObject.SetActive(true);
                mark.sprite = AssetLoader.Cross;
                PlayerMaterial.SetColors(0, mark);
                var shipStatus = MapUtilities.CachedShipStatus;
                if (shipStatus == null) return;
                var pos = door.gameObject.transform.position / shipStatus.MapScale;
                pos.x *= Mathf.Sign(shipStatus.transform.localScale.x);
                pos.z = -10f;
                mark.transform.localPosition = pos;
            }
            else
            {
                if (mark.gameObject.activeSelf) mark.gameObject.SetActive(false);
            }
        }
    }

    private static void ChangeSabotageLayout(MapBehaviour __instance)
    {
        if (Helpers.IsAirship && __instance.infectedOverlay)
        {
            // サボタージュアイコンのレイアウトを変更
            var halfScale = new Vector3(0.75f, 0.75f, 0.75f);
            var originalScale = new Vector3(1f, 1f, 1f);
            var scale = RebuildUs.BetterSabotageMap.Value ? halfScale : originalScale;
            var comms = __instance.infectedOverlay.transform.FindChild("Comms");
            var electrical = __instance.infectedOverlay.transform.FindChild("Electrical");
            var mainHall = __instance.infectedOverlay.transform.FindChild("MainHall");
            var gapRoom = __instance.infectedOverlay.transform.FindChild("Gap Room");
            var records = __instance.infectedOverlay.transform.FindChild("Records");
            var brig = __instance.infectedOverlay.transform.FindChild("Brig");
            var kitchen = __instance.infectedOverlay.transform.FindChild("Kitchen");
            var medbay = __instance.infectedOverlay.transform.FindChild("Medbay");

            if (comms) comms.localScale = scale;
            if (electrical) electrical.localScale = scale;
            if (mainHall) mainHall.localScale = scale;
            if (gapRoom) gapRoom.localScale = scale;
            if (records) records.localScale = scale;
            if (brig) brig.localScale = scale;
            if (kitchen) kitchen.localScale = scale;
            if (medbay) medbay.localScale = scale;

            if (RebuildUs.BetterSabotageMap.Value)
            {
                if (comms)
                {
                    var bomb = comms.FindChild("bomb");
                    if (bomb) bomb.localPosition = new Vector3(-0.1f, 0.9f, -1f);
                    var doors = comms.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0.5f, 0.45f, -1f);
                }
                if (electrical)
                {
                    var lightsOut = electrical.FindChild("lightsOut");
                    if (lightsOut) lightsOut.localPosition = new Vector3(0f, -0.6f, -1f);
                }
                if (mainHall)
                {
                    var doors = mainHall.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(-0.18f, -0.35f, -1f);
                }
                if (gapRoom)
                {
                    var meltdown = gapRoom.FindChild("meltdown");
                    if (meltdown) meltdown.localPosition = new Vector3(-0.34f, 0f, -1f);
                }
                if (records)
                {
                    var doors = records.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0.01f, 1.2f, -1f);
                }
                if (brig)
                {
                    var doors = brig.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0f, 0.9f, -1f);
                }
                if (kitchen)
                {
                    var doors = kitchen.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0.1f, 0.9f, -1f);
                }
                if (medbay)
                {
                    var doors = medbay.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0.2f, 0f, -1f);
                }
            }
            else
            {
                if (comms)
                {
                    var bomb = comms.FindChild("bomb");
                    if (bomb) bomb.localPosition = new Vector3(-0.3f, 0f, -0.5f);
                    var doors = comms.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0.3f, 0f, -0.5f);
                }
                if (electrical)
                {
                    var lightsOut = electrical.FindChild("lightsOut");
                    if (lightsOut) lightsOut.localPosition = new Vector3(0f, 0f, -0.5f);
                }
                if (mainHall)
                {
                    var doors = mainHall.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0f, 0f, -0.5f);
                }
                if (gapRoom)
                {
                    var meltdown = gapRoom.FindChild("meltdown");
                    if (meltdown) meltdown.localPosition = new Vector3(0f, 0f, -0.5f);
                }
                if (records)
                {
                    var doors = records.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0f, 0f, -0.5f);
                }
                if (brig)
                {
                    var doors = brig.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0f, 0f, -0.5f);
                }
                if (kitchen)
                {
                    var doors = kitchen.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0f, 0f, -0.5f);
                }
                if (medbay)
                {
                    var doors = medbay.FindChild("Doors");
                    if (doors) doors.localPosition = new Vector3(0f, 0f, -0.5f);
                }
            }
        }
    }

    private static void EvilHackerFixedUpdate(MapBehaviour __instance)
    {
        ShowDoorStatus(__instance);
    }

    private static void EvilTrackerFixedUpdate(MapBehaviour __instance)
    {
        var shipStatus = MapUtilities.CachedShipStatus;
        if (!shipStatus) return;

        // ターゲットの位置をマップに表示
        if (EvilTracker.Target != null)
        {
            if (TargetHerePoint == null && __instance.HerePoint && __instance.HerePoint.transform.parent)
            {
                TargetHerePoint = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
            }
            if (TargetHerePoint != null)
            {
                bool isAlive = EvilTracker.Target.IsAlive();
                if (TargetHerePoint.gameObject.activeSelf != isAlive) TargetHerePoint.gameObject.SetActive(isAlive);
                NetworkedPlayerInfo PlayerById = GameData.Instance.GetPlayerById(EvilTracker.Target.PlayerId);
                PlayerControl.LocalPlayer.SetPlayerMaterialColors(TargetHerePoint);
                var pos = EvilTracker.Target.transform.position;
                pos.x /= shipStatus.MapScale;
                pos.y /= shipStatus.MapScale;
                pos.x *= Mathf.Sign(shipStatus.transform.localScale.x);
                pos.z = -10;
                TargetHerePoint.transform.localPosition = pos;
            }
        }

        // インポスターの位置をマップに表示
        if (ImpostorHerePoint == null) ImpostorHerePoint = [];
        var localPlayer = PlayerControl.LocalPlayer;
        float mapScale = shipStatus.MapScale;
        float flipX = Mathf.Sign(shipStatus.transform.localScale.x);

        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.IsTeamImpostor() && p != localPlayer)
            {
                if (!ImpostorHerePoint.TryGetValue(p.PlayerId, out var icon))
                {
                    if (__instance.HerePoint && __instance.HerePoint.transform.parent)
                    {
                        icon = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                        ImpostorHerePoint[p.PlayerId] = icon;
                    }
                }
                if (icon != null)
                {
                    bool isAlive = p.IsAlive();
                    if (icon.gameObject.activeSelf != isAlive) icon.gameObject.SetActive(isAlive);
                    PlayerMaterial.SetColors(0, icon);
                    var pos = p.transform.position;
                    pos.x /= mapScale;
                    pos.y /= mapScale;
                    pos.x *= flipX;
                    pos.z = -10;
                    icon.transform.localPosition = pos;
                }
            }
        }
    }
    private static bool EvilTrackerShowMap(MapBehaviour __instance)
    {
        // if (MeetingHud.Instance) return true;
        if (__instance.IsOpen)
        {
            __instance.Close();
            return false;
        }
        // if (!PlayerControl.LocalPlayer.CanMove)
        // {
        //     return false;
        // }
        __instance.specialInputHandler?.disableVirtualCursor = true;
        PlayerControl.LocalPlayer.SetPlayerMaterialColors(__instance.HerePoint);
        __instance.GenericShow();
        __instance.gameObject.SetActive(true);
        if (__instance.infectedOverlay) __instance.infectedOverlay.gameObject.SetActive(MeetingHud.Instance ? false : true);
        if (RebuildUs.HideFakeTasks.Value && !(PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker) && EvilTracker.Target != null))
        {
            if (__instance.taskOverlay) __instance.taskOverlay.Hide();
        }
        else
        {
            if (__instance.taskOverlay) __instance.taskOverlay.Show();
        }
        if (__instance.ColorControl) __instance.ColorControl.SetColor(Palette.ImpostorRed);

        var hm = FastDestroyableSingleton<HudManager>.Instance;
        if (hm) hm.SetHudActive(false);

        ConsoleJoystick.SetMode_Sabotage();

        return false;
    }
    private static bool EvilHackerShowMap(MapBehaviour __instance)
    {
        // if (MeetingHud.Instance) return true;
        if (__instance.IsOpen)
        {
            __instance.Close();
            return false;
        }
        // if (!PlayerControl.LocalPlayer.CanMove)
        // {
        //     return false;
        // }
        __instance.specialInputHandler?.disableVirtualCursor = true;
        PlainDoors = UnityEngine.Object.FindObjectsOfType<PlainDoor>();
        PlayerControl.LocalPlayer.SetPlayerMaterialColors(__instance.HerePoint);
        __instance.GenericShow();
        __instance.gameObject.SetActive(true);
        Admin.IsEvilHackerAdmin = true;
        if (__instance.countOverlay) __instance.countOverlay.gameObject.SetActive(true);
        if (__instance.infectedOverlay) __instance.infectedOverlay.gameObject.SetActive(MeetingHud.Instance ? false : true);
        if (MeetingHud.Instance != null && PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker) && EvilTracker.CanSeeTargetTask)
        {
            if (__instance.taskOverlay) __instance.taskOverlay.Show();
        }
        else if (RebuildUs.HideFakeTasks.Value)
        {
            if (__instance.taskOverlay) __instance.taskOverlay.Hide();
        }
        else
        {
            if (__instance.taskOverlay) __instance.taskOverlay.Show();
        }
        if (__instance.ColorControl) __instance.ColorControl.SetColor(Palette.ImpostorRed);

        var hm = FastDestroyableSingleton<HudManager>.Instance;
        if (hm) hm.SetHudActive(false);

        ConsoleJoystick.SetMode_Sabotage();

        return false;
    }

    public static Dictionary<byte, Il2CppSystem.Collections.Generic.List<Vector2>> RealTasks = [];
    public static void ResetRealTasks()
    {
        RealTasks.Clear();
    }
    public static void ShareRealTasks()
    {
        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShareRealTasks);
        int count = 0;
        foreach (var task in PlayerControl.LocalPlayer.myTasks)
        {
            if (!task.IsComplete && task.HasLocation && !PlayerTask.TaskIsEmergency(task))
            {
                foreach (var loc in task.Locations)
                {
                    count++;
                }
            }
        }
        sender.Write((byte)count);
        foreach (var task in PlayerControl.LocalPlayer.myTasks)
        {
            if (!task.IsComplete && task.HasLocation && !PlayerTask.TaskIsEmergency(task))
            {
                foreach (var loc in task.Locations)
                {
                    sender.Write(PlayerControl.LocalPlayer.PlayerId);
                    sender.Write(loc.x);
                    sender.Write(loc.y);
                }
            }
        }
    }

    public static bool ShowOverlay(MapTaskOverlay __instance)
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker))
        {
            return EvilTrackerShowTask(__instance);
        }
        return true;
    }

    private static bool EvilTrackerShowTask(MapTaskOverlay __instance)
    {
        if (!MeetingHud.Instance) return true;  // Only run in meetings, and then set the Position of the HerePoint to the Position before the Meeting!
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker) || !CustomOptionHolder.EvilTrackerCanSeeTargetTask.GetBool()) return true;
        if (EvilTracker.Target == null) return true;
        if (!RealTasks.ContainsKey(EvilTracker.Target.PlayerId) || RealTasks[EvilTracker.Target.PlayerId] == null) return false;

        var shipStatus = MapUtilities.CachedShipStatus;
        if (!shipStatus) return true;

        __instance.gameObject.SetActive(true);
        __instance.data.Clear();
        for (int i = 0; i < RealTasks[EvilTracker.Target.PlayerId].Count; i++)
        {
            try
            {
                Vector2 pos = RealTasks[EvilTracker.Target.PlayerId][i];

                Vector3 localPosition = pos / shipStatus.MapScale;
                localPosition.z = -1f;
                PooledMapIcon pooledMapIcon = __instance.icons.Get<PooledMapIcon>();
                pooledMapIcon.transform.localScale = new Vector3(pooledMapIcon.NormalSize, pooledMapIcon.NormalSize, pooledMapIcon.NormalSize);
                pooledMapIcon.rend.color = Color.yellow;
                pooledMapIcon.name = $"{i}";
                pooledMapIcon.lastMapTaskStep = 0;
                pooledMapIcon.transform.localPosition = localPosition;
                string text = $"{i}";
                __instance.data.Add(text, pooledMapIcon);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
        }
        return false;
    }

    public static bool OverlayOnEnablePrefix(MapCountOverlay __instance)
    {
        if (CustomOptionHolder.ImpostorCanIgnoreCommSabotage.GetBool() && PlayerControl.LocalPlayer.IsTeamImpostor())
        {
            return false;
        }
        return true;
    }

    public static bool OverlayOnEnablePostfix(MapCountOverlay __instance)
    {
        if (CustomOptionHolder.ImpostorCanIgnoreCommSabotage.GetBool() && PlayerControl.LocalPlayer.IsTeamImpostor())
        {
            __instance.timer += Time.deltaTime;
            if (__instance.timer < 0.1f)
            {
                return false;
            }
            __instance.timer = 0f;
            for (int i = 0; i < __instance.CountAreas.Length; i++)
            {
                CounterArea counterArea = __instance.CountAreas[i];
                PlainShipRoom plainShipRoom;
                if (MapUtilities.CachedShipStatus.FastRooms.TryGetValue(counterArea.RoomType, out plainShipRoom) && plainShipRoom.roomArea)
                {
                    int num = plainShipRoom.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
                    HashSet<byte> countedPlayers = [];
                    int num2 = 0;
                    for (int j = 0; j < num; j++)
                    {
                        Collider2D collider2D = __instance.buffer[j];
                        if (!(collider2D.tag == "DeadBody"))
                        {
                            PlayerControl component = collider2D.GetComponent<PlayerControl>();
                            if (component && component.Data != null && !component.Data.Disconnected && !component.Data.IsDead)
                            {
                                if (countedPlayers.Add(component.PlayerId))
                                {
                                    num2++;
                                }
                            }
                        }
                    }
                    counterArea.UpdateCount(num2);
                }
                else
                {
                    Logger.LogWarn("Couldn't find counter for:" + counterArea.RoomType.ToString());
                }
            }
            return false;
        }
        return true;
    }

    public static void AlphaPulseUpdate(AlphaPulse __instance)
    {
        if (!RebuildUs.TransparentMap.Value) return;
        if (__instance.rend)
        {
            __instance.rend.color = new Color(__instance.rend.color.r, __instance.rend.color.g, __instance.rend.color.b, 0.2f);

        }
        if (__instance.mesh)
        {
            __instance.mesh.material.color = new Color(__instance.mesh.material.color.r, __instance.mesh.material.color.g, __instance.mesh.material.color.b, 0.2f);
        }
    }
}