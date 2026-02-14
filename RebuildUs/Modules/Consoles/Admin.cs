namespace RebuildUs.Modules.Consoles;

public static class Admin
{
    static Dictionary<SystemTypes, List<Color>> PlayerColors = [];
    static float AdminTimer = 0f;
    static TextMeshPro OutOfTime;
    static TextMeshPro TimeRemaining;
    static bool ClearedIcons = false;
    public static bool IsEvilHackerAdmin = false;
    static GameObject Map;
    static GameObject NewMap;

    static PlainShipRoom Room;
    static readonly SystemTypes[] FilterCockpitAdmin = [SystemTypes.Cockpit, SystemTypes.Armory, SystemTypes.Kitchen, SystemTypes.VaultRoom, SystemTypes.Comms];
    static readonly SystemTypes[] FilterRecordsAdmin = [SystemTypes.Records, SystemTypes.Lounge, SystemTypes.CargoBay, SystemTypes.Showers, SystemTypes.Ventilation];

    private static bool FilterAdmin(SystemTypes type)
    {
        // イビルハッカーのアドミンは今まで通り
        var lp = PlayerControl.LocalPlayer;
        if (lp != null && (lp.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited())) return true;

        if (CustomOptionHolder.AirshipRestrictedAdmin.GetBool())
        {
            if (Room == null) return true;
            string roomName = Room.name;
            if (roomName == "Cockpit")
            {
                for (int i = 0; i < FilterCockpitAdmin.Length; i++)
                {
                    if (FilterCockpitAdmin[i] == type) return true;
                }
                return false;
            }
            if (roomName == "Records")
            {
                for (int i = 0; i < FilterRecordsAdmin.Length; i++)
                {
                    if (FilterRecordsAdmin[i] == type) return true;
                }
                return false;
            }
        }
        return true;
    }

    public static void ResetData()
    {
        AdminTimer = 0f;
        if (TimeRemaining != null)
        {
            UnityEngine.Object.Destroy(TimeRemaining.gameObject);
            TimeRemaining = null;
        }

        if (OutOfTime != null)
        {
            UnityEngine.Object.Destroy(OutOfTime.gameObject);
            OutOfTime = null;
        }
    }

    static void UseAdminTime()
    {
        // Don't waste network traffic if we're out of time.
        if (!IsEvilHackerAdmin)
        {
            if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictAdmin && MapSettings.RestrictAdminTime > 0f && PlayerControl.LocalPlayer.IsAlive())
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UseAdminTime);
                sender.Write(AdminTimer);
                RPCProcedure.UseAdminTime(AdminTimer);
            }
        }
        AdminTimer = 0f;
    }

    public static bool Update(MapCountOverlay __instance)
    {
        if (RebuildUs.activatedSensei && GameOptionsManager.Instance.currentGameOptions.MapId == 0 && !RebuildUs.updatedSenseiAdminmap) {
                    GameObject myAdminIcons = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/CountOverlay");
                    myAdminIcons.transform.GetChild(0).transform.position = myAdminIcons.transform.GetChild(0).transform.position + new Vector3(0, -0.2f, 0); // upper engine
                    myAdminIcons.transform.GetChild(1).transform.position = myAdminIcons.transform.GetChild(1).transform.position + new Vector3(0, 0.3f, 0); // lower engine
                    myAdminIcons.transform.GetChild(2).transform.position = myAdminIcons.transform.GetChild(2).transform.position + new Vector3(0.5f, 0, 0); // Reactor
                    myAdminIcons.transform.GetChild(3).transform.position = myAdminIcons.transform.GetChild(3).transform.position + new Vector3(1.6f, 2.3f, 0); // security
                    myAdminIcons.transform.GetChild(4).transform.position = myAdminIcons.transform.GetChild(4).transform.position + new Vector3(0.7f, -0.95f, 0); // medbey
                    myAdminIcons.transform.GetChild(5).transform.position = myAdminIcons.transform.GetChild(5).transform.position + new Vector3(0.5f, -1f, 0); // Cafeter�a
                    myAdminIcons.transform.GetChild(6).transform.position = myAdminIcons.transform.GetChild(6).transform.position + new Vector3(0.80f, -1, 0); // weapons
                    myAdminIcons.transform.GetChild(7).transform.position = myAdminIcons.transform.GetChild(7).transform.position + new Vector3(-1.5f, -2.6f, 0); // nav
                    myAdminIcons.transform.GetChild(8).transform.position = myAdminIcons.transform.GetChild(8).transform.position + new Vector3(0f, 1.5f, 0); // shields
                    myAdminIcons.transform.GetChild(9).transform.position = myAdminIcons.transform.GetChild(9).transform.position + new Vector3(0.9f, 3f, 0); // cooms
                    myAdminIcons.transform.GetChild(10).transform.position = myAdminIcons.transform.GetChild(10).transform.position + new Vector3(-1.7f, -0.3f, 0); // storage
                    myAdminIcons.transform.GetChild(11).transform.position = myAdminIcons.transform.GetChild(11).transform.position + new Vector3(0.20f, -0.5f, 0); // Admin
                    myAdminIcons.transform.GetChild(12).transform.position = myAdminIcons.transform.GetChild(12).transform.position + new Vector3(0.5f, -1.2f, 0); // elec
                    myAdminIcons.transform.GetChild(13).transform.position = myAdminIcons.transform.GetChild(13).transform.position + new Vector3(-2.9f, 0, 0); // o2
                    RebuildUs.updatedSenseiAdminmap = true;
                }

        AdminTimer += Time.deltaTime;
        if (AdminTimer > 0.1f)
            UseAdminTime();

        // Save colors for the Hacker
        __instance.timer += Time.deltaTime;
        if (__instance.timer < 0.1f)
        {
            return false;
        }
        __instance.timer = 0f;

        PlayerColors = [];

        if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictAdmin)
        {
            if (OutOfTime == null)
            {
                OutOfTime = UnityEngine.Object.Instantiate(__instance.SabotageText, __instance.SabotageText.transform.parent);
                OutOfTime.text = Tr.Get(TrKey.RestrictOutOfTime);
            }

            if (TimeRemaining == null)
            {
                TimeRemaining = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
                TimeRemaining.alignment = TMPro.TextAlignmentOptions.BottomRight;
                TimeRemaining.transform.position = Vector3.zero;
                TimeRemaining.transform.localPosition = new Vector3(3.25f, 5.25f);
                TimeRemaining.transform.localScale *= 2f;
                TimeRemaining.color = Palette.White;
            }

            if (MapSettings.RestrictAdminTime <= 0f && !PlayerControl.LocalPlayer.IsTeamImpostor())
            {
                __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                OutOfTime.gameObject.SetActive(true);
                TimeRemaining.gameObject.SetActive(false);
                if (ClearedIcons == false)
                {
                    foreach (CounterArea ca in __instance.CountAreas) ca.UpdateCount(0);
                    ClearedIcons = true;
                }
                return false;
            }

            ClearedIcons = false;
            OutOfTime.gameObject.SetActive(false);
            string timeString = TimeSpan.FromSeconds(MapSettings.RestrictAdminTime).ToString(@"mm\:ss\.ff");
            TimeRemaining.text = string.Format(Tr.Get(TrKey.TimeRemaining), timeString);
            //TimeRemaining.color = MapOptions.restrictAdminTime > 10f ? Palette.AcceptedGreen : Palette.ImpostorRed;
            TimeRemaining.gameObject.SetActive(true);
        }

        bool commsActive = false;
        foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
        {
            if (task.TaskType == TaskTypes.FixComms)
            {
                commsActive = true;
            }
        }
        if (CustomOptionHolder.ImpostorCanIgnoreCommSabotage.GetBool() && PlayerControl.LocalPlayer.IsTeamImpostor()) commsActive = false;

        if (!__instance.isSab && commsActive)
        {
            __instance.isSab = true;
            __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
            __instance.SabotageText.gameObject.SetActive(true);
            OutOfTime.gameObject.SetActive(false);
            return false;
        }

        if (__instance.isSab && !commsActive)
        {
            __instance.isSab = false;
            __instance.BackgroundColor.SetColor(Color.green);
            __instance.SabotageText.gameObject.SetActive(false);
            OutOfTime.gameObject.SetActive(false);
        }

        for (int i = 0; i < __instance.CountAreas.Length; i++)
        {
            CounterArea counterArea = __instance.CountAreas[i];
            List<Color> roomColors = [];
            PlayerColors.Add(counterArea.RoomType, roomColors);

            if (!commsActive && counterArea.RoomType > SystemTypes.Hallway)
            {
                PlainShipRoom plainShipRoom = MapUtilities.CachedShipStatus.FastRooms[counterArea.RoomType];

                if (plainShipRoom != null && plainShipRoom.roomArea)
                {
                    int num = plainShipRoom.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
                    HashSet<byte> countedPlayers = [];
                    HashSet<int> countedDeadBodies = [];
                    int num2 = 0;

                    // ロミジュリと絵画の部屋をアドミンの対象から外す
                    bool forceZero = false;
                    if (CustomOptionHolder.AirshipOldAdmin.GetBool() && (counterArea.RoomType is SystemTypes.Ventilation or SystemTypes.HallOfPortraits))
                    {
                        forceZero = true;
                    }

                    // アドミン毎に表示する範囲を制限する
                    if (!FilterAdmin(counterArea.RoomType))
                    {
                        forceZero = true;
                    }

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
                                    if (component?.cosmetics?.currentBodySprite?.BodySprite.material != null)
                                    {
                                        // Color color = component.myRend.material.GetColor("_BodyColor");
                                        Color color = Palette.PlayerColors[component.Data.DefaultOutfit.ColorId];
                                        if (Hacker.OnlyColorType)
                                        {
                                            var id = Mathf.Max(0, Palette.PlayerColors.IndexOf(color));
                                            color = Helpers.IsLighterColor((byte)id) ? Palette.PlayerColors[7] : Palette.PlayerColors[6];
                                        }
                                        roomColors.Add(color);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DeadBody component = collider2D.GetComponent<DeadBody>();
                            if (component && countedDeadBodies.Add(component.GetInstanceID()))
                            {
                                NetworkedPlayerInfo playerInfo = GameData.Instance.GetPlayerById(component.ParentId);
                                if (playerInfo != null)
                                {
                                    var color = Palette.PlayerColors[playerInfo.Object.CurrentOutfit.ColorId];
                                    if (Hacker.OnlyColorType)
                                    {
                                        color = Helpers.IsLighterColor(playerInfo.Object.CurrentOutfit.ColorId) ? Palette.PlayerColors[7] : Palette.PlayerColors[6];
                                    }
                                    roomColors.Add(color);
                                }
                            }
                        }
                    }
                    if (forceZero) num2 = 0;
                    if (num2 < 0) num2 = 0;
                    counterArea.UpdateCount(num2);
                }
                else
                {
                    Logger.LogWarn("Couldn't find counter for:" + counterArea.RoomType);
                }
            }
            else
            {
                counterArea.UpdateCount(0);
            }
        }
        return false;
    }

    public static void OnEnable(MapCountOverlay __instance)
    {
        AdminTimer = 0f;

        // 現在地からどのアドミンを使っているか特定する
        Room = Helpers.GetPlainShipRoom(PlayerControl.LocalPlayer);

        if (Room == null) return;

        // アドミンの画像を差し替える
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) && !EvilHacker.IsInherited() && Helpers.IsAirship && CustomOptionHolder.AirshipRestrictedAdmin.GetBool() && (Room.name is "Cockpit" or "Records"))
        {
            if (!Map)
            {
                Map = DestroyableSingleton<MapBehaviour>.Instance.gameObject.GetComponentsInChildren<SpriteRenderer>().FirstOrDefault(x => x.name == "Background").gameObject;
            }
            if (!NewMap) NewMap = UnityEngine.Object.Instantiate(Map, Map.transform.parent);

            SpriteRenderer renderer = NewMap.GetComponent<SpriteRenderer>();
            if (Room.name == "Cockpit")
            {
                renderer.sprite = AssetLoader.AdminCockpit;
                NewMap.transform.position = new Vector3(Map.transform.position.x + 0.5f, Map.transform.position.y, Map.transform.position.z - 0.1f);
            }
            if (Room.name == "Records")
            {
                renderer.sprite = AssetLoader.AdminRecords;
                NewMap.transform.position = new Vector3(Map.transform.position.x - 0.38f, Map.transform.position.y, Map.transform.position.z - 0.1f);
            }
            NewMap.SetActive(true);
        }
    }

    public static void OnDisable()
    {
        UseAdminTime();
        IsEvilHackerAdmin = false;
        if (NewMap) NewMap.SetActive(false);
    }

    private static Material DefaultMat;
    private static Material NewMat;
    public static void UpdateCount(CounterArea __instance)
    {
        // Hacker display saved colors on the admin panel
        bool showHackerInfo = PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && Hacker.HackerTimer > 0;
        if (PlayerColors.ContainsKey(__instance.RoomType))
        {
            List<Color> colors = PlayerColors[__instance.RoomType];
            List<Color> impostorColors = [];
            List<Color> mimicKColors = [];
            List<Color> deadBodyColors = [];
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                // var color = p.myRend.material.GetColor("_BodyColor");
                var color = Palette.PlayerColors[p.Data.DefaultOutfit.ColorId];
                if (p.IsTeamImpostor())
                {
                    impostorColors.Add(color);
                }
                else if (p.IsDead())
                {
                    deadBodyColors.Add(color);
                }
            }

            for (int i = 0; i < __instance.myIcons.Count; i++)
            {
                var icon = __instance.myIcons[i];
                var renderer = icon.GetComponent<SpriteRenderer>();

                if (renderer != null)
                {
                    if (DefaultMat == null) DefaultMat = renderer.material;
                    if (NewMat == null) NewMat = UnityEngine.Object.Instantiate(DefaultMat);
                    if (showHackerInfo && colors.Count > i)
                    {
                        renderer.material = NewMat;
                        var color = colors[i];
                        renderer.material.SetColor("_BodyColor", color);
                        var id = Palette.PlayerColors.IndexOf(color);
                        if (id < 0)
                        {
                            renderer.material.SetColor("_BackColor", color);
                        }
                        else
                        {
                            renderer.material.SetColor("_BackColor", Palette.ShadowColors[id]);
                        }
                        renderer.material.SetColor("_VisorColor", Palette.VisorColor);
                    }
                    else if ((PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited()) && EvilHacker.CanHasBetterAdmin)
                    {
                        renderer.material = NewMat;
                        var color = colors[i];
                        if (impostorColors.Contains(color))
                        {
                            if (mimicKColors.Contains(color))
                            {
                                color = Palette.PlayerColors[3];
                            }
                            else
                            {
                                color = Palette.ImpostorRed;
                            }
                            renderer.material.SetColor("_BodyColor", color);
                            var id = Palette.PlayerColors.IndexOf(color);
                            if (id < 0)
                            {
                                renderer.material.SetColor("_BackColor", color);
                            }
                            else
                            {
                                renderer.material.SetColor("_BackColor", Palette.ShadowColors[id]);
                            }
                            renderer.material.SetColor("_VisorColor", Palette.VisorColor);
                        }
                        else if (deadBodyColors.Contains(color))
                        {
                            color = Palette.Black;
                            renderer.material.SetColor("_BodyColor", color);
                            var id = Palette.PlayerColors.IndexOf(color);
                            if (id < 0)
                            {
                                renderer.material.SetColor("_BackColor", color);
                            }
                            else
                            {
                                renderer.material.SetColor("_BackColor", Palette.ShadowColors[id]);
                            }
                            renderer.material.SetColor("_VisorColor", Palette.VisorColor);
                        }
                        else
                        {
                            renderer.material = DefaultMat;
                        }
                    }
                    else
                    {
                        renderer.material = DefaultMat;
                    }
                }
            }
        }
    }
}