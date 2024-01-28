using MessagePack;
using MessagePack.Resolvers;

public class MsgPackSerializer
{
    MessagePackSerializerOptions serializerOptions;
    public MsgPackSerializer()
    {
        serializerOptions =
        MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray)
        .WithResolver(ContractlessStandardResolver.Instance);
    }

    public T DeSerialize<T>(byte[] inpuBytes)
    {
        return MessagePackSerializer.Deserialize<T>(inpuBytes, serializerOptions);
    }
}
