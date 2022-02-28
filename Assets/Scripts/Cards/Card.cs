using Unity.Netcode;
using System;

public struct Card : INetworkSerializable, IEquatable<Card> {
    public Meta meta;
    public Attributes attributes;

    public Card(Meta meta, Attributes attributes) {
        this.meta = meta;
        this.attributes = attributes;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref meta);
        serializer.SerializeValue(ref attributes);
    }

    public bool Equals(Card other) {
        return meta.Equals(other.meta) && attributes.Equals(other.attributes);
    }

    public override bool Equals(object obj) {
        return obj is Card other && Equals(other);
    }
    
    public override int GetHashCode() {
        return meta.GetHashCode() ^ attributes.GetHashCode();
    }
}