using RebuildUs.Modules;

namespace RebuildUs.Modules.RPC;

public static class RPCProcedure
{
    public static void Handle(CustomRPC callId, MessageReader reader)
    {
        switch (callId)
        {
            case CustomRPC.ShareOptions:
                HandleShareOptions(reader);
                break;
            default:
                break;
        }
    }

    private static void HandleShareOptions(MessageReader reader)
    {
        byte amount = reader.ReadByte();
        for (int i = 0; i < amount; i++)
        {
            uint id = reader.ReadPackedUInt32();
            uint selection = reader.ReadPackedUInt32();
            var option = CustomOption.AllOptions.FirstOrDefault(x => x.Id == (int)id);
            option?.UpdateSelection((int)selection);
        }
    }
}