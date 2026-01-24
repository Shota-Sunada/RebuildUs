namespace RebuildUs.Modules.RPC;

public static partial class RPCProcedure
{
    public static void Handle(CustomRPC callId, MessageReader reader)
    {
        if (callId < CustomRPC.ResetVariables) return;

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
                byte major = reader.ReadByte();
                byte minor = reader.ReadByte();
                byte patch = reader.ReadByte();
                int versionOwnerId = reader.ReadPackedInt32();
                byte revision = 0xFF;
                Guid guid;
                if (reader.Length - reader.Position >= 17)
                {
                    // enough bytes left to read
                    revision = reader.ReadByte();
                    // GUID
                    byte[] bytes = reader.ReadBytes(16);
                    guid = new Guid(bytes);
                }
                else
                {
                    guid = new Guid(new byte[16]);
                }
                VersionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, versionOwnerId);

                // If I am host, send my version back to the person who sent it
                if (AmongUsClient.Instance.AmHost && versionOwnerId != AmongUsClient.Instance.ClientId)
                {
                    Helpers.ShareGameVersion((byte)versionOwnerId);
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
                UncheckedEndGame(reader.ReadByte());
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
            default:
                break;
        }
    }
}