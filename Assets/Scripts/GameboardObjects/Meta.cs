using Unity.Netcode;

public struct Meta : INetworkSerializable {
    public NetworkString title;
    public NetworkString description;

    public Meta(string title, string description) {
        this.title = title;
        this.description = description;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref title);
        serializer.SerializeValue(ref description);
    }
}