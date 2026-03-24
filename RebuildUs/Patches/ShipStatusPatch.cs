namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ShipStatusPatch
{
    private static SwitchSystem _cachedSwitchSystem;
    private static ShipStatus _lastShipStatus;

    private static int _originalNumCommonTasksOption;
    private static int _originalNumShortTasksOption;
    private static int _originalNumLongTasksOption;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    internal static bool CalculateLightRadiusPrefix(ref float __result, ShipStatus __instance, NetworkedPlayerInfo player)
    {
        if (!__instance.Systems.ContainsKey(SystemTypes.Electrical) && !Helpers.IsFungle || Helpers.IsHideNSeekMode)
        {
            return true;
        }

        // If player is a role which has Impostor vision
        if (player.Object.HasImpostorVision())
        {
            __result = GetNeutralLightRadius(__instance, true);
            return false;
        }

        // If player is Lighter with ability active
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Lighter) && Lighter.IsLightActive(PlayerControl.LocalPlayer))
        {
            var unLerp = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, true));
            __result = Mathf.Lerp(__instance.MaxLightRadius * Lighter.ModeLightsOffVision, __instance.MaxLightRadius * Lighter.ModeLightsOnVision, unLerp);
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

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1 - lerpValue) * FloatOptionNames.CrewLightMod.Get();
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

    // 動作確認しました
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