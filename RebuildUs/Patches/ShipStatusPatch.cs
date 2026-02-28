namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ShipStatusPatch
{
    private static SwitchSystem _cachedSwitchSystem;
    private static ShipStatus _lastShipStatus;

    private static int _originalNumCommonTasksOption;
    private static int _originalNumShortTasksOption;
    private static int _originalNumLongTasksOption;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    internal static void AwakePostfix(ShipStatus __instance)
    {
        if (ByteOptionNames.MapId.Get() != 4)
        {
            return;
        }

        if (CustomOptionHolder.AirshipAdditionalWireTask.GetBool())
        {
            Airship.ActivateWiring("task_wiresHallway2", 2);
            Airship.ActivateWiring("task_electricalside2", 3).Room = SystemTypes.Armory;
            Airship.ActivateWiring("task_wireShower", 4);
            Airship.ActivateWiring("taks_wiresLounge", 5);
            Airship.ActivateWiring("panel_wireHallwayL", 6);
            Airship.ActivateWiring("task_wiresStorage", 7);
            Airship.ActivateWiring("task_electricalSide", 8).Room = SystemTypes.VaultRoom;
            Airship.ActivateWiring("task_wiresMeeting", 9);
        }

        if (CustomOptionHolder.AirshipOptimize.GetBool())
        {
            var obj = MapUtilities.CachedShipStatus.FastRooms[SystemTypes.GapRoom].gameObject;
            // 昇降機右に影を追加
            var oneWayShadow = obj.transform.FindChild("Shadow").FindChild("LedgeShadow").GetComponent<OneWayShadows>();
            oneWayShadow.enabled = false;
            if (PlayerControl.LocalPlayer.IsTeamImpostor())
            {
                oneWayShadow.gameObject.SetActive(false);
            }

            GameObject fence = new("ModFence")
            {
                layer = LayerMask.NameToLayer("Ship"),
            };
            fence.transform.SetParent(obj.transform);
            fence.transform.localPosition = new(4.2f, 0.15f, 0.5f);
            fence.transform.localScale = new(1f, 1f, 1f);
            fence.SetActive(true);
            var collider = fence.AddComponent<EdgeCollider2D>();
            collider.points = new Vector2[]
            {
                new(1.5f, -0.2f), new(-1.5f, -0.2f), new(-1.5f, 1.5f),
            };
            collider.enabled = true;
            var renderer = fence.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetLoader.AirshipFence;

            // GameObject pole = new("DownloadPole")
            // {
            //     layer = LayerMask.NameToLayer("Ship")
            // };
            // pole.transform.SetParent(obj.transform);
            // pole.transform.localPosition = new Vector3(4.1f, 0.75f, 0.8f);
            // pole.transform.localScale = new Vector3(1f, 1f, 1f);
            // renderer = pole.AddComponent<SpriteRenderer>();
            // renderer.sprite = AssetLoader.AirshipDownloadG;

            var panel = obj.transform.FindChild("panel_data");
            panel.localPosition = new(4.52f, -3.95f, 0.1f);
            // panel.gameObject.GetComponent<Console>().usableDistance = 0.9f;
        }

        {
            // Add Ladder
            var meetingRoom = MapUtilities.CachedShipStatus.FastRooms[SystemTypes.MeetingRoom].gameObject;
            var gapRoom = MapUtilities.CachedShipStatus.FastRooms[SystemTypes.GapRoom].gameObject;

            var meetingRenderers = meetingRoom.GetComponentsInChildren<SpriteRenderer>();
            GameObject ladder = null;
            foreach (var renderer in meetingRenderers)
            {
                if (renderer.name != "ladder_meeting")
                {
                    continue;
                }
                ladder = renderer.gameObject;
                break;
            }

            if (CustomOptionHolder.AirshipAdditionalLadder.GetBool())
            {
                // 梯子追加
                if (ladder != null)
                {
                    var newLadder = UnityObject.Instantiate(ladder, ladder.transform.parent);
                    var ladders = newLadder.GetComponentsInChildren<Ladder>();
                    var id = 100;
                    foreach (var l in ladders)
                    {
                        if (l.name == "LadderBottom")
                        {
                            l.gameObject.SetActive(false);
                        }
                        l.Id = (byte)id;
                        FastDestroyableSingleton<AirshipStatus>.Instance.Ladders.AddItem(l);
                        id++;
                    }

                    newLadder.transform.position = new(15.442f, 12.18f, 0.1f);
                    newLadder.GetComponentInChildren<SpriteRenderer>().sprite = AssetLoader.Ladder;
                }

                // 梯子の周りの影を消す
                foreach (var x in gapRoom.GetComponentsInChildren<EdgeCollider2D>())
                {
                    if (!(Math.Abs(x.points[0].x + 6.2984f) < 0.1))
                    {
                        continue;
                    }
                    UnityObject.Destroy(x);
                    break;
                }

                EdgeCollider2D collider = null;
                foreach (var x in meetingRoom.GetComponentsInChildren<EdgeCollider2D>())
                {
                    if (x.pointCount != 46)
                    {
                        continue;
                    }
                    collider = x;
                    break;
                }

                if (collider != null)
                {
                    Il2CppSystem.Collections.Generic.List<Vector2> points = new();
                    var newCollider = collider.gameObject.AddComponent<EdgeCollider2D>();
                    var newCollider2 = collider.gameObject.AddComponent<EdgeCollider2D>();
                    points.Add(collider.points[45]);
                    points.Add(collider.points[44]);
                    points.Add(collider.points[43]);
                    points.Add(collider.points[42]);
                    points.Add(collider.points[41]);
                    newCollider.SetPoints(points);
                    points.Clear();
                    for (var i = 0; i < 41; i++)
                    {
                        points.Add(collider.points[i]);
                    }

                    newCollider2.SetPoints(points);
                    UnityObject.DestroyObject(collider);
                }

                // 梯子の背景を変更
                SpriteRenderer side = null;
                foreach (var r in meetingRenderers)
                {
                    if (r.name != "meeting_side")
                    {
                        continue;
                    }
                    side = r;
                    break;
                }

                if (side != null)
                {
                    var bg = UnityObject.Instantiate(side, side.transform.parent);
                    bg.sprite = AssetLoader.LadderBackground;
                    bg.transform.localPosition = new(9.57f, -3.355f, 4.9f);
                }
            }

            if (!CustomOptionHolder.AirshipOneWayLadder.GetBool())
            {
                return;
            }
            {
                if (ladder == null)
                {
                    return;
                }
                foreach (var l in ladder.GetComponentsInChildren<Ladder>())
                {
                    if (l.name != "LadderTop")
                    {
                        continue;
                    }
                    l.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnEnable))]
    internal static void OnEnablePostfix() { }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    internal static bool CalculateLightRadiusPrefix(ref float __result, ShipStatus __instance, NetworkedPlayerInfo player)
    {
        if (!__instance.Systems.ContainsKey(SystemTypes.Electrical) && !Helpers.IsFungle || Helpers.IsHideNSeekMode)
        {
            return true;
        }

        // If player is a role which has Impostor vision
        if (Helpers.HasImpostorVision(player.Object))
        {
            __result = GetNeutralLightRadius(__instance, true);
            return false;
        }

        // If player is Lighter with ability active

        if (PlayerControl.LocalPlayer.IsRole(RoleType.Lighter) && Lighter.IsLightActive(PlayerControl.LocalPlayer))
        {
            var unLerp = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, true));
            __result = Mathf.Lerp(__instance.MaxLightRadius * Lighter.ModeLightsOffVision,
                __instance.MaxLightRadius * Lighter.ModeLightsOnVision,
                unLerp);
            return false;
        }

        // If there is a Trickster with their ability active

        if (Trickster.Exists && Trickster.LightsOutTimer > 0f)
        {
            var lerpValue = 1f;
            if (Trickster.LightsOutDuration - Trickster.LightsOutTimer < 0.5f)
            {
                lerpValue = Mathf.Clamp01((Trickster.LightsOutDuration - Trickster.LightsOutTimer) * 2);
            }
            else if (Trickster.LightsOutTimer < 0.5)
            {
                lerpValue = Mathf.Clamp01(Trickster.LightsOutTimer * 2);
            }

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1 - lerpValue)
                       * FloatOptionNames.CrewLightMod.Get();
            return false;
        }

        // Default light radius

        __result = GetNeutralLightRadius(__instance, false);

        return false;
    }

    private static float GetNeutralLightRadius(ShipStatus shipStatus, bool isImpostor)
    {
        if (SubmergedCompatibility.IsSubmerged)
        {
            return SubmergedCompatibility.GetSubmergedNeutralLightRadius(isImpostor);
        }

        if (isImpostor)
        {
            return shipStatus.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
        }

        var lerpValue = 1.0f;
        UpdateCachedSystems(shipStatus);
        if (_cachedSwitchSystem != null)
        {
            lerpValue = _cachedSwitchSystem.Value / 255f;
        }

        return Mathf.Lerp(shipStatus.MinLightRadius, shipStatus.MaxLightRadius, lerpValue)
               * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
    }

    private static void UpdateCachedSystems(ShipStatus instance)
    {
        if (_lastShipStatus == instance && _cachedSwitchSystem != null)
        {
            return;
        }
        _lastShipStatus = instance;
        _cachedSwitchSystem = null;
        if (instance != null && instance.Systems != null && instance.Systems.TryGetValue(SystemTypes.Electrical, out var system))
        {
            _cachedSwitchSystem = system.CastFast<SwitchSystem>();
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    internal static bool BeginPrefix(ShipStatus __instance)
    {
        var commonTaskCount = __instance.CommonTasks.Count;
        var normalTaskCount = __instance.ShortTasks.Count;
        var longTaskCount = __instance.LongTasks.Count;

        _originalNumCommonTasksOption = Int32OptionNames.NumCommonTasks.Get();
        _originalNumShortTasksOption = Int32OptionNames.NumShortTasks.Get();
        _originalNumLongTasksOption = Int32OptionNames.NumLongTasks.Get();

        if (Int32OptionNames.NumCommonTasks.Get() > commonTaskCount)
        {
            Int32OptionNames.NumCommonTasks.Set(commonTaskCount);
        }
        if (Int32OptionNames.NumShortTasks.Get() > normalTaskCount)
        {
            Int32OptionNames.NumShortTasks.Set(normalTaskCount);
        }
        if (Int32OptionNames.NumLongTasks.Get() > longTaskCount)
        {
            Int32OptionNames.NumLongTasks.Set(longTaskCount);
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    internal static void BeginPostfix(ShipStatus __instance)
    {
        // Restore original settings after the tasks have been selected
        Int32OptionNames.NumCommonTasks.Set(_originalNumCommonTasksOption);
        Int32OptionNames.NumShortTasks.Set(_originalNumShortTasksOption);
        Int32OptionNames.NumLongTasks.Set(_originalNumLongTasksOption);

        // 一部役職のタスクを再割り当てする
        {
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.FinishShipStatusBegin);
            RPCProcedure.FinishShipStatusBegin();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    internal static void StartPostfix() { }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.SpawnPlayer))]
    internal static void SpawnPlayerPostfix(ShipStatus __instance, PlayerControl player, int numPlayers, bool initialSpawn)
    {
        // Polusの湧き位置をランダムにする
        if (Helpers.IsPolus && CustomOptionHolder.PolusRandomSpawn.GetBool())
        {
            if (AmongUsClient.Instance.AmHost && player.Data != null)
            {
                var randVal = (byte)RebuildUs.Rnd.Next(0, 6);
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.PolusRandomSpawn);
                sender.Write(player.Data.PlayerId);
                sender.Write(randVal);
                RPCProcedure.PolusRandomSpawn(player.Data.PlayerId, randVal);
            }
        }

        CustomButton.StopCountdown = false;
    }
}