namespace RebuildUs.Modules;

public static class Ship
{
    private static SwitchSystem _cachedSwitchSystem;
    private static ShipStatus _lastShipStatus;

    public static void UpdateCachedSystems(ShipStatus instance)
    {
        if (_lastShipStatus == instance && _cachedSwitchSystem != null) return;
        _lastShipStatus = instance;
        _cachedSwitchSystem = null;
        if (instance != null && instance.Systems != null && instance.Systems.TryGetValue(SystemTypes.Electrical, out var system))
        {
            _cachedSwitchSystem = system.CastFast<SwitchSystem>();
        }
    }

    public static bool CalculateLightRadius(ref float __result, ShipStatus __instance, NetworkedPlayerInfo player)
    {
        switch (MapSettings.GameMode)
        {
            case CustomGameMode.Roles:
                if ((!__instance.Systems.ContainsKey(SystemTypes.Electrical) && !Helpers.IsFungle) || Helpers.IsHideNSeekMode) return true;

                // If player is a role which has Impostor vision
                if (Helpers.HasImpostorVision(player.Object))
                {
                    __result = GetNeutralLightRadius(__instance, true);
                    return false;
                }

                // If player is Lighter with ability active
                else if (PlayerControl.LocalPlayer.IsRole(RoleType.Lighter) && Lighter.IsLightActive(PlayerControl.LocalPlayer))
                {
                    float unLerp = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, true));
                    __result = Mathf.Lerp(__instance.MaxLightRadius * Lighter.ModeLightsOffVision, __instance.MaxLightRadius * Lighter.ModeLightsOnVision, unLerp);
                    return false;
                }

                // If there is a Trickster with their ability active
                else if (Trickster.Exists && Trickster.LightsOutTimer > 0f)
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
                else
                {
                    __result = GetNeutralLightRadius(__instance, false);
                }
                return false;

            case CustomGameMode.CaptureTheFlag:
            case CustomGameMode.BattleRoyale:
                if (player == null || player.IsDead)
                {
                    // IsDead
                    __result = __instance.MaxLightRadius;
                }
                else
                {
                    foreach (PlayerControl gamemodePlayer in PlayerControl.AllPlayerControls)
                    {
                        if (gamemodePlayer != null && gamemodePlayer.PlayerId == player.PlayerId)
                        {
                            float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, unlerped) * GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.CrewLightMod);
                        }
                    }
                }
                return false;

            case CustomGameMode.PoliceAndThieves:
                if (player == null || player.IsDead)
                {
                    // IsDead
                    __result = __instance.MaxLightRadius;
                }
                else
                {
                    foreach (PlayerControl gamemodePlayer in PlayerControl.AllPlayerControls)
                    {
                        if (gamemodePlayer != null && PoliceAndThief.policeplayer01 != null && gamemodePlayer == PoliceAndThief.policeplayer01 && PoliceAndThief.policeplayer01.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.policeplayer01)
                        {
                            if (PoliceAndThief.policeplayer01lightTimer > 0f)
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * MapSettings.gamemodeFlashlightRange, __instance.MaxLightRadius * MapSettings.gamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.gamemodeFlashlightRange / 2), unlerped);
                            }
                            return false;
                        }
                        else if (gamemodePlayer != null && PoliceAndThief.policeplayer03 != null && gamemodePlayer == PoliceAndThief.policeplayer03 && PoliceAndThief.policeplayer03.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.policeplayer03)
                        {
                            if (PoliceAndThief.policeplayer03lightTimer > 0f)
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * MapSettings.gamemodeFlashlightRange, __instance.MaxLightRadius * MapSettings.gamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.gamemodeFlashlightRange / 2), unlerped);
                            }
                            return false;
                        }
                        else if (gamemodePlayer != null && PoliceAndThief.policeplayer02 != null && gamemodePlayer == PoliceAndThief.policeplayer02 && PoliceAndThief.policeplayer02.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.policeplayer02)
                        {
                            if (PoliceAndThief.policeplayer02lightTimer > 0f)
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange), __instance.MaxLightRadius * MapSettings.gamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.gamemodeFlashlightRange / 2), unlerped);
                            }
                            return false;
                        }
                        else if (gamemodePlayer != null && PoliceAndThief.policeplayer05 != null && gamemodePlayer == PoliceAndThief.policeplayer05 && PoliceAndThief.policeplayer05.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.policeplayer05)
                        {
                            if (PoliceAndThief.policeplayer05lightTimer > 0f)
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange), __instance.MaxLightRadius * MapSettings.gamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.gamemodeFlashlightRange / 2), unlerped);
                            }
                            return false;
                        }
                        else if (gamemodePlayer != null && PoliceAndThief.policeplayer04 != null && gamemodePlayer == PoliceAndThief.policeplayer04 && PoliceAndThief.policeplayer04.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.policeplayer04)
                        {
                            if (PoliceAndThief.policeplayer04lightTimer > 0f)
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange), __instance.MaxLightRadius * MapSettings.gamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.gamemodeFlashlightRange / 2), unlerped);
                            }
                            return false;
                        }
                        else if (gamemodePlayer != null && PoliceAndThief.policeplayer06 != null && gamemodePlayer == PoliceAndThief.policeplayer06 && PoliceAndThief.policeplayer06.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.policeplayer06)
                        {
                            if (PoliceAndThief.policeplayer06lightTimer > 0f)
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange), __instance.MaxLightRadius * MapSettings.gamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.gamemodeFlashlightRange / 2), unlerped);
                            }
                            return false;
                        }
                        else
                        {
                            float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, unlerped) * GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.CrewLightMod);
                        }
                    }
                }
                return false;

            case CustomGameMode.HotPotato:
                if (player == null || player.IsDead)
                {
                    // IsDead
                    __result = __instance.MaxLightRadius;
                }
                else
                {
                    foreach (PlayerControl gamemodePlayer in PlayerControl.AllPlayerControls)
                    {
                        if (gamemodePlayer != null && HotPotato.hotPotatoPlayer != null && gamemodePlayer == HotPotato.hotPotatoPlayer && HotPotato.hotPotatoPlayer.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == HotPotato.hotPotatoPlayer)
                        {
                            float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius * (MapSettings.gamemodeFlashlightRange / 2), unlerped) * GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.CrewLightMod);
                        }
                        else
                            if (gamemodePlayer != null && gamemodePlayer.PlayerId == player.PlayerId)
                            {
                                float unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.gamemodeFlashlightRange / 1.5f), __instance.MaxLightRadius * (MapSettings.gamemodeFlashlightRange / 1.5f), unlerped);
                            }
                    }
                }
                return false;
        }

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
        UpdateCachedSystems(shipStatus);
        if (_cachedSwitchSystem != null)
        {
            lerpValue = _cachedSwitchSystem.Value / 255f;
        }

        return Mathf.Lerp(shipStatus.MinLightRadius, shipStatus.MaxLightRadius, lerpValue) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
    }

    public static void IsGameOverDueToDeath(ref bool __result)
    {
        __result = false;
    }

    private static int OriginalNumCommonTasksOption = 0;
    private static int OriginalNumShortTasksOption = 0;
    private static int OriginalNumLongTasksOption = 0;

    public static bool BeginPrefix(ShipStatus __instance)
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

    public static void BeginPostfix(ShipStatus __instance)
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

    public static void StartPostfix()
    {
        Logger.LogInfo("Game Started", "Phase");
    }

    public static void SpawnPlayer(ShipStatus __instance, PlayerControl player, int numPlayers, bool initialSpawn)
    {
        // Polusの湧き位置をランダムにする
        if (Helpers.IsPolus && CustomOptionHolder.PolusRandomSpawn.GetBool())
        {
            if (AmongUsClient.Instance.AmHost && player.Data != null)
            {
                byte randVal = (byte)RebuildUs.Instance.Rnd.Next(0, 6);
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.PolusRandomSpawn);
                sender.Write(player.Data.PlayerId);
                sender.Write(randVal);
                RPCProcedure.PolusRandomSpawn(player.Data.PlayerId, randVal);
            }
        }

        CustomButton.StopCountdown = false;
    }
}