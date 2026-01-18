namespace RebuildUs.Modules;

public static class Map
{
    public static Dictionary<byte, SpriteRenderer> MapIcons = null;
    public static Dictionary<byte, SpriteRenderer> CorpseIcons = null;

    public static Dictionary<String, SpriteRenderer> DoorMarks;
    public static Il2CppArrayBase<PlainDoor> PlainDoors;
    private static Vector3 UseButtonPos;

    public static SpriteRenderer TargetHerePoint;
    public static Dictionary<byte, SpriteRenderer> ImpostorHerePoint;

    public static void Reset()
    {
        if (MapIcons != null)
        {
            foreach (SpriteRenderer r in MapIcons.Values)
                UnityEngine.Object.Destroy(r.gameObject);
            MapIcons.Clear();
            MapIcons = null;
        }

        if (CorpseIcons != null)
        {
            foreach (SpriteRenderer r in CorpseIcons.Values)
                UnityEngine.Object.Destroy(r.gameObject);
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
                UnityEngine.Object.Destroy(r.gameObject);
            }
            ImpostorHerePoint.Clear();
            ImpostorHerePoint = null;
        }
        if (DoorMarks != null)
        {
            foreach (var mark in DoorMarks.Values)
            {
                UnityEngine.Object.Destroy(mark.gameObject);
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
        List<PlayerControl> players = [];
        if (pc == null)
        {
            MapIcons = [];
            CorpseIcons = [];
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
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
            if (p.IsGM()) continue;

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
        var vector = AntiTeleport.Position;
        vector /= MapUtilities.CachedShipStatus.MapScale;
        vector.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
        vector.z = -1f;
        __instance.HerePoint.transform.localPosition = vector;
        PlayerControl.LocalPlayer.SetPlayerMaterialColors(__instance.HerePoint);
    }

    public static void UpdatePostfix(MapBehaviour __instance)
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker) && EvilTracker.CanSeeTargetPosition)
        {
            EvilTrackerFixedUpdate(__instance);
        }

        if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited())
        {
            EvilHackerFixedUpdate(__instance);
        }

        if (PlayerControl.LocalPlayer.IsGM())
        {
            foreach (PlayerControl p in CachedPlayer.AllPlayers)
            {
                if (p == null || p.IsGM()) continue;

                byte id = p.PlayerId;
                if (!MapIcons.ContainsKey(id))
                {
                    continue;
                }

                bool enabled = !p.Data.IsDead;
                if (enabled)
                {
                    Vector3 vector = p.transform.position;
                    vector /= MapUtilities.CachedShipStatus.MapScale;
                    vector.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
                    vector.z = -1f;
                    MapIcons[id].transform.localPosition = vector;

                }

                MapIcons[id].enabled = enabled;
            }

            foreach (SpriteRenderer r in CorpseIcons.Values) { r.enabled = false; }
            foreach (DeadBody b in UnityEngine.Object.FindObjectsOfType<DeadBody>())
            {
                byte id = b.ParentId;
                Vector3 vector = b.transform.position;
                vector /= MapUtilities.CachedShipStatus.MapScale;
                vector.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
                vector.z = -1f;

                if (!CorpseIcons.ContainsKey(id))
                {
                    continue;
                }

                CorpseIcons[id].transform.localPosition = vector;
                CorpseIcons[id].enabled = true;
            }
        }
    }

    public static bool ShowNormalMap(MapBehaviour __instance)
    {
        if (PlayerControl.LocalPlayer.IsTeamImpostor())
        {
            Vector3 pos = __instance.HerePoint.transform.parent.transform.position;
            __instance.HerePoint.transform.parent.transform.position = new Vector3(pos.x, pos.y, -60f);
            ChangeSabotageLayout(__instance);
            if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited()) return EvilHackerShowMap(__instance);
            if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker)) return EvilTrackerShowMap(__instance);
        }
        PlayerControl.LocalPlayer.SetPlayerMaterialColors(__instance.HerePoint);
        __instance.GenericShow();
        __instance.taskOverlay.Show();
        __instance.ColorControl.SetColor(new Color(0.05f, 0.2f, 1f, 1f));
        FastDestroyableSingleton<HudManager>.Instance.SetHudActive(false);

        return false;
    }

    public static void GenericShowPrefix(MapBehaviour __instance)
    {
        if (PlayerControl.LocalPlayer.IsGM())
        {
            UseButtonPos = FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition;
        }
        CustomOverlays.HideInfoOverlay();
        CustomOverlays.HideRoleOverlay();
    }

    public static void GenericShowPostfix(MapBehaviour __instance)
    {
        if (PlayerControl.LocalPlayer.IsGM())
        {
            if (MapIcons == null || CorpseIcons == null)
            {
                InitializeIcons(__instance);
            }

            __instance.taskOverlay.Hide();
            foreach (var id in MapIcons.Keys)
            {
                var p = Helpers.PlayerById(id);
                p.SetPlayerMaterialColors(MapIcons[id]);
                MapIcons[id].enabled = !p.Data.IsDead;
            }

            foreach (var b in UnityEngine.Object.FindObjectsOfType<DeadBody>())
            {
                byte id = b.ParentId;
                var p = Helpers.PlayerById(id);
                p.SetPlayerMaterialColors(CorpseIcons[id]);
                CorpseIcons[id].enabled = true;
            }
        }
    }

    public static void Close(MapBehaviour __instance)
    {
        if (PlayerControl.LocalPlayer.IsGM())
        {
            FastDestroyableSingleton<HudManager>.Instance.UseButton.transform.localPosition = UseButtonPos;
        }
        FastDestroyableSingleton<HudManager>.Instance.transform.FindChild("TaskDisplay").FindChild("TaskPanel").gameObject.SetActive(true);
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

        Vector3 pos = __instance.HerePoint.transform.parent.transform.position;
        __instance.HerePoint.transform.parent.transform.position = new Vector3(pos.x, pos.y, -60f);
        ChangeSabotageLayout(__instance);
        if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited()) return EvilHackerShowMap(__instance);
        if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker)) return EvilTrackerShowMap(__instance);
        return true;
    }

    public static void ShowSabotageMapPostfix(MapBehaviour __instance)
    {
        if (RebuildUs.HideFakeTasks.Value)
        {
            __instance.taskOverlay.Hide();
        }
    }

    private static void ShowDoorStatus(MapBehaviour __instance)
    {
        if (!EvilHacker.CanSeeDoorStatus) return;
        if (MeetingHud.Instance == null && RebuildUs.ForceNormalSabotageMap.Value)
        {
            foreach (var mark in DoorMarks.Values)
            {
                mark.gameObject.SetActive(false);
            }
            return;
        }

        // if (plainDoors == null) plainDoors = GameObject.FindObjectsOfType<PlainDoor>();
        if (DoorMarks == null) DoorMarks = [];

        foreach (var door in PlainDoors)
        {
            Vector3 pos = door.gameObject.transform.position / MapUtilities.CachedShipStatus.MapScale;
            pos.z = -10f;
            String key = $"{pos.x},{pos.y}";
            SpriteRenderer mark;
            if (DoorMarks.ContainsKey(key))
            {
                mark = DoorMarks[key];
            }
            else
            {
                mark = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                DoorMarks.Add(key, mark);
            }
            if (!door.Open)
            {
                mark.gameObject.SetActive(true);
                mark.sprite = AssetLoader.Cross;
                PlayerMaterial.SetColors(0, mark);
                mark.transform.localPosition = pos;
                mark.gameObject.SetActive(true);
            }
            else
            {
                mark.gameObject.SetActive(false);
            }
        }
    }

    private static void ChangeSabotageLayout(MapBehaviour __instance)
    {
        if (Helpers.IsAirship)
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

            comms.localScale = scale;
            electrical.localScale = scale;
            mainHall.localScale = scale;
            gapRoom.localScale = scale;
            records.localScale = scale;
            brig.localScale = scale;
            kitchen.localScale = scale;
            medbay.localScale = scale;

            if (RebuildUs.BetterSabotageMap.Value)
            {
                comms.FindChild("bomb").localPosition = new Vector3(-0.1f, 0.9f, -1f);
                comms.FindChild("Doors").localPosition = new Vector3(0.5f, 0.45f, -1f);
                electrical.FindChild("lightsOut").localPosition = new Vector3(0f, -0.6f, -1f);
                mainHall.FindChild("Doors").localPosition = new Vector3(-0.18f, -0.35f, -1f);
                gapRoom.FindChild("meltdown").localPosition = new Vector3(-0.34f, 0f, -1f);
                records.FindChild("Doors").localPosition = new Vector3(0.01f, 1.2f, -1f);
                brig.FindChild("Doors").localPosition = new Vector3(0f, 0.9f, -1f);
                kitchen.FindChild("Doors").localPosition = new Vector3(0.1f, 0.9f, -1f);
                medbay.FindChild("Doors").localPosition = new Vector3(0.2f, 0f, -1f);
            }
            else
            {
                comms.FindChild("bomb").localPosition = new Vector3(-0.3f, 0f, -0.5f);
                comms.FindChild("Doors").localPosition = new Vector3(0.3f, 0f, -0.5f);
                electrical.FindChild("lightsOut").localPosition = new Vector3(0f, 0f, -0.5f);
                mainHall.FindChild("Doors").localPosition = new Vector3(0f, 0f, -0.5f);
                gapRoom.FindChild("meltdown").localPosition = new Vector3(0f, 0f, -0.5f);
                records.FindChild("Doors").localPosition = new Vector3(0f, 0f, -0.5f);
                brig.FindChild("Doors").localPosition = new Vector3(0f, 0f, -0.5f);
                kitchen.FindChild("Doors").localPosition = new Vector3(0f, 0f, -0.5f);
                medbay.FindChild("Doors").localPosition = new Vector3(0f, 0f, -0.5f);
            }
        }
    }

    private static void EvilHackerFixedUpdate(MapBehaviour __instance)
    {
        ShowDoorStatus(__instance);
    }

    private static void EvilTrackerFixedUpdate(MapBehaviour __instance)
    {
        // ターゲットの位置をマップに表示
        if (EvilTracker.Target != null)
        {
            if (TargetHerePoint == null)
            {
                TargetHerePoint = UnityEngine.Object.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent);
            }
            TargetHerePoint.gameObject.SetActive(EvilTracker.Target.IsAlive());
            NetworkedPlayerInfo playerById = GameData.Instance.GetPlayerById(EvilTracker.Target.PlayerId);
            PlayerMaterial.SetColors((playerById != null) ? playerById.DefaultOutfit.ColorId : 0, TargetHerePoint);
            var pos = new Vector3(EvilTracker.Target.transform.position.x, EvilTracker.Target.transform.position.y, EvilTracker.Target.transform.position.z);
            pos /= MapUtilities.CachedShipStatus.MapScale;
            pos.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
            pos.z = -10;
            TargetHerePoint.transform.localPosition = pos;
        }

        // インポスターの位置をマップに表示
        if (ImpostorHerePoint == null) ImpostorHerePoint = [];
        foreach (PlayerControl p in CachedPlayer.AllPlayers)
        {
            if (p.IsTeamImpostor() && p != PlayerControl.LocalPlayer)
            {
                if (!ImpostorHerePoint.ContainsKey(p.PlayerId))
                {
                    ImpostorHerePoint[p.PlayerId] = GameObject.Instantiate<SpriteRenderer>(__instance.HerePoint, __instance.HerePoint.transform.parent);
                }
                ImpostorHerePoint[p.PlayerId].gameObject.SetActive(p.IsAlive());
                PlayerMaterial.SetColors(0, ImpostorHerePoint[p.PlayerId]);
                var pos = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z);
                pos /= MapUtilities.CachedShipStatus.MapScale;
                pos.x *= Mathf.Sign(MapUtilities.CachedShipStatus.transform.localScale.x);
                pos.z = -10;
                ImpostorHerePoint[p.PlayerId].transform.localPosition = pos;
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
        __instance.infectedOverlay.gameObject.SetActive(MeetingHud.Instance ? false : true);
        if (RebuildUs.HideFakeTasks.Value && !(PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker) && EvilTracker.Target != null))
        {
            __instance.taskOverlay.Hide();
        }
        else
        {
            __instance.taskOverlay.Show();
        }
        __instance.ColorControl.SetColor(Palette.ImpostorRed);
        FastDestroyableSingleton<HudManager>.Instance.SetHudActive(false);
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
        __instance.countOverlay.gameObject.SetActive(true);
        __instance.infectedOverlay.gameObject.SetActive(MeetingHud.Instance ? false : true);
        if (MeetingHud.Instance != null && PlayerControl.LocalPlayer.IsRole(RoleType.EvilTracker) && EvilTracker.CanSeeTargetTask)
        {
            __instance.taskOverlay.Show();
        }
        else if (RebuildUs.HideFakeTasks.Value)
        {
            __instance.taskOverlay.Hide();
        }
        else
        {
            __instance.taskOverlay.Show();
        }
        __instance.ColorControl.SetColor(Palette.ImpostorRed);
        FastDestroyableSingleton<HudManager>.Instance.SetHudActive(false);
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
        if (RealTasks[EvilTracker.Target.PlayerId] == null) return false;
        __instance.gameObject.SetActive(true);
        __instance.data.Clear();
        for (int i = 0; i < RealTasks[EvilTracker.Target.PlayerId].Count; i++)
        {
            try
            {
                Vector2 pos = RealTasks[EvilTracker.Target.PlayerId][i];

                Vector3 localPosition = pos / MapUtilities.CachedShipStatus.MapScale;
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
                if (ShipStatus.Instance.FastRooms.TryGetValue(counterArea.RoomType, out plainShipRoom) && plainShipRoom.roomArea)
                {
                    int num = plainShipRoom.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
                    int num2 = num;
                    for (int j = 0; j < num; j++)
                    {
                        Collider2D collider2D = __instance.buffer[j];
                        if (!(collider2D.tag == "DeadBody"))
                        {
                            PlayerControl component = collider2D.GetComponent<PlayerControl>();
                            if (!component || component.Data == null || component.Data.Disconnected || component.Data.IsDead)
                            {
                                num2--;
                            }
                        }
                    }
                    counterArea.UpdateCount(num2);
                }
                else
                {
                    Debug.LogWarning("Couldn't find counter for:" + counterArea.RoomType.ToString());
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