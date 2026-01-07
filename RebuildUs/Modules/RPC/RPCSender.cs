namespace RebuildUs.Modules.RPC;

public class RPCSender(uint netId, CustomRPC callId, int targetId = -1) : IDisposable
{
    // Send RPC to player with netId
    private readonly MessageWriter Writer = AmongUsClient.Instance.StartRpcImmediately(netId, (byte)callId, SendOption.Reliable, targetId);

    public void Dispose()
    {
        AmongUsClient.Instance.FinishRpcImmediately(Writer);
    }

    public void Write(bool value)
    {
        Writer.Write(value);
    }

    public void Write(byte value)
    {
        Writer.Write(value);
    }

    public void Write(uint value, bool isPacked = false)
    {
        if (isPacked)
        {
            Writer.WritePacked(value);
        }
        else
        {
            Writer.Write(value);
        }
    }

    public void Write(int value, bool isPacked = false)
    {
        if (isPacked)
        {
            Writer.WritePacked(value);
        }
        else
        {
            Writer.Write(value);
        }
    }

    public void Write(float value)
    {
        Writer.Write(value);
    }

    public void Write(string value)
    {
        Writer.Write(value);
    }

    public void WriteBytesAndSize(Il2CppStructArray<byte> bytes)
    {
        Writer.WriteBytesAndSize(bytes);
    }

    public void WritePacked(int value)
    {
        Writer.WritePacked(value);
    }

    public void WritePacked(uint value)
    {
        Writer.WritePacked(value);
    }
}