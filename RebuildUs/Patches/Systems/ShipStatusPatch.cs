namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ShipStatusPatch
{
    private static int _originalNumCommonTasksOption;
    private static int _originalNumShortTasksOption;
    private static int _originalNumLongTasksOption;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    internal static bool CalculateLightRadiusPrefix(ref float __result, ShipStatus __instance, NetworkedPlayerInfo player)
    {
        switch (GameModeManager.CurrentGameMode)
        {
            default:
            case CustomGamemode.Normal:
                // If player is a role which has Impostor vision
                if (player.Object.HasImpostorVision())
                {
                    __result = GetNeutralLightRadius(true);
                    return false;
                }

                // If player is Lighter with ability active
                if (PlayerControl.LocalPlayer.IsRole(RoleType.Lighter) && Lighter.IsLightActive(PlayerControl.LocalPlayer))
                {
                    var unLerp = Mathf.InverseLerp(MapUtilities.CachedShipStatus.MinLightRadius, MapUtilities.CachedShipStatus.MaxLightRadius, GetNeutralLightRadius(true));
                    __result = Mathf.Lerp(MapUtilities.CachedShipStatus.MaxLightRadius * Lighter.ModeLightsOffVision, MapUtilities.CachedShipStatus.MaxLightRadius * Lighter.ModeLightsOnVision, unLerp);
                    return false;
                }

                // If there is a Trickster with their ability active
                if (Trickster.Exists && Trickster.LightsOutTimer > 0f)
                {
                    var lerpValue = 1f;
                    if ((Trickster.LightsOutDuration - Trickster.LightsOutTimer) < 0.5f)
                    {
                        lerpValue = Mathf.Clamp01((Trickster.LightsOutDuration - Trickster.LightsOutTimer) * 2);
                    }
                    else if (Trickster.LightsOutTimer < 0.5)
                    {
                        lerpValue = Mathf.Clamp01(Trickster.LightsOutTimer * 2);
                    }

                    __result = Mathf.Lerp(MapUtilities.CachedShipStatus.MinLightRadius, MapUtilities.CachedShipStatus.MaxLightRadius, 1 - lerpValue) * FloatOptionNames.CrewLightMod.Get();
                    return false;
                }

                // Default light radius
                __result = GetNeutralLightRadius(false);
                break;
            case CustomGamemode.BattleRoyale:
                __result = CustomOptionHolder.BattleRoyaleVisionRange.GetFloat();
                break;
        }

        return false;
    }

    private static float GetNeutralLightRadius(bool isImpostor)
    {
        if (SubmergedCompatibility.IsSubmerged)
        {
            return SubmergedCompatibility.GetSubmergedNeutralLightRadius(isImpostor);
        }

        if (isImpostor)
        {
            return MapUtilities.CachedShipStatus.MaxLightRadius * Helpers.Get(FloatOptionNames.ImpostorLightMod);
        }

        var lerpValue = 1.0f;
        if (MapUtilities.Systems.TryGetValue(SystemTypes.Electrical, out var system))
        {
            lerpValue = system.CastFast<SwitchSystem>().Value / 255f;
        }

        return Mathf.Lerp(MapUtilities.CachedShipStatus.MinLightRadius, MapUtilities.CachedShipStatus.MaxLightRadius, lerpValue) * Helpers.Get(FloatOptionNames.CrewLightMod);
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
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.SpawnPlayer))]
    internal static void SpawnPlayerPostfix(ShipStatus __instance, PlayerControl player, int numPlayers, bool initialSpawn)
    {
        CustomButton.StopCountdown = false;
        if (!AmongUsClient.Instance.AmHost) return;
        if (Helpers.IsAirship || SubmergedCompatibility.IsSubmerged) return;
        if (CustomOptionHolder.RandomSpawn.GetBool()
            || GameModeManager.CurrentGameMode is CustomGamemode.BattleRoyale)
        {
            if (player.Data != null)
            {
                var mapId = ByteOptionNames.MapId.Get();
                byte index;
                switch (mapId)
                {
                    case 0:
                        index = (byte)RebuildUs.Rnd.Next(RandomSpawnPositions.SKELD_POSITIONS.Length);
                        break;
                    case 1:
                        index = (byte)RebuildUs.Rnd.Next(RandomSpawnPositions.MIRA_POSITIONS.Length);
                        break;
                    case 2:
                        index = (byte)RebuildUs.Rnd.Next(RandomSpawnPositions.POLUS_POSITIONS.Length);
                        break;
                    case 5:
                        index = (byte)RebuildUs.Rnd.Next(RandomSpawnPositions.FUNGLE_POSITIONS.Length);
                        break;
                    default:
                        return;
                }

                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.RandomSpawn);
                sender.Write(player.Data.PlayerId);
                sender.Write(mapId);
                sender.Write(index);
                RPCProcedure.RandomSpawn(player.Data.PlayerId, mapId, index);
            }
        }
    }
}