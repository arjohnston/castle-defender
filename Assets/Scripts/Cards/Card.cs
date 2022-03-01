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
        // TODO: This needs to be updated to be unique 100% of the time. Right now this would
        // think instantiated objects of type orc with the same meta and attributes are equal.
        // That's not true since we want to compare object signatures against each other.
        return meta.GetHashCode() ^ attributes.GetHashCode();
    }
}