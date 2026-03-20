using Reactor.Networking.Attributes;
using Reactor.Networking.Serialization;

namespace RebuildUs.Modules.RPC;

[MessageConverter]
internal sealed class ByteArrayConverter : MessageConverter<byte[]>
{
    public override void Write(MessageWriter writer, byte[] value)
    {
        writer.WriteBytesAndSize(value);
    }

    public override byte[] Read(MessageReader reader, Type objectType)
    {
        return reader.ReadBytesAndSize();
    }
}