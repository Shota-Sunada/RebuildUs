namespace RebuildUs.Modules.RPC;

public static partial class RPCProcedure
{
    public static void Handle(CustomRPC callId, MessageReader reader)
    {
        switch (callId)
        {
            case CustomRPC.ResetVariables:
                resetVariables();
                break;
            case CustomRPC.ShareOptions:
                ShareOptions(reader);
                break;
            case CustomRPC.WorkaroundSetRoles:
                workaroundSetRoles(reader.ReadByte(), reader);
                break;
            case CustomRPC.SetRole:
                setRole(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.AddModifier:
                addModifier(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.VersionHandshake:
                byte major = reader.ReadByte();
                byte minor = reader.ReadByte();
                byte patch = reader.ReadByte();
                float timer = reader.ReadSingle();
                if (!AmongUsClient.Instance.AmHost && timer >= 0f) GameStartManagerPatch.timer = timer;
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
                versionHandshake(major, minor, patch, revision == 0xFF ? -1 : revision, guid, versionOwnerId);
                break;
            case CustomRPC.UseUncheckedVent:
                useUncheckedVent(reader.ReadPackedInt32(), reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.UncheckedMurderPlayer:
                uncheckedMurderPlayer(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.UncheckedExilePlayer:
                uncheckedExilePlayer(reader.ReadByte());
                break;
            case CustomRPC.UncheckedCmdReportDeadBody:
                uncheckedCmdReportDeadBody(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.DynamicMapOption:
                dynamicMapOption(reader.ReadByte());
                break;
            case CustomRPC.SetGameStarting:
                setGameStarting();
                break;
            case CustomRPC.ShareGamemode:
                shareGamemode(reader.ReadByte());
                break;
            case CustomRPC.FinishResetVariables:
                finishResetVariables(reader.ReadByte());
                break;
            case CustomRPC.FinishSetRole:
                finishSetRole();
                break;
            case CustomRPC.SetLovers:
                setLovers(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.OverrideNativeRole:
                overrideNativeRole(reader.ReadByte(), reader.ReadByte());
                break;
            case CustomRPC.UncheckedEndGame:
                uncheckedEndGame(reader.ReadByte());
                break;
            case CustomRPC.UncheckedSetTasks:
                UncheckedSetTasks(reader.ReadByte(), reader.ReadBytesAndSize());
                break;
            case CustomRPC.FinishShipStatusBegin:
                finishShipStatusBegin();
                break;
            case CustomRPC.EngineerFixLights:
                engineerFixLights();
                break;
            case CustomRPC.EngineerFixSubmergedOxygen:
                engineerFixSubmergedOxygen();
                break;
            case CustomRPC.EngineerUsedRepair:
                engineerUsedRepair();
                break;
            default:
                break;
        }
    }
}