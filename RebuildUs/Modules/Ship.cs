namespace RebuildUs.Modules;

public static class Ship
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static bool CalculateLightRadiusPrefix(ref float __result, ShipStatus __instance, NetworkedPlayerInfo player)
    {
        if ((!__instance.Systems.ContainsKey(SystemTypes.Electrical) && !Helpers.IsFungle) || Helpers.IsHideNSeekMode) return true;

        // If player is a role which has Impostor vision
        if (Helpers.HasImpostorVision(player.Object))
        {
            // __result = __instance.MaxLightRadius * PlayerControl.GameOptions.ImpostorLightMod;
            __result = GetNeutralLightRadius(__instance, true);
            return false;
        }

        // If player is Lighter with ability active
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Lighter) && Lighter.IsLightActive(CachedPlayer.LocalPlayer.PlayerControl))
        {
            float unLerp = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, true));
            __result = Mathf.Lerp(__instance.MaxLightRadius * Lighter.ModeLightsOffVision, __instance.MaxLightRadius * Lighter.ModeLightsOnVision, unLerp);
            return false;
        }

        // If there is a Trickster with their ability active
        if (Trickster.Exists && Trickster.LightsOutTimer > 0f)
        {
            float lerpValue = 1f;
            if (Trickster.LightsOutDuration - Trickster.LightsOutTimer < 0.5f)
            {
                lerpValue = Mathf.Clamp01((Trickster.LightsOutDuration - Trickster.LightsOutTimer) * 2);
            }
            else if (Trickster.LightsOutTimer < 0.5)
            {
                lerpValue = Mathf.Clamp01(Trickster.LightsOutTimer * 2);
            }

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1 - lerpValue) * Helpers.GetOption(FloatOptionNames.CrewLightMod);
            return false;
        }

        // Default light radius
        __result = GetNeutralLightRadius(__instance, false);
        return false;
    }

    public static float GetNeutralLightRadius(ShipStatus shipStatus, bool isImpostor)
    {
        if (SubmergedCompatibility.IsSubmerged)
        {
            return SubmergedCompatibility.GetSubmergedNeutralLightRadius(isImpostor);
        }

        if (isImpostor) return shipStatus.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
        float lerpValue = 1.0f;
        try
        {
            SwitchSystem switchSystem = MapUtilities.Systems[SystemTypes.Electrical].CastFast<SwitchSystem>();
            lerpValue = switchSystem.Value / 255f;
        }
        catch { }

        return Mathf.Lerp(shipStatus.MinLightRadius, shipStatus.MaxLightRadius, lerpValue) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static void Postfix2(ShipStatus __instance, ref bool __result)
    {
        __result = false;
    }

    private static int OriginalNumCommonTasksOption = 0;
    private static int OriginalNumShortTasksOption = 0;
    private static int OriginalNumLongTasksOption = 0;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static bool Prefix(ShipStatus __instance)
    {
        var commonTaskCount = __instance.CommonTasks.Count;
        var normalTaskCount = __instance.ShortTasks.Count;
        var longTaskCount = __instance.LongTasks.Count;

        OriginalNumCommonTasksOption = Helpers.GetOption(Int32OptionNames.NumCommonTasks);
        OriginalNumShortTasksOption = Helpers.GetOption(Int32OptionNames.NumShortTasks);
        OriginalNumLongTasksOption = Helpers.GetOption(Int32OptionNames.NumLongTasks);

        if (Helpers.GetOption(Int32OptionNames.NumCommonTasks) > commonTaskCount) Helpers.SetOption(Int32OptionNames.NumCommonTasks, commonTaskCount);
        if (Helpers.GetOption(Int32OptionNames.NumShortTasks) > normalTaskCount) Helpers.SetOption(Int32OptionNames.NumShortTasks, normalTaskCount);
        if (Helpers.GetOption(Int32OptionNames.NumLongTasks) > longTaskCount) Helpers.SetOption(Int32OptionNames.NumLongTasks, longTaskCount);

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    public static void Postfix3(ShipStatus __instance)
    {
        // Restore original settings after the tasks have been selected
        Helpers.SetOption(Int32OptionNames.NumCommonTasks, OriginalNumCommonTasksOption);
        Helpers.SetOption(Int32OptionNames.NumShortTasks, OriginalNumShortTasksOption);
        Helpers.SetOption(Int32OptionNames.NumLongTasks, OriginalNumLongTasksOption);

        // 一部役職のタスクを再割り当てする
        {
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.FinishShipStatusBegin);
            RPCProcedure.FinishShipStatusBegin();
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    class ShipStatusStartPatch
    {
        public static void Postfix()
        {
            Logger.LogInfo("Game Started", "Phase");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.SpawnPlayer))]
    public static void Postfix(ShipStatus __instance, PlayerControl player, int numPlayers, bool initialSpawn)
    {
        // Polusの湧き位置をランダムにする 無駄に人数分シャッフルが走るのをそのうち直す
        if (Helpers.IsPolus && CustomOptionHolder.PolusRandomSpawn.GetBool())
        {
            if (AmongUsClient.Instance.AmHost)
            {
                System.Random rand = new();
                byte randVal = (byte)rand.Next(0, 6);
                using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.PolusRandomSpawn);
                sender.Write(player.Data.PlayerId);
                sender.Write(randVal);
                RPCProcedure.PolusRandomSpawn((byte)player.Data.PlayerId, (byte)randVal);
            }
        }

        CustomButton.StopCountdown = false;
    }
}