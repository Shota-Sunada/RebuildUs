namespace RebuildUs.Modules.Consoles;

internal static class Admin
{
    private static Dictionary<SystemTypes, List<Color>> _playerColors = [];
    private static float _adminTimer;
    private static TextMeshPro _outOfTime;
    private static TextMeshPro _timeRemaining;
    private static bool _clearedIcons;
    internal static bool IsEvilHackerAdmin;
    private static GameObject _map;
    private static GameObject _newMap;

    private static PlainShipRoom _room;
    private static readonly SystemTypes[] FilterCockpitAdmin =
    [
        SystemTypes.Cockpit,
        SystemTypes.Armory,
        SystemTypes.Kitchen,
        SystemTypes.VaultRoom,
        SystemTypes.Comms,
    ];
    private static readonly SystemTypes[] FilterRecordsAdmin =
    [
        SystemTypes.Records,
        SystemTypes.Lounge,
        SystemTypes.CargoBay,
        SystemTypes.Showers,
        SystemTypes.Ventilation,
    ];

    private static Material _defaultMat;
    private static Material _newMat;
    private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
    private static readonly int BackColor = Shader.PropertyToID("_BackColor");
    private static readonly int VisorColor = Shader.PropertyToID("_VisorColor");

    private static bool FilterAdmin(SystemTypes type)
    {
        // イビルハッカーのアドミンは今まで通り
        var lp = PlayerControl.LocalPlayer;
        if (lp != null && (lp.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited()))
        {
            return true;
        }

        if (!CustomOptionHolder.AirshipRestrictedAdmin.GetBool())
        {
            return true;
        }
        if (_room == null)
        {
            return true;
        }
        var roomName = _room.name;
        switch (roomName)
        {
            case "Cockpit":
                {
                    foreach (var t in FilterCockpitAdmin)
                    {
                        if (t == type)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            case "Records":
                {
                    foreach (var t in FilterRecordsAdmin)
                    {
                        if (t == type)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            default:
                return true;
        }
    }

    internal static void ResetData()
    {
        _adminTimer = 0f;
        if (_timeRemaining != null)
        {
            UnityObject.Destroy(_timeRemaining.gameObject);
            _timeRemaining = null;
        }

        if (_outOfTime == null)
        {
            return;
        }
        UnityObject.Destroy(_outOfTime.gameObject);
        _outOfTime = null;
    }

    private static void UseAdminTime()
    {
        // Don't waste network traffic if we're out of time.
        if (!IsEvilHackerAdmin)
        {
            if (MapSettings.RestrictDevices > 0
                && MapSettings.RestrictAdmin
                && MapSettings.RestrictAdminTime > 0f
                && PlayerControl.LocalPlayer.IsAlive())
            {
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UseAdminTime);
                sender.Write(_adminTimer);
                RPCProcedure.UseAdminTime(_adminTimer);
            }
        }

        _adminTimer = 0f;
    }

    internal static bool Update(MapCountOverlay __instance)
    {
        _adminTimer += Time.deltaTime;
        if (_adminTimer > 0.1f)
        {
            UseAdminTime();
        }

        // Save colors for the Hacker
        __instance.timer += Time.deltaTime;
        if (__instance.timer < 0.1f)
        {
            return false;
        }

        __instance.timer = 0f;

        _playerColors = [];

        if (MapSettings.RestrictDevices > 0 && MapSettings.RestrictAdmin)
        {
            if (_outOfTime == null)
            {
                _outOfTime = UnityObject.Instantiate(__instance.SabotageText, __instance.SabotageText.transform.parent);
                _outOfTime.text = Tr.Get(TrKey.RestrictOutOfTime);
            }

            if (_timeRemaining == null)
            {
                _timeRemaining = UnityObject.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, __instance.transform);
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
                if (_clearedIcons)
                {
                    return false;
                }
                foreach (var ca in __instance.CountAreas)
                {
                    ca.UpdateCount(0);
                }
                _clearedIcons = true;

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
            {
                commsActive = true;
            }
        }

        if (CustomOptionHolder.ImpostorCanIgnoreCommSabotage.GetBool() && PlayerControl.LocalPlayer.IsTeamImpostor())
        {
            commsActive = false;
        }

        switch (__instance.isSab)
        {
            case false when commsActive:
                __instance.isSab = true;
                __instance.BackgroundColor.SetColor(Palette.DisabledGrey);
                __instance.SabotageText.gameObject.SetActive(true);
                _outOfTime.gameObject.SetActive(false);
                return false;
            case true when !commsActive:
                __instance.isSab = false;
                __instance.BackgroundColor.SetColor(Color.green);
                __instance.SabotageText.gameObject.SetActive(false);
                _outOfTime.gameObject.SetActive(false);
                break;
        }

        foreach (var counterArea in __instance.CountAreas)
        {
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
                    // アドミン毎に表示する範囲を制限する
                    var forceZero = CustomOptionHolder.AirshipOldAdmin.GetBool()
                                     && counterArea.RoomType is SystemTypes.Ventilation or SystemTypes.HallOfPortraits
                                     || !FilterAdmin(counterArea.RoomType);

                    for (var j = 0; j < num; j++)
                    {
                        var collider2D = __instance.buffer[j];
                        if (collider2D.tag != "DeadBody")
                        {
                            var component = collider2D.GetComponent<PlayerControl>();
                            if (!component || component.Data == null || component.Data.Disconnected || component.Data.IsDead)
                            {
                                continue;
                            }
                            if (!countedPlayers.Add(component.PlayerId))
                            {
                                continue;
                            }
                            num2++;
                            if (component?.cosmetics?.currentBodySprite?.BodySprite.material == null)
                            {
                                continue;
                            }
                            // Color color = component.myRend.material.GetColor("_BodyColor");
                            Color color = Palette.PlayerColors[component.Data.DefaultOutfit.ColorId];
                            if (Hacker.OnlyColorType)
                            {
                                var id = Mathf.Max(0, Palette.PlayerColors.IndexOf(color));
                                color = Helpers.IsLighterColor((byte)id) ? Palette.PlayerColors[7] : Palette.PlayerColors[6];
                            }

                            roomColors.Add(color);
                        }
                        else
                        {
                            var component = collider2D.GetComponent<DeadBody>();
                            if (!component || !countedDeadBodies.Add(component.GetInstanceID()))
                            {
                                continue;
                            }
                            var playerInfo = GameData.Instance.GetPlayerById(component.ParentId);
                            if (playerInfo == null)
                            {
                                continue;
                            }
                            var color = Palette.PlayerColors[playerInfo.Object.CurrentOutfit.ColorId];
                            if (Hacker.OnlyColorType)
                            {
                                color = Helpers.IsLighterColor(playerInfo.Object.CurrentOutfit.ColorId)
                                    ? Palette.PlayerColors[7]
                                    : Palette.PlayerColors[6];
                            }

                            roomColors.Add(color);
                        }
                    }

                    if (forceZero)
                    {
                        num2 = 0;
                    }
                    if (num2 < 0)
                    {
                        num2 = 0;
                    }
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

    internal static void OnEnable(MapCountOverlay __instance)
    {
        _adminTimer = 0f;

        // 現在地からどのアドミンを使っているか特定する
        _room = Helpers.GetPlainShipRoom(PlayerControl.LocalPlayer);

        if (_room == null)
        {
            return;
        }

        // アドミンの画像を差し替える
        if (PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker)
            || EvilHacker.IsInherited()
            || !Helpers.IsAirship
            || !CustomOptionHolder.AirshipRestrictedAdmin.GetBool()
            || _room.name is not ("Cockpit" or "Records"))
        {
            return;
        }

        if (!_map)
        {
            var renderers = MapBehaviour.Instance.gameObject.GetComponentsInChildren<SpriteRenderer>();

            foreach (var rend in renderers)
            {
                if (rend.name is "Background")
                {
                    _map = rend.gameObject;
                }
            }
        }

        if (!_newMap)
        {
            if (_map != null)
            {
                _newMap = UnityObject.Instantiate(_map, _map.transform.parent);
            }
        }

        var renderer = _newMap.GetComponent<SpriteRenderer>();
        switch (_room.name)
        {
            case "Cockpit":
                {
                    renderer.sprite = AssetLoader.AdminCockpit;
                    if (_map != null)
                    {
                        _newMap.transform.position = new(_map.transform.position.x + 0.5f,
                            _map.transform.position.y,
                            _map.transform.position.z - 0.1f);
                    }
                    break;
                }
            case "Records":
                {
                    renderer.sprite = AssetLoader.AdminRecords;
                    if (_map != null)
                    {
                        _newMap.transform.position = new(_map.transform.position.x - 0.38f,
                            _map.transform.position.y,
                            _map.transform.position.z - 0.1f);
                    }
                    break;
                }
        }

        _newMap.SetActive(true);
    }

    internal static void OnDisable()
    {
        UseAdminTime();
        IsEvilHackerAdmin = false;
        if (_newMap)
        {
            _newMap.SetActive(false);
        }
    }

    internal static void UpdateCount(CounterArea __instance)
    {
        // Hacker display saved colors on the admin panel
        var showHackerInfo = PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && Hacker.HackerTimer > 0;
        if (!_playerColors.TryGetValue(__instance.RoomType, out var colors))
        {
            return;
        }
        List<Color> impostorColors = [];
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

        for (var i = 0; i < __instance.myIcons.Count; i++)
        {
            var icon = __instance.myIcons[i];
            var renderer = icon.GetComponent<SpriteRenderer>();

            if (renderer == null)
            {
                continue;
            }
            if (_defaultMat == null)
            {
                _defaultMat = renderer.material;
            }
            if (_newMat == null)
            {
                _newMat = UnityObject.Instantiate(_defaultMat);
            }
            if (showHackerInfo && colors.Count > i)
            {
                renderer.material = _newMat;
                var color = colors[i];
                renderer.material.SetColor(BodyColor, color);
                var id = Palette.PlayerColors.IndexOf(color);
                if (id < 0)
                {
                    renderer.material.SetColor(BackColor, color);
                }
                else
                {
                    renderer.material.SetColor(BackColor, Palette.ShadowColors[id]);
                }

                renderer.material.SetColor(VisorColor, Palette.VisorColor);
            }
            else if ((PlayerControl.LocalPlayer.IsRole(RoleType.EvilHacker) || EvilHacker.IsInherited()) && EvilHacker.CanHasBetterAdmin)
            {
                renderer.material = _newMat;
                var color = colors[i];
                if (impostorColors.Contains(color))
                {
                    color = Palette.ImpostorRed;

                    renderer.material.SetColor(BodyColor, color);
                    var id = Palette.PlayerColors.IndexOf(color);
                    if (id < 0)
                    {
                        renderer.material.SetColor(BackColor, color);
                    }
                    else
                    {
                        renderer.material.SetColor(BackColor, Palette.ShadowColors[id]);
                    }

                    renderer.material.SetColor(VisorColor, Palette.VisorColor);
                }
                else if (deadBodyColors.Contains(color))
                {
                    color = Palette.Black;
                    renderer.material.SetColor(BodyColor, color);
                    var id = Palette.PlayerColors.IndexOf(color);
                    if (id < 0)
                    {
                        renderer.material.SetColor(BackColor, color);
                    }
                    else
                    {
                        renderer.material.SetColor(BackColor, Palette.ShadowColors[id]);
                    }

                    renderer.material.SetColor(VisorColor, Palette.VisorColor);
                }
                else
                {
                    renderer.material = _defaultMat;
                }
            }
            else
            {
                renderer.material = _defaultMat;
            }
        }
    }
}