namespace RebuildUs.Modules.RPC;

internal sealed class RPCSender(uint netId, CustomRPC callId, int targetId = -1) : IDisposable
{
    // Send RPC to player with netId
    private readonly MessageWriter _writer = AmongUsClient.Instance.StartRpcImmediately(netId, (byte)callId, SendOption.Reliable, targetId);

    public void Dispose()
    {
        AmongUsClient.Instance.FinishRpcImmediately(_writer);
    }

    internal void Write(bool value)
    {
        _writer.Write(value);
    }

    internal void Write(byte value)
    {
        _writer.Write(value);
    }

    internal void Write(uint value, bool isPacked = false)
    {
        if (isPacked)
            _writer.WritePacked(value);
        else
            _writer.Write(value);
    }

    internal void Write(int value, bool isPacked = false)
    {
        if (isPacked)
            _writer.WritePacked(value);
        else
            _writer.Write(value);
    }

    internal void Write(float value)
    {
        _writer.Write(value);
    }

    internal void Write(string value)
    {
        _writer.Write(value);
    }

    internal void Write(Il2CppStructArray<byte> bytes)
    {
        _writer.Write(bytes);
    }

    internal void WriteBytesAndSize(Il2CppStructArray<byte> bytes)
    {
        _writer.WriteBytesAndSize(bytes);
    }

    internal void WritePacked(int value)
    {
        _writer.WritePacked(value);
    }

    internal void WritePacked(uint value)
    {
        _writer.WritePacked(value);
    }
}