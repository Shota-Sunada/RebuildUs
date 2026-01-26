namespace RebuildUs.Modules;

public static class Exile
{
    public static NetworkedPlayerInfo LastExiled;
    private static readonly StringBuilder ExileStringBuilder = new();

    public static void BeginForGameplay(ExileController __instance, NetworkedPlayerInfo player, bool voteTie)
    {
        LastExiled = player;
        if (player != null)
        {
            DiscordModManager.OnExile(player.PlayerName);
            GameHistory.FinalStatuses[player.PlayerId] = FinalStatus.Exiled;
        }

        // Medic shield
        if (Medic.Exists && AmongUsClient.Instance.AmHost && Medic.FutureShielded != null && Medic.LivingPlayers.Count != 0)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
            using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.MedicSetShielded);
            sender.Write(Medic.FutureShielded.PlayerId);
            RPCProcedure.MedicSetShielded(Medic.FutureShielded.PlayerId);
        }

        // Madmate exiled
        if (AmongUsClient.Instance.AmHost && player?.Object != null && ((CreatedMadmate.ExileCrewmate && player.Object.HasModifier(ModifierType.CreatedMadmate))
            || (Madmate.ExileCrewmate && player.Object.HasModifier(ModifierType.Madmate)))
        )
        {
            // pick random crewmate
            var target = PickRandomCrewmate(player.PlayerId);
            if (target != null)
            {
                // exile the picked crewmate
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedExilePlayer);
                sender.Write(target.PlayerId);
                RPCProcedure.UncheckedExilePlayer(target.PlayerId);
            }
        }

        // Shifter shift
        if (Shifter.Exists && AmongUsClient.Instance.AmHost && Shifter.FutureShift != null)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
            using (var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ShifterShift))
            {
                sender.Write(Shifter.FutureShift.PlayerId);
            }
            RPCProcedure.ShifterShift(Shifter.FutureShift.PlayerId);
        }
        Shifter.FutureShift = null;

        // Eraser erase
        if (Eraser.Exists && AmongUsClient.Instance.AmHost && Eraser.FutureErased != null)
        {  // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
            foreach (PlayerControl target in Eraser.FutureErased)
            {
                if (target != null && target.CanBeErased())
                {
                    using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.ErasePlayerRoles);
                    sender.Write(target.PlayerId);
                    RPCProcedure.ErasePlayerRoles(target.PlayerId);
                }
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
            bool witchDiesWithExiledLover = exiledPlayer != null && Lovers.BothDie && exiledPlayer.IsLovers() && exiledPlayer.GetPartner()?.IsRole(RoleType.Witch) == true;

            if ((witchDiesWithExiledLover || exiledIsWitch) && Witch.VoteSavesTargets) Witch.FutureSpelled = [];
            foreach (var target in Witch.FutureSpelled)
            {
                if (target != null && !target.Data.IsDead)
                {
                    PlayerControl witchKiller = exiledIsWitch ? exiledPlayer : null;
                    if (witchKiller == null)
                    {
                        foreach (var w in Witch.Players)
                        {
                            if (w.Player != null && !w.Player.Data.IsDead)
                            {
                                witchKiller = w.Player;
                                break;
                            }
                        }
                    }
                    if (witchKiller == null) witchKiller = PlayerControl.LocalPlayer; // Fallback to host for shield check

                    if (Helpers.CheckMurderAttempt(witchKiller, target, true) == MurderAttemptResult.PerformKill)
                    {
                        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.WitchSpellCast);
                        sender.Write(target.PlayerId);
                        RPCProcedure.WitchSpellCast(target.PlayerId);
                    }
                }
            }
        }
        Witch.FutureSpelled = [];

        // SecurityGuard vents and cameras
        var ship = MapUtilities.CachedShipStatus;
        if (ship != null)
        {
            int oldLen = ship.AllCameras.Length;
            int addLen = ModMapOptions.CamerasToAdd.Count;
            if (addLen > 0)
            {
                var newCameras = new SurvCamera[oldLen + addLen];
                for (int i = 0; i < oldLen; i++) newCameras[i] = ship.AllCameras[i];
                for (int i = 0; i < addLen; i++)
                {
                    var camera = ModMapOptions.CamerasToAdd[i];
                    camera.gameObject.SetActive(true);
                    camera.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    newCameras[oldLen + i] = camera;
                }
                ship.AllCameras = newCameras;
                ModMapOptions.CamerasToAdd.Clear();
            }
        }

        foreach (var vent in ModMapOptions.VentsToSeal)
        {
            var animator = vent.GetComponent<PowerTools.SpriteAnim>();
            vent.EnterVentAnim = vent.ExitVentAnim = null;
            var newSprite = animator == null ? AssetLoader.StaticVentSealed : AssetLoader.AnimatedVentSealed;
            if (Helpers.IsFungle)
            {
                newSprite = AssetLoader.FungleVentSealed;
                vent.myRend = vent.transform.GetChild(3).GetComponent<SpriteRenderer>();
                animator = vent.transform.GetChild(3).GetComponent<PowerTools.SpriteAnim>();
            }
            animator?.Stop();
            vent.EnterVentAnim = vent.ExitVentAnim = null;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0) vent.myRend.sprite = AssetLoader.CentralUpperBlocked;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14) vent.myRend.sprite = AssetLoader.CentralLowerBlocked;
            vent.myRend.color = Color.white;
            vent.name = "SealedVent_" + vent.name;
        }
        ModMapOptions.VentsToSeal = [];

        // 1 = reset per turn
        if (ModMapOptions.RestrictDevices == 1)
        {
            ModMapOptions.ResetDeviceTimes();
        }
    }

    private static PlayerControl PickRandomCrewmate(int exiledPlayerId)
    {
        int numAliveCrewmates = 0;
        // count alive crewmates
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (player.IsTeamImpostor())
                continue;
            if (player.IsDead())
                continue;
            if (player.PlayerId == exiledPlayerId)
                continue;
            numAliveCrewmates++;
        }
        if (numAliveCrewmates == 0) return null;
        // get random number range 0, num of alive crewmates
        int targetPlayerIndex = RebuildUs.Instance.Rnd.Next(0, numAliveCrewmates);
        int currentPlayerIndex = 0;
        // return the player
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (player.IsTeamImpostor())
                continue;
            if (player.IsDead())
                continue;
            if (player.PlayerId == exiledPlayerId)
                continue;
            if (currentPlayerIndex == targetPlayerIndex)
                return player;
            currentPlayerIndex++;
        }
        return null;
    }

    public static void WrapUpPostfix(PlayerControl exiled)
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

        if (SubmergedCompatibility.IsSubmerged)
        {
            var fullscreen = GameObject.Find("FullScreen500(Clone)");
            if (fullscreen) fullscreen.SetActive(false);
        }
    }

    public static void ReEnableGameplay()
    {
        CustomButton.MeetingEndedUpdate();
        ModMapOptions.MeetingEndedUpdate();
        RebuildUs.OnMeetingEnd();

        // Mini set adapted cooldown
        if (PlayerControl.LocalPlayer.HasModifier(ModifierType.Mini) && PlayerControl.LocalPlayer.IsTeamImpostor())
        {
            var multiplier = Mini.IsGrownUp(PlayerControl.LocalPlayer) ? 0.66f : 2f;
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
        var array = UnityEngine.Object.FindObjectsOfType<DeadBody>();
        for (int i = 0; i < array.Length; i++)
        {
            UnityEngine.Object.Destroy(array[i].gameObject);
        }

        // ベントバグ対策
        var vs = FastDestroyableSingleton<ShipStatus>.Instance.Systems[SystemTypes.Ventilation].TryCast<VentilationSystem>();
        vs.PlayersInsideVents.Clear();

        // イビルトラッカーで他のプレイヤーのタスク情報を表示する
        Map.ResetRealTasks();

        int deadPlayers = 0;
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p.Data.IsDead) deadPlayers += 1;
        }
        if (deadPlayers < (int)CustomOptionHolder.AdditionalEmergencyCooldown.GetFloat())
        {
            ShipStatus.Instance.EmergencyCooldown = Helpers.GetOption(Int32OptionNames.EmergencyCooldown) + CustomOptionHolder.AdditionalEmergencyCooldownTime.GetFloat();
        }
    }

    public static void ExileMessage(ref string __result, StringNames id)
    {
        try
        {
            if (ExileController.Instance != null && ExileController.Instance.initData != null)
            {
                var netPlayer = ExileController.Instance.initData.networkedPlayer;
                if (netPlayer == null) return;
                PlayerControl player = netPlayer.Object;
                if (player == null) return;

                // Exile role text
                if (id == StringNames.ExileTextPN || id == StringNames.ExileTextSN || id == StringNames.ExileTextPP || id == StringNames.ExileTextSP)
                {
                    ExileStringBuilder.Clear();
                    ExileStringBuilder.Append(player.Data.PlayerName).Append(" was The ");
                    var roleInfos = RoleInfo.GetRoleInfoForPlayer(player, false);
                    for (int i = 0; i < roleInfos.Count; i++)
                    {
                        if (i > 0) ExileStringBuilder.Append(' ');
                        ExileStringBuilder.Append(roleInfos[i].Name);
                    }
                    __result = ExileStringBuilder.ToString();
                }
                if (id == StringNames.ImpostorsRemainP || id == StringNames.ImpostorsRemainS)
                {
                    if (player.IsRole(RoleType.Jester)) __result = string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "ExileMessage");
        }
    }
}