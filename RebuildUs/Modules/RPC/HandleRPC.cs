namespace RebuildUs.Modules.RPC;

internal static partial class RPCProcedure
{
    internal static void Handle(CustomRPC callId, MessageReader reader)
    {
        if (callId < CustomRPC.ResetVariables) return;

        // Logger.LogInfo($"RPC Called: {(byte)callId}. {Enum.GetName(callId)}");

        switch (callId)
        {
            case CustomRPC.ResetVariables:
                ResetVariables();
                break;
            case CustomRPC.ShareOptions:
                ShareOptions(reader);
                break;
            case CustomRPC.WorkaroundSetRoles:
                WorkaroundSetRoles(reader.ReadByte(), reader);
                break;
            case CustomRPC.SetRole:
                SetRole(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.AddModifier:
                AddModifier(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.VersionHandshake:
                var major = reader.ReadByte();
                var minor = reader.ReadByte();
                var patch = reader.ReadByte();
                var versionOwnerId = reader.ReadPackedInt32();
                var revRaw = reader.ReadByte();
                byte[] guidBytes = reader.ReadBytes(16);
                var rev = revRaw == 0xFF ? -1 : revRaw;

                var isNewToMe = !GameStart.PlayerVersions.ContainsKey(versionOwnerId);
                VersionHandshake(major, minor, patch, rev, versionOwnerId, new(guidBytes));

                // If it's a new player to me, or I am host, send my version back
                if (versionOwnerId != AmongUsClient.Instance.ClientId)
                {
                    if (isNewToMe || AmongUsClient.Instance.AmHost)
                        Helpers.ShareGameVersion(versionOwnerId);
                }
                break;
            case CustomRPC.UseUncheckedVent:
                UseUncheckedVent(reader.ReadPackedInt32(), reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.UncheckedMurderPlayer:
                UncheckedMurderPlayer(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.UncheckedExilePlayer:
                UncheckedExilePlayer(reader.ReadByte());
                break;
            case CustomRPC.UncheckedCmdReportDeadBody:
                UncheckedCmdReportDeadBody(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.DynamicMapOption:
                DynamicMapOption(reader.ReadByte());
                break;
            case CustomRPC.SetGameStarting:
                SetGameStarting();
                break;
            case CustomRPC.StopStart:
                StopStart();
                break;
            case CustomRPC.StopStartSound:
                StopStartSound();
                break;
            case CustomRPC.ShareGamemode:
                ShareGamemode(reader.ReadByte());
                break;
            case CustomRPC.FinishResetVariables:
                FinishResetVariables(reader.ReadByte());
                break;
            case CustomRPC.FinishSetRole:
                FinishSetRole();
                break;
            case CustomRPC.SetLovers:
                SetLovers(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.OverrideNativeRole:
                OverrideNativeRole(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.UncheckedEndGame:
                UncheckedEndGame(reader.ReadByte(), reader.ReadBoolean());
                break;
            case CustomRPC.UncheckedSetTasks:
                UncheckedSetTasks(reader.ReadByte(), reader.ReadBytesAndSize());
                break;
            case CustomRPC.FinishShipStatusBegin:
                FinishShipStatusBegin();
                break;
            case CustomRPC.EngineerFixLights:
                EngineerFixLights();
                break;
            case CustomRPC.EngineerFixSubmergedOxygen:
                EngineerFixSubmergedOxygen();
                break;
            case CustomRPC.EngineerUsedRepair:
                EngineerUsedRepair(reader.ReadByte());
                break;
            case CustomRPC.ArsonistDouse:
                ArsonistDouse(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.ArsonistWin:
                ArsonistWin(reader.ReadByte());
                break;
            case CustomRPC.CleanBody:
                CleanBody(reader.ReadByte());
                break;
            case CustomRPC.VultureWin:
                VultureWin();
                break;
            case CustomRPC.VultureEat:
                VultureEat(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.ErasePlayerRoles:
                ErasePlayerRoles(reader.ReadByte());
                break;
            case CustomRPC.JackalCreatesSidekick:
                JackalCreatesSidekick(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.SidekickPromotes:
                SidekickPromotes(reader.ReadByte());
                break;
            case CustomRPC.MedicSetShielded:
                MedicSetShielded(reader.ReadByte());
                break;
            case CustomRPC.ShieldedMurderAttempt:
                ShieldedMurderAttempt();
                break;
            case CustomRPC.SetFutureShielded:
                SetFutureShielded(reader.ReadByte());
                break;
            case CustomRPC.TimeMasterRewindTime:
                TimeMasterRewindTime();
                break;
            case CustomRPC.TimeMasterShield:
                TimeMasterShield();
                break;
            case CustomRPC.GuesserShoot:
                GuesserShoot(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.PlaceJackInTheBox:
                PlaceJackInTheBox(reader.ReadBytesAndSize());
                break;
            case CustomRPC.LightsOut:
                LightsOut();
                break;
            case CustomRPC.EvilHackerCreatesMadmate:
                EvilHackerCreatesMadmate(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.UseAdminTime:
                UseAdminTime(reader.ReadSingle());
                break;
            case CustomRPC.UseCameraTime:
                UseCameraTime(reader.ReadSingle());
                break;
            case CustomRPC.UseVitalsTime:
                UseVitalsTime(reader.ReadSingle());
                break;
            case CustomRPC.TrackerUsedTracker:
                TrackerUsedTracker(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.SetFutureErased:
                SetFutureErased(reader.ReadByte());
                break;
            case CustomRPC.VampireSetBitten:
                VampireSetBitten(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.ShareRealTasks:
                ShareRealTasks(reader);
                break;
            case CustomRPC.PolusRandomSpawn:
                PolusRandomSpawn(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.Synchronize:
                Synchronize(reader.ReadByte(), reader.ReadInt32());
                break;
            case CustomRPC.PlaceCamera:
                PlaceCamera(reader.ReadSingle(), reader.ReadSingle(), reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.SealVent:
                SealVent(reader.ReadPackedInt32(), reader.ReadByte());
                break;
            case CustomRPC.MorphingMorph:
                MorphingMorph(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.CamouflagerCamouflage:
                CamouflagerCamouflage();
                break;
            case CustomRPC.SwapperSwap:
                SwapperSwap(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.SwapperAnimate:
                SwapperAnimate();
                break;
            case CustomRPC.SetFutureSpelled:
                SetFutureSpelled(reader.ReadByte());
                break;
            case CustomRPC.WitchSpellCast:
                WitchSpellCast(reader.ReadByte());
                break;
            case CustomRPC.PlaceGarlic:
                PlaceGarlic(reader.ReadSingle(), reader.ReadSingle());
                break;
            case CustomRPC.ImpostorPromotesToLastImpostor:
                ImpostorPromotesToLastImpostor(reader.ReadByte());
                break;
            case CustomRPC.ShifterShift:
                ShifterShift(reader.ReadByte());
                break;
            case CustomRPC.SetFutureShifted:
                SetFutureShifted(reader.ReadByte());
                break;
            case CustomRPC.SetShifterType:
                SetShifterType(reader.ReadBoolean());
                break;
            case CustomRPC.FortuneTellerUsedDivine:
                FortuneTellerUsedDivine(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.SheriffKill:
                SheriffKill(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean());
                break;
            case CustomRPC.SheriffKillRequest:
                SheriffKillRequest(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.GamemodeKills:
                var gamemodeTarget = reader.ReadByte();
                var gamemodeSource = reader.ReadByte();
                GamemodeKills(gamemodeTarget, gamemodeSource);
                break;
            case CustomRPC.CaptureTheFlagWhoTookTheFlag:
                var bluePlayerWhoHasRedFlag = reader.ReadByte();
                var redorblue = reader.ReadByte();
                CaptureTheFlagWhoTookTheFlag(bluePlayerWhoHasRedFlag, redorblue);
                break;
            case CustomRPC.CaptureTheFlagWhichTeamScored:
                var whichteam = reader.ReadByte();
                CaptureTheFlagWhichTeamScored(whichteam);
                break;
            case CustomRPC.PoliceandThiefJail:
                var thiefId = reader.ReadByte();
                PoliceandThiefJail(thiefId);
                break;
            case CustomRPC.PoliceandThiefFreeThief:
                PoliceandThiefFreeThief();
                break;
            case CustomRPC.PoliceandThiefTakeJewel:
                var thiefwhotookjewel = reader.ReadByte();
                var jewelTakeId = reader.ReadByte();
                PoliceandThiefTakeJewel(thiefwhotookjewel, jewelTakeId);
                break;
            case CustomRPC.PoliceandThiefDeliverJewel:
                var thiefwhodeliverjewel = reader.ReadByte();
                var jewelDeliverId = reader.ReadByte();
                PoliceandThiefDeliverJewel(thiefwhodeliverjewel, jewelDeliverId);
                break;
            case CustomRPC.PoliceandThiefRevertedJewelPosition:
                var thiefWhoLostJewel = reader.ReadByte();
                var jewelRevertedId = reader.ReadByte();
                PoliceandThiefRevertedJewelPosition(thiefWhoLostJewel, jewelRevertedId);
                break;
            case CustomRPC.PoliceandThiefsTased:
                PoliceandThiefsTased(reader.ReadByte());
                break;
            case CustomRPC.HotPotatoTransfer:
                HotPotatoTransfer(reader.ReadByte());
                break;
            case CustomRPC.HotPotatoExploded:
                HotPotatoExploded();
                break;
            case CustomRPC.BattleRoyaleShowShoots:
                var playerWhoShot = reader.ReadByte();
                var color = reader.ReadByte();
                var angle = reader.ReadSingle();
                BattleRoyaleShowShoots(playerWhoShot, color, angle);
                break;
        }
    }
}
