namespace RebuildUs.Modules.RPC;

public sealed class RPCSender(uint netId, CustomRPC callId, int targetId = -1) : IDisposable
{
    // Send RPC to player with netId
    private readonly MessageWriter _writer = AmongUsClient.Instance.StartRpcImmediately(netId, (byte)callId, SendOption.Reliable, targetId);

    public void Dispose()
    {
        AmongUsClient.Instance.FinishRpcImmediately(_writer);
    }

    public void Write(bool value)
    {
        _writer.Write(value);
    }

    public void Write(byte value)
    {
        _writer.Write(value);
    }

    public void Write(uint value, bool isPacked = false)
    {
        if (isPacked)
            _writer.WritePacked(value);
        else
            _writer.Write(value);
    }

    public void Write(int value, bool isPacked = false)
    {
        if (isPacked)
            _writer.WritePacked(value);
        else
            _writer.Write(value);
    }

    public void Write(float value)
    {
        _writer.Write(value);
    }

    public void Write(string value)
    {
        _writer.Write(value);
    }

    public void Write(Il2CppStructArray<byte> bytes)
    {
        _writer.Write(bytes);
    }

    public void WriteBytesAndSize(Il2CppStructArray<byte> bytes)
    {
        _writer.WriteBytesAndSize(bytes);
    }

    public void WritePacked(int value)
    {
        _writer.WritePacked(value);
    }

    public void WritePacked(uint value)
    {
        _writer.WritePacked(value);
    }
}
