using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;
using RebuildUs.Roles.Neutral;

namespace RebuildUs.Modules;

public static class Exile
{
    public static NetworkedPlayerInfo lastExiled;
    public static void BeginForGameplay(ExileController __instance, NetworkedPlayerInfo player, bool voteTie)
    {
        lastExiled = player;

        // Medic shield
        if (Medic.Exists && AmongUsClient.Instance.AmHost && Medic.futureShielded != null && Medic.LivingPlayers.Count != 0)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and setting the shield is correct(for that reason the futureShifted and futureShielded are being synced)
            using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.MedicSetShielded);
            sender.Write(Medic.futureShielded.PlayerId);
            RPCProcedure.medicSetShielded(Medic.futureShielded.PlayerId);
        }

        // Madmate exiled
        if (AmongUsClient.Instance.AmHost && player != null && ((CreatedMadmate.exileCrewmate && player.Object.HasModifier(ModifierType.CreatedMadmate))
            || (Madmate.exileCrewmate && player.Object.HasModifier(ModifierType.Madmate)))
        )
        {
            // pick random crewmate
            var target = pickRandomCrewmate(player.PlayerId);
            if (target != null)
            {
                // exile the picked crewmate
                using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.UncheckedExilePlayer);
                sender.Write(target.PlayerId);
                RPCProcedure.UncheckedExilePlayer(target.PlayerId);
            }
        }

        // Shifter shift
        if (Shifter.Exists && AmongUsClient.Instance.AmHost && Shifter.futureShift != null)
        {
            // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
            writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.ShifterShift, Hazel.SendOption.Reliable, -1);
            writer.Write(Shifter.futureShift.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.shifterShift(Shifter.futureShift.PlayerId);
        }
        Shifter.futureShift = null;

        // Eraser erase
        if (Eraser.Exists && AmongUsClient.Instance.AmHost && Eraser.futureErased != null)
        {  // We need to send the RPC from the host here, to make sure that the order of shifting and erasing is correct (for that reason the futureShifted and futureErased are being synced)
            foreach (PlayerControl target in Eraser.futureErased)
            {
                if (target != null && target.CanBeErased())
                {
                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.ErasePlayerRoles);
                    sender.Write(target.PlayerId);
                    RPCProcedure.ErasePlayerRoles(target.PlayerId);
                }
            }
        }
        Eraser.futureErased = [];

        // Trickster boxes
        if (Trickster.Exists && JackInTheBox.HasJackInTheBoxLimitReached())
        {
            JackInTheBox.ConvertToVents();
        }

        // Witch execute casted spells
        if (Witch.Exists && Witch.futureSpelled != null && AmongUsClient.Instance.AmHost)
        {
            bool exiledIsWitch = player != null && player.Object.IsRole(RoleType.Witch);
            bool witchDiesWithExiledLover = player != null && Lovers.bothDie && player.Object.IsLovers() && player.Object.getPartner().IsRole(RoleType.Witch);

            if ((witchDiesWithExiledLover || exiledIsWitch) && Witch.voteSavesTargets) Witch.futureSpelled = [];
            foreach (var target in Witch.futureSpelled)
            {
                if (target != null && !target.Data.IsDead && Helpers.CheckMurderAttempt(player.Object, target, true) == MurderAttemptResult.PerformKill)
                {
                    using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.WitchSpellCast);
                    sender.Write(target.PlayerId);
                    RPCProcedure.witchSpellCast(target.PlayerId);
                }
            }
        }
        Witch.futureSpelled = [];

        // SecurityGuard vents and cameras
        var allCameras = MapUtilities.CachedShipStatus.AllCameras.ToList();
        ModMapOptions.CamerasToAdd.ForEach(camera =>
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            allCameras.Add(camera);
        });
        MapUtilities.CachedShipStatus.AllCameras = allCameras.ToArray();
        ModMapOptions.CamerasToAdd = [];

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
        if (ModMapOptions.restrictDevices == 1)
        {
            ModMapOptions.resetDeviceTimes();
        }
    }

    private static PlayerControl pickRandomCrewmate(int exiledPlayerId)
    {
        int numAliveCrewmates = 0;
        // count alive crewmates
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
        {
            if (player.Data.Role.IsImpostor)
                continue;
            if (player.Data.IsDead)
                continue;
            if (player.PlayerId == exiledPlayerId)
                continue;
            numAliveCrewmates++;
        }
        // get random number range 0, num of alive crewmates
        int targetPlayerIndex = RebuildUs.Instance.Rnd.Next(0, numAliveCrewmates);
        int currentPlayerIndex = 0;
        // return the player
        foreach (PlayerControl player in CachedPlayer.AllPlayers)
        {
            if (player.Data.Role.IsImpostor)
                continue;
            if (player.Data.IsDead)
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
            var p = Helpers.PlayerById(exiled.PlayerId);
            if (p.HasModifier(ModifierType.Mini) && !Mini.isGrownUp(p) && !p.Data.Role.IsImpostor && !p.IsNeutral())
            {
                Mini.triggerMiniLose = true;
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
        if (CachedPlayer.LocalPlayer.PlayerControl.HasModifier(ModifierType.Mini) && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor)
        {
            var multiplier = Mini.isGrownUp(CachedPlayer.LocalPlayer.PlayerControl) ? 0.66f : 2f;
            CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(Helpers.GetOption(FloatOptionNames.KillCooldown) * multiplier);
        }

        // // Seer spawn souls
        // if (Seer.deadBodyPositions != null && Seer.seer != null && CachedPlayer.LocalPlayer.PlayerControl == Seer.seer && (Seer.mode == 0 || Seer.mode == 2))
        // {
        //     foreach (Vector3 pos in Seer.deadBodyPositions)
        //     {
        //         GameObject soul = new();
        //         // soul.transform.position = pos;
        //         soul.transform.position = new Vector3(pos.x, pos.y, pos.y / 1000 - 1f);
        //         soul.layer = 5;
        //         var rend = soul.AddComponent<SpriteRenderer>();
        //         soul.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        //         rend.sprite = Seer.getSoulSprite();

        //         if (Seer.limitSoulDuration)
        //         {
        //             FastDestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Lerp(Seer.soulDuration, new Action<float>((p) =>
        //             {
        //                 if (rend != null)
        //                 {
        //                     var tmp = rend.color;
        //                     tmp.a = Mathf.Clamp01(1 - p);
        //                     rend.color = tmp;
        //                 }
        //                 if (p == 1f && rend != null && rend.gameObject != null) UnityEngine.Object.Destroy(rend.gameObject);
        //             })));
        //         }
        //     }
        //     Seer.deadBodyPositions = new List<Vector3>();
        // }

        // // Tracker reset deadBodyPositions
        // Tracker.deadBodyPositions = new List<Vector3>();

        // // Arsonist deactivate dead poolable players
        // Arsonist.updateIcons();

        // // Force Bounty Hunter Bounty Update
        // if (BountyHunter.bountyHunter != null && BountyHunter.bountyHunter == CachedPlayer.LocalPlayer.PlayerControl)
        //     BountyHunter.bountyUpdateTimer = 0f;

        // // Medium spawn souls
        // if (Medium.medium != null && CachedPlayer.LocalPlayer.PlayerControl == Medium.medium)
        // {
        //     if (Medium.souls != null)
        //     {
        //         foreach (SpriteRenderer sr in Medium.souls) UnityEngine.Object.Destroy(sr.gameObject);
        //         Medium.souls = new List<SpriteRenderer>();
        //     }

        //     if (Medium.featureDeadBodies != null)
        //     {
        //         foreach ((DeadPlayer db, Vector3 ps) in Medium.featureDeadBodies)
        //         {
        //             GameObject s = new();
        //             // s.transform.position = ps;
        //             s.transform.position = new Vector3(ps.x, ps.y, ps.y / 1000 - 1f);
        //             s.layer = 5;
        //             var rend = s.AddComponent<SpriteRenderer>();
        //             s.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover);
        //             rend.sprite = Medium.getSoulSprite();
        //             Medium.souls.Add(rend);
        //         }
        //         Medium.deadBodies = Medium.featureDeadBodies;
        //         Medium.featureDeadBodies = new List<Tuple<DeadPlayer, Vector3>>();
        //     }
        // }

        // if (Lawyer.lawyer != null && CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer && !Lawyer.lawyer.Data.IsDead)
        //     Lawyer.meetings++;

        if (CachedPlayer.LocalPlayer.PlayerControl.HasModifier(ModifierType.AntiTeleport))
        {
            if (AntiTeleport.position != new Vector3())
            {
                CachedPlayer.LocalPlayer.PlayerControl.transform.position = AntiTeleport.position;
                if (SubmergedCompatibility.IsSubmerged)
                {
                    SubmergedCompatibility.ChangeFloor(AntiTeleport.position.y > -7);
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
        Map.resetRealTasks();

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
                PlayerControl player = ExileController.Instance.initData.networkedPlayer.Object;
                if (player == null) return;
                // Exile role text
                if (id == StringNames.ExileTextPN || id == StringNames.ExileTextSN || id == StringNames.ExileTextPP || id == StringNames.ExileTextSP)
                {
                    __result = player.Data.PlayerName + " was The " + string.Join(" ", [.. RoleInfo.GetRoleInfoForPlayer(player, false).Select(x => x.Name)]);
                }
                if (id == StringNames.ImpostorsRemainP || id == StringNames.ImpostorsRemainS)
                {
                    if (player.IsRole(RoleType.Jester)) __result = "";
                }
            }
        }
        catch
        {
            // pass - Hopefully prevent leaving while exiling to soft lock game
        }
    }
}