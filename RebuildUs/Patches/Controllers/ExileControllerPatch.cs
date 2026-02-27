using PowerTools;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ExileControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal static void BeginForGameplayPrefix(ExileController __instance, NetworkedPlayerInfo player, bool voteTie)
    {
        if (player != null)
        {
            GameHistory.FinalStatuses[player.PlayerId] = FinalStatus.Exiled;
        }

        // Medic shield
        if (Medic.Exists && AmongUsClient.Instance.AmHost && Medic.FutureShielded != null && Medic.PlayerControl.IsAlive())
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
            using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.MedicSetShielded);
            sender.Write(Medic.FutureShielded.PlayerId);
            RPCProcedure.MedicSetShielded(Medic.FutureShielded.PlayerId);
        }

        // Madmate exiled
        if (AmongUsClient.Instance.AmHost
            && player?.Object != null
            && (CreatedMadmate.ExileCrewmate && player.Object.HasModifier(ModifierType.CreatedMadmate)
                || Madmate.ExileCrewmate && player.Object.HasModifier(ModifierType.Madmate)))
        {
            // pick random crewmate
            PlayerControl target = PickRandomCrewmate(player.PlayerId);
            if (target != null)
            {
                // exile the picked crewmate
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedExilePlayer);
                sender.Write(target.PlayerId);
                RPCProcedure.UncheckedExilePlayer(target.PlayerId);
            }
        }

        // Shifter shift
        if (Shifter.Exists && AmongUsClient.Instance.AmHost && Shifter.FutureShift != null)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
            using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ShifterShift)) sender.Write(Shifter.FutureShift.PlayerId);

            RPCProcedure.ShifterShift(Shifter.FutureShift.PlayerId);
        }

        Shifter.FutureShift = null;

        // Eraser erase
        if (Eraser.Exists && AmongUsClient.Instance.AmHost && Eraser.FutureErased != null)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
            foreach (PlayerControl target in Eraser.FutureErased)
            {
                if (target == null || !target.CanBeErased())
                {
                    continue;
                }
                using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.ErasePlayerRoles);
                sender.Write(target.PlayerId);
                RPCProcedure.ErasePlayerRoles(target.PlayerId);
            }
        }

        Eraser.FutureErased = [];

        // Trickster boxes
        if (Trickster.Exists && JackInTheBox.HasJackInTheBoxLimitReached())
        {
            JackInTheBox.ConvertToVents();
        }

        // Witch execute casted spells
        if (Witch.Exists && Witch.FutureSpelled != null && AmongUsClient.Instance.AmHost)
        {
            PlayerControl exiledPlayer = player?.Object;
            bool exiledIsWitch = exiledPlayer != null && exiledPlayer.IsRole(RoleType.Witch);
            bool witchDiesWithExiledLover = exiledPlayer != null
                                            && Lovers.BothDie
                                            && exiledPlayer.IsLovers()
                                            && exiledPlayer.GetPartner()?.IsRole(RoleType.Witch) == true;

            if ((witchDiesWithExiledLover || exiledIsWitch) && Witch.VoteSavesTargets)
            {
                Witch.FutureSpelled = [];
            }
            foreach (PlayerControl target in Witch.FutureSpelled)
            {
                if (target != null && !target.Data.IsDead)
                {
                    PlayerControl witchKiller = exiledIsWitch ? exiledPlayer : null;
                    if (witchKiller == null)
                    {
                        foreach (Witch w in Witch.Players)
                        {
                            if (w.Player == null || w.Player.Data.IsDead)
                            {
                                continue;
                            }
                            witchKiller = w.Player;
                            break;
                        }
                    }

                    if (witchKiller == null)
                    {
                        witchKiller = PlayerControl.LocalPlayer; // Fallback to host for shield check
                    }

                    if (Helpers.CheckMurderAttempt(witchKiller, target, true) != MurderAttemptResult.PerformKill)
                    {
                        continue;
                    }
                    using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.WitchSpellCast);
                    sender.Write(target.PlayerId);
                    RPCProcedure.WitchSpellCast(target.PlayerId);
                }
            }
        }

        Witch.FutureSpelled = [];

        // SecurityGuard vents and cameras
        ShipStatus ship = MapUtilities.CachedShipStatus;
        if (ship != null)
        {
            int oldLen = ship.AllCameras.Length;
            int addLen = MapSettings.CamerasToAdd.Count;
            if (addLen > 0)
            {
                SurvCamera[] newCameras = new SurvCamera[oldLen + addLen];
                for (int i = 0; i < oldLen; i++)
                {
                    newCameras[i] = ship.AllCameras[i];
                }
                for (int i = 0; i < addLen; i++)
                {
                    SurvCamera camera = MapSettings.CamerasToAdd[i];
                    camera.gameObject.SetActive(true);
                    camera.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    newCameras[oldLen + i] = camera;
                }

                ship.AllCameras = newCameras;
                MapSettings.CamerasToAdd.Clear();
            }
        }

        foreach (Vent vent in MapSettings.VentsToSeal)
        {
            SpriteAnim animator = vent.GetComponent<SpriteAnim>();
            vent.EnterVentAnim = vent.ExitVentAnim = null;
            if (Helpers.IsFungle)
            {
                vent.myRend = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                animator = vent.transform.GetChild(3).GetComponent<SpriteAnim>();
            }

            animator?.Stop();
            vent.EnterVentAnim = vent.ExitVentAnim = null;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0)
            {
                vent.myRend.sprite = AssetLoader.CentralUpperBlocked;
            }
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14)
            {
                vent.myRend.sprite = AssetLoader.CentralLowerBlocked;
            }
            vent.myRend.color = Color.white;
            vent.name = "SealedVent_" + vent.name;
        }

        MapSettings.VentsToSeal = [];

        // 1 = reset per turn
        if (MapSettings.RestrictDevices == 1)
        {
            MapSettings.ResetDeviceTimes();
        }
    }

    private static PlayerControl PickRandomCrewmate(int exiledPlayerId)
    {
        int numAliveCrewmates = 0;
        // count alive crewmates
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.IsTeamImpostor())
            {
                continue;
            }
            if (player.IsDead())
            {
                continue;
            }
            if (player.PlayerId == exiledPlayerId)
            {
                continue;
            }
            numAliveCrewmates++;
        }

        if (numAliveCrewmates == 0)
        {
            return null;
        }
        // get random number range 0, num of alive crewmates
        int targetPlayerIndex = RebuildUs.Rnd.Next(0, numAliveCrewmates);
        int currentPlayerIndex = 0;
        // return the player
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player.IsTeamImpostor())
            {
                continue;
            }
            if (player.IsDead())
            {
                continue;
            }
            if (player.PlayerId == exiledPlayerId)
            {
                continue;
            }
            if (currentPlayerIndex == targetPlayerIndex)
            {
                return player;
            }
            currentPlayerIndex++;
        }

        return null;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    internal static void WrapUpPostfix(ExileController __instance)
    {
        NetworkedPlayerInfo networkedPlayer = __instance.initData.networkedPlayer;
        WrapUpPostfix(networkedPlayer?.Object);
    }

    internal static void WrapUpPostfix(PlayerControl exiled)
    {
        CustomButton.MeetingEndedUpdate();

        if (exiled != null)
        {
            if (exiled.HasModifier(ModifierType.Mini) && !Mini.IsGrownUp(exiled) && !exiled.IsTeamImpostor() && !exiled.IsNeutral())
            {
                Mini.TriggerMiniLose = true;
            }

            if (exiled.IsRole(RoleType.Jester))
            {
                Jester.TriggerJesterWin = true;
            }
        }

        if (!SubmergedCompatibility.IsSubmerged)
        {
            return;
        }
        GameObject fullscreen = GameObject.Find("FullScreen500(Clone)");
        if (fullscreen)
        {
            fullscreen.SetActive(false);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.ReEnableGameplay))]
    internal static void ReEnableGameplay(ExileController __instance)
    {
        CustomButton.MeetingEndedUpdate();
        MapSettings.MeetingEndedUpdate();
        RebuildUs.OnMeetingEnd();

        // Mini set adapted cooldown
        if (PlayerControl.LocalPlayer.HasModifier(ModifierType.Mini) && PlayerControl.LocalPlayer.IsTeamImpostor())
        {
            float multiplier = Mini.IsGrownUp(PlayerControl.LocalPlayer) ? 0.66f : 2f;
            PlayerControl.LocalPlayer.SetKillTimer(Helpers.GetOption(FloatOptionNames.KillCooldown) * multiplier);
        }

        if (PlayerControl.LocalPlayer.HasModifier(ModifierType.AntiTeleport))
        {
            if (AntiTeleport.Position != new Vector3())
            {
                PlayerControl.LocalPlayer.transform.position = AntiTeleport.Position;
                if (SubmergedCompatibility.IsSubmerged)
                {
                    SubmergedCompatibility.ChangeFloor(AntiTeleport.Position.y > -7);
                }
            }
        }

        // Remove DeadBodies
        Il2CppArrayBase<DeadBody> array = UnityObject.FindObjectsOfType<DeadBody>();
        foreach (DeadBody t in array)
        {
            UnityObject.Destroy(t.gameObject);
        }

        // ベントバグ対策
        VentilationSystem vs = FastDestroyableSingleton<ShipStatus>.Instance.Systems[SystemTypes.Ventilation].TryCast<VentilationSystem>();
        vs?.PlayersInsideVents.Clear();

        // イビルトラッカーで他のプレイヤーのタスク情報を表示する
        Map.ResetRealTasks();

        int deadPlayers = 0;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.Data.IsDead)
            {
                deadPlayers += 1;
            }
        }

        if (deadPlayers < (int)CustomOptionHolder.AdditionalEmergencyCooldown.GetFloat())
        {
            MapUtilities.CachedShipStatus.EmergencyCooldown = Helpers.GetOption(Int32OptionNames.EmergencyCooldown)
                                                              + CustomOptionHolder.AdditionalEmergencyCooldownTime.GetFloat();
        }
    }
}