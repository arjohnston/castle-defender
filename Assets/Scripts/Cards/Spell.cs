using Unity.Netcode;

public struct Spell : INetworkSerializable {
    public SpellTypes spellType;
    public int effectAmount;
    public int occupiedRadius;
    public int effectDuration;

    public Spell(
        SpellTypes spellType = SpellTypes.DAMAGE,
        int effectAmount = 0,
        int occupiedRadius = 0,
        int effectDuration = 0
    ) {
        this.spellType = spellType;
        this.effectAmount = effectAmount;
        this.occupiedRadius = occupiedRadius;
        this.effectDuration = effectDuration;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref spellType);
        serializer.SerializeValue(ref effectAmount);
        serializer.SerializeValue(ref occupiedRadius);
        serializer.SerializeValue(ref effectDuration);
    }
}