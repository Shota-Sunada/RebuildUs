namespace RebuildUs.Modules.RPC;

/// <summary>
/// Handles RPCs that require manual serialization (complex MessageReader patterns, targeted sends, etc.).
/// All other RPCs are handled automatically via Reactor's [MethodRpc] system.
/// </summary>
internal static partial class RPCProcedure
{
    internal static void HandleManualRpc(CustomRPC callId, MessageReader reader)
    {
        switch (callId)
        {
            case CustomRPC.ShareOptions:
                ShareOptions(reader);
                break;
            case CustomRPC.WorkaroundSetRoles:
                WorkaroundSetRoles(reader.ReadByte(), reader);
                break;
            case CustomRPC.VersionHandshake:
                {
                    var major = reader.ReadByte();
                    var minor = reader.ReadByte();
                    var patch = reader.ReadByte();
                    var versionOwnerId = reader.ReadPackedInt32();
                    var revRaw = reader.ReadByte();
                    byte[] guidBytes = reader.ReadBytes(16);
                    var rev = revRaw == 0xFF ? -1 : revRaw;

                    var isNewToMe = !GameStart.PlayerVersions.ContainsKey(versionOwnerId);
                    VersionHandshake(major, minor, patch, rev, versionOwnerId, new(guidBytes));

                    if (versionOwnerId != AmongUsClient.Instance.ClientId)
                    {
                        if (isNewToMe || AmongUsClient.Instance.AmHost)
                        {
                            Helpers.ShareGameVersion(versionOwnerId);
                        }
                    }
                }
                break;
            case CustomRPC.ShareRealTasks:
                ShareRealTasks(reader);
                break;
        }
    }
}