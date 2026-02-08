namespace RebuildUs.Modules;

public static class Ship
{
    private static SwitchSystem _cachedSwitchSystem;
    private static ShipStatus _lastShipStatus;

    private static int _originalNumCommonTasksOption;
    private static int _originalNumShortTasksOption;
    private static int _originalNumLongTasksOption;

    public static void UpdateCachedSystems(ShipStatus instance)
    {
        if (_lastShipStatus == instance && _cachedSwitchSystem != null) return;
        _lastShipStatus = instance;
        _cachedSwitchSystem = null;
        if (instance != null && instance.Systems != null && instance.Systems.TryGetValue(SystemTypes.Electrical, out var system)) _cachedSwitchSystem = system.CastFast<SwitchSystem>();
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
                        lerpValue = Mathf.Clamp01((Trickster.LightsOutDuration - Trickster.LightsOutTimer) * 2);
                    else if (Trickster.LightsOutTimer < 0.5) lerpValue = Mathf.Clamp01(Trickster.LightsOutTimer * 2);

                    __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1 - lerpValue) * Helpers.GetOption(FloatOptionNames.CrewLightMod);
                    return false;
                }

                // Default light radius

                __result = GetNeutralLightRadius(__instance, false);
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
                    foreach (var gamemodePlayer in PlayerControl.AllPlayerControls)
                    {
                        if (gamemodePlayer != null && gamemodePlayer.PlayerId == player.PlayerId)
                        {
                            var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
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
                    foreach (var gamemodePlayer in PlayerControl.AllPlayerControls)
                    {
                        if (gamemodePlayer != null && PoliceAndThief.Policeplayer01 != null && gamemodePlayer == PoliceAndThief.Policeplayer01 && PoliceAndThief.Policeplayer01.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer01)
                        {
                            if (PoliceAndThief.Policeplayer01LightTimer > 0f)
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * MapSettings.GamemodeFlashlightRange, __instance.MaxLightRadius * MapSettings.GamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.GamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.GamemodeFlashlightRange / 2), unlerped);
                            }

                            return false;
                        }

                        if (gamemodePlayer != null && PoliceAndThief.Policeplayer03 != null && gamemodePlayer == PoliceAndThief.Policeplayer03 && PoliceAndThief.Policeplayer03.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer03)
                        {
                            if (PoliceAndThief.Policeplayer03LightTimer > 0f)
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * MapSettings.GamemodeFlashlightRange, __instance.MaxLightRadius * MapSettings.GamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.GamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.GamemodeFlashlightRange / 2), unlerped);
                            }

                            return false;
                        }

                        if (gamemodePlayer != null && PoliceAndThief.Policeplayer02 != null && gamemodePlayer == PoliceAndThief.Policeplayer02 && PoliceAndThief.Policeplayer02.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer02)
                        {
                            if (PoliceAndThief.Policeplayer02LightTimer > 0f)
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * MapSettings.GamemodeFlashlightRange, __instance.MaxLightRadius * MapSettings.GamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.GamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.GamemodeFlashlightRange / 2), unlerped);
                            }

                            return false;
                        }

                        if (gamemodePlayer != null && PoliceAndThief.Policeplayer05 != null && gamemodePlayer == PoliceAndThief.Policeplayer05 && PoliceAndThief.Policeplayer05.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer05)
                        {
                            if (PoliceAndThief.Policeplayer05LightTimer > 0f)
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * MapSettings.GamemodeFlashlightRange, __instance.MaxLightRadius * MapSettings.GamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.GamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.GamemodeFlashlightRange / 2), unlerped);
                            }

                            return false;
                        }

                        if (gamemodePlayer != null && PoliceAndThief.Policeplayer04 != null && gamemodePlayer == PoliceAndThief.Policeplayer04 && PoliceAndThief.Policeplayer04.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer04)
                        {
                            if (PoliceAndThief.Policeplayer04LightTimer > 0f)
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * MapSettings.GamemodeFlashlightRange, __instance.MaxLightRadius * MapSettings.GamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.GamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.GamemodeFlashlightRange / 2), unlerped);
                            }

                            return false;
                        }

                        if (gamemodePlayer != null && PoliceAndThief.Policeplayer06 != null && gamemodePlayer == PoliceAndThief.Policeplayer06 && PoliceAndThief.Policeplayer06.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == PoliceAndThief.Policeplayer06)
                        {
                            if (PoliceAndThief.Policeplayer06LightTimer > 0f)
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * MapSettings.GamemodeFlashlightRange, __instance.MaxLightRadius * MapSettings.GamemodeFlashlightRange, unlerped);
                            }
                            else
                            {
                                var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                                __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.GamemodeFlashlightRange / 2), __instance.MaxLightRadius * (MapSettings.GamemodeFlashlightRange / 2), unlerped);
                            }

                            return false;
                        }

                        {
                            var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
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
                    foreach (var gamemodePlayer in PlayerControl.AllPlayerControls)
                    {
                        if (gamemodePlayer != null && HotPotato.HotPotatoPlayer != null && gamemodePlayer == HotPotato.HotPotatoPlayer && HotPotato.HotPotatoPlayer.PlayerId == player.PlayerId && PlayerControl.LocalPlayer == HotPotato.HotPotatoPlayer)
                        {
                            var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius * (MapSettings.GamemodeFlashlightRange / 2), unlerped) * GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.CrewLightMod);
                        }
                        else if (gamemodePlayer != null && gamemodePlayer.PlayerId == player.PlayerId)
                        {
                            var unlerped = Mathf.InverseLerp(__instance.MinLightRadius, __instance.MaxLightRadius, GetNeutralLightRadius(__instance, false));
                            __result = Mathf.Lerp(__instance.MinLightRadius * (MapSettings.GamemodeFlashlightRange / 1.5f), __instance.MaxLightRadius * (MapSettings.GamemodeFlashlightRange / 1.5f), unlerped);
                        }
                    }
                }

                return false;
        }

        return false;
    }

    public static float GetNeutralLightRadius(ShipStatus shipStatus, bool isImpostor)
    {
        if (SubmergedCompatibility.IsSubmerged) return SubmergedCompatibility.GetSubmergedNeutralLightRadius(isImpostor);

        if (isImpostor) return shipStatus.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;

        var lerpValue = 1.0f;
        UpdateCachedSystems(shipStatus);
        if (_cachedSwitchSystem != null) lerpValue = _cachedSwitchSystem.Value / 255f;

        return Mathf.Lerp(shipStatus.MinLightRadius, shipStatus.MaxLightRadius, lerpValue) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
    }

    public static void IsGameOverDueToDeath(ref bool __result)
    {
        __result = false;
    }

    public static bool BeginPrefix(ShipStatus __instance)
    {
        var commonTaskCount = __instance.CommonTasks.Count;
        var normalTaskCount = __instance.ShortTasks.Count;
        var longTaskCount = __instance.LongTasks.Count;

        _originalNumCommonTasksOption = Helpers.GetOption(Int32OptionNames.NumCommonTasks);
        _originalNumShortTasksOption = Helpers.GetOption(Int32OptionNames.NumShortTasks);
        _originalNumLongTasksOption = Helpers.GetOption(Int32OptionNames.NumLongTasks);

        if (Helpers.GetOption(Int32OptionNames.NumCommonTasks) > commonTaskCount) Helpers.SetOption(Int32OptionNames.NumCommonTasks, commonTaskCount);
        if (Helpers.GetOption(Int32OptionNames.NumShortTasks) > normalTaskCount) Helpers.SetOption(Int32OptionNames.NumShortTasks, normalTaskCount);
        if (Helpers.GetOption(Int32OptionNames.NumLongTasks) > longTaskCount) Helpers.SetOption(Int32OptionNames.NumLongTasks, longTaskCount);

        return true;
    }

    public static void BeginPostfix(ShipStatus __instance)
    {
        // Restore original settings after the tasks have been selected
        Helpers.SetOption(Int32OptionNames.NumCommonTasks, _originalNumCommonTasksOption);
        Helpers.SetOption(Int32OptionNames.NumShortTasks, _originalNumShortTasksOption);
        Helpers.SetOption(Int32OptionNames.NumLongTasks, _originalNumLongTasksOption);

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
                var randVal = (byte)RebuildUs.Instance.Rnd.Next(0, 6);
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.PolusRandomSpawn);
                sender.Write(player.Data.PlayerId);
                sender.Write(randVal);
                RPCProcedure.PolusRandomSpawn(player.Data.PlayerId, randVal);
            }
        }

        CustomButton.StopCountdown = false;
    }
}
