using Object = UnityEngine.Object;

namespace RebuildUs.Modules.Consoles;

public static class Admin
{
    private static Dictionary<SystemTypes, List<Color>> _playerColors = [];
    private static float _adminTimer;
    private static TextMeshPro _outOfTime;
    private static TextMeshPro _timeRemaining;
    private static bool _clearedIcons;
    public static bool IsEvilHackerAdmin;
    private static GameObject _map;
    private static GameObject _newMap;

    private static PlainShipRoom _room;

    private static readonly SystemTypes[] FILTER_COCKPIT_ADMIN = [SystemTypes.Cockpit, SystemTypes.Armory, SystemTypes.Kitchen, SystemTypes.VaultRoom, SystemTypes.Comms];

    private static readonly SystemTypes[] FILTER_RECORDS_ADMIN = [SystemTypes.Records, SystemTypes.Lounge, SystemTypes.CargoBay, SystemTypes.Showers, SystemTypes.Ventilation];

    private static Material _defaultMat;
    private static Material _newMat;

    private static bool FilterAdmin(SystemTypes type)
    {
        // イビルハッカーのアドミンは今まで通り
        var lp = PlayerControl.LocalPlayer;
        if (lp != null && (lp.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited())) return true;

        if (CustomOptionHolder.AirshipRestrictedAdmin.GetBool())
        {
            if (_room == null) return true;
            var roomName = _room.name;
            if (roomName == "Cockpit")
            {
                for (var i = 0; i < FILTER_COCKPIT_ADMIN.Length; i++)
                {
                    if (FILTER_COCKPIT_ADMIN[i] == type)
                        return true;
                }

                return false;
            }

            if (roomName == "Records")
            {
                for (var i = 0; i < FILTER_RECORDS_ADMIN.Length; i++)
                {
                    if (FILTER_RECORDS_ADMIN[i] == type)
                        return true;
                }

                return false;
            }
        }

        return true;
    }

    public static void ResetData()
    {
        _adminTimer = 0f;
        if (_timeRemaining != null)
        {
            Object.Destroy(_timeRemaining.gameObject);
            _timeRemaining = null;
        }

        if (_outOfTime != null)
        {
            Object.Destroy(_outOfTime.gameObject);
            _outOfTime = null;
        }
    }

    private static void UseAdminTime()
    {
        // Don't waste network traffic if we're out of time.
        if (!IsEvilHackerAdmin)
        {
            if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictAdmin && MapSettings.RestrictAdminTime > 0f && PlayerControl.LocalPlayer.IsAlive())
            {
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UseAdminTime);
                sender.Write(_adminTimer);
                RPCProcedure.UseAdminTime(_adminTimer);
            }
        }

        _adminTimer = 0f;
    }

    public static bool Update(MapCountOverlay __instance)
    {
        _adminTimer += Time.deltaTime;
        if (_adminTimer > 0.1f)
            UseAdminTime();

        // Save colors for the Hacker
        __instance.timer += Time.deltaTime;
        if (__instance.timer < 0.1f) return false;
        __instance.timer = 0f;

        _playerColors = [];

        if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictAdmin)
        {
            if (_outOfTime == null)
            {
                _outOfTime = Object.Instantiate(__instance.SabotageText, __instance.SabotageText.transform.parent);
                _outOfTime.text = Tr.Get(TrKey.RestrictOutOfTime);
            }

            if (_timeRemaining == null)
            {
                _timeRemaining = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
                _timeRemaining.alignment = TextAlignmentOptions.BottomRight;
                _timeRemaining.transform.position = Vector3.zero;
                _timeRemaining.transform.localPosition = new(3.25f, 5.25f);
                _timeRemaining.transform.localScale *= 2f;
                _timeRemaining.color = Palette.White;
            }

            if (MapSettings.RestrictAdminTime <= 0f && !PlayerControl.LocalPlayer.IsTeamImpostor())
            {
                __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                _outOfTime.gameObject.SetActive(true);
                _timeRemaining.gameObject.SetActive(false);
                if (!_clearedIcons)
                {
                    foreach (var ca in __instance.CountAreas) ca.UpdateCount(0);
                    _clearedIcons = true;
                }

                return false;
            }

            _clearedIcons = false;
            _outOfTime.gameObject.SetActive(false);
            var timeString = TimeSpan.FromSeconds(MapSettings.RestrictAdminTime).ToString(@"mm\:ss\.ff");
            _timeRemaining.text = string.Format(Tr.Get(TrKey.TimeRemaining), timeString);
            //TimeRemaining.color = MapOptions.restrictAdminTime > 10f ? Palette.AcceptedGreen : Palette.ImpostorRed;
            _timeRemaining.gameObject.SetActive(true);
        }

        var commsActive = false;
        foreach (var task in PlayerControl.LocalPlayer.myTasks.GetFastEnumerator())
        {
            if (task.TaskType == TaskTypes.FixComms)
                commsActive = true;
        }

        if (CustomOptionHolder.ImpostorCanIgnoreCommSabotage.GetBool() && PlayerControl.LocalPlayer.IsTeamImpostor())
            commsActive = false;

        if (!__instance.isSab && commsActive)
        {
            __instance.isSab = true;
            __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
            __instance.SabotageText.gameObject.SetActive(true);
            _outOfTime.gameObject.SetActive(false);
            return false;
        }

        if (__instance.isSab && !commsActive)
        {
            __instance.isSab = false;
            __instance.BackgroundColor.SetColor(Color.green);
            __instance.SabotageText.gameObject.SetActive(false);
            _outOfTime.gameObject.SetActive(false);
        }

        for (var i = 0; i < __instance.CountAreas.Length; i++)
        {
            var counterArea = __instance.CountAreas[i];
            List<Color> roomColors = [];
            _playerColors.Add(counterArea.RoomType, roomColors);

            if (!commsActive && counterArea.RoomType > SystemTypes.Hallway)
            {
                var plainShipRoom = MapUtilities.CachedShipStatus.FastRooms[counterArea.RoomType];

                if (plainShipRoom != null && plainShipRoom.roomArea)
                {
                    var num = plainShipRoom.roomArea.OverlapCollider(__instance.filter, __instance.buffer);
                    HashSet<byte> countedPlayers = [];
                    HashSet<int> countedDeadBodies = [];
                    var num2 = 0;

                    // ロミジュリと絵画の部屋をアドミンの対象から外す
                    var forceZero = false;
                    if (CustomOptionHolder.AirshipOldAdmin.GetBool() && counterArea.RoomType is SystemTypes.Ventilation or SystemTypes.HallOfPortraits)
                        forceZero = true;

                    // アドミン毎に表示する範囲を制限する
                    if (!FilterAdmin(counterArea.RoomType)) forceZero = true;

                    for (var j = 0; j < num; j++)
                    {
                        var collider2D = __instance.buffer[j];
                        if (!(collider2D.tag == "DeadBody"))
                        {
                            var component = collider2D.GetComponent<PlayerControl>();
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
                            var component = collider2D.GetComponent<DeadBody>();
                            if (component && countedDeadBodies.Add(component.GetInstanceID()))
                            {
                                var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);
                                if (playerInfo != null)
                                {
                                    var color = Palette.PlayerColors[playerInfo.Object.CurrentOutfit.ColorId];
                                    if (Hacker.OnlyColorType) color = Helpers.IsLighterColor(playerInfo.Object.CurrentOutfit.ColorId) ? Palette.PlayerColors[7] : Palette.PlayerColors[6];

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
                    Logger.LogWarn("Couldn't find counter for:" + counterArea.RoomType);
            }
            else
                counterArea.UpdateCount(0);
        }

        return false;
    }

    public static void OnEnable(MapCountOverlay __instance)
    {
        _adminTimer = 0f;

        // 現在地からどのアドミンを使っているか特定する
        _room = Helpers.GetPlainShipRoom(PlayerControl.LocalPlayer);

        if (_room == null) return;

        // アドミンの画像を差し替える
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) && !EvilHacker.IsInherited() && Helpers.IsAirship && CustomOptionHolder.AirshipRestrictedAdmin.GetBool() && _room.name is "Cockpit" or "Records")
        {
            if (!_map) _map = DestroyableSingleton<MapBehaviour>.Instance.gameObject.GetComponentsInChildren<SpriteRenderer>().FirstOrDefault(x => x.name == "Background").gameObject;

            if (!_newMap) _newMap = Object.Instantiate(_map, _map.transform.parent);

            var renderer = _newMap.GetComponent<SpriteRenderer>();
            if (_room.name == "Cockpit")
            {
                renderer.sprite = AssetLoader.AdminCockpit;
                _newMap.transform.position = new(_map.transform.position.x + 0.5f, _map.transform.position.y, _map.transform.position.z - 0.1f);
            }

            if (_room.name == "Records")
            {
                renderer.sprite = AssetLoader.AdminRecords;
                _newMap.transform.position = new(_map.transform.position.x - 0.38f, _map.transform.position.y, _map.transform.position.z - 0.1f);
            }

            _newMap.SetActive(true);
        }
    }

    public static void OnDisable()
    {
        UseAdminTime();
        IsEvilHackerAdmin = false;
        if (_newMap) _newMap.SetActive(false);
    }

    public static void UpdateCount(CounterArea __instance)
    {
        // Hacker display saved colors on the admin panel
        var showHackerInfo = PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && Hacker.HackerTimer > 0;
        if (_playerColors.ContainsKey(__instance.RoomType))
        {
            var colors = _playerColors[__instance.RoomType];
            List<Color> impostorColors = [];
            List<Color> mimicKColors = [];
            List<Color> deadBodyColors = [];
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                // var color = p.myRend.material.GetColor("_BodyColor");
                var color = Palette.PlayerColors[p.Data.DefaultOutfit.ColorId];
                if (p.IsTeamImpostor())
                    impostorColors.Add(color);
                else if (p.IsDead()) deadBodyColors.Add(color);
            }

            for (var i = 0; i < __instance.myIcons.Count; i++)
            {
                var icon = __instance.myIcons[i];
                var renderer = icon.GetComponent<SpriteRenderer>();

                if (renderer != null)
                {
                    if (_defaultMat == null) _defaultMat = renderer.material;
                    if (_newMat == null) _newMat = Object.Instantiate(_defaultMat);
                    if (showHackerInfo && colors.Count > i)
                    {
                        renderer.material = _newMat;
                        var color = colors[i];
                        renderer.material.SetColor("_BodyColor", color);
                        var id = Palette.PlayerColors.IndexOf(color);
                        if (id < 0)
                            renderer.material.SetColor("_BackColor", color);
                        else
                            renderer.material.SetColor("_BackColor", Palette.ShadowColors[id]);
                        renderer.material.SetColor("_VisorColor", Palette.VisorColor);
                    }
                    else if ((PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited()) && EvilHacker.CanHasBetterAdmin)
                    {
                        renderer.material = _newMat;
                        var color = colors[i];
                        if (impostorColors.Contains(color))
                        {
                            if (mimicKColors.Contains(color))
                                color = Palette.PlayerColors[3];
                            else
                                color = Palette.ImpostorRed;
                            renderer.material.SetColor("_BodyColor", color);
                            var id = Palette.PlayerColors.IndexOf(color);
                            if (id < 0)
                                renderer.material.SetColor("_BackColor", color);
                            else
                                renderer.material.SetColor("_BackColor", Palette.ShadowColors[id]);
                            renderer.material.SetColor("_VisorColor", Palette.VisorColor);
                        }
                        else if (deadBodyColors.Contains(color))
                        {
                            color = Palette.Black;
                            renderer.material.SetColor("_BodyColor", color);
                            var id = Palette.PlayerColors.IndexOf(color);
                            if (id < 0)
                                renderer.material.SetColor("_BackColor", color);
                            else
                                renderer.material.SetColor("_BackColor", Palette.ShadowColors[id]);
                            renderer.material.SetColor("_VisorColor", Palette.VisorColor);
                        }
                        else
                            renderer.material = _defaultMat;
                    }
                    else
                        renderer.material = _defaultMat;
                }
            }
        }
    }
}
