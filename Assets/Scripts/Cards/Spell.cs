using Unity.Netcode;

public struct Spell : INetworkSerializable {
    public SpellTypes spellType;
    public Targets validTarget;
    public int effectAmount;
    public int effectDuration;

    public Spell(
        SpellTypes spellType = SpellTypes.DAMAGE,
        Targets validTarget = Targets.ENEMY,
        int effectAmount = 0,
        int effectDuration = 0
    ) {
        this.spellType = spellType;
        this.validTarget = validTarget;
        this.effectAmount = effectAmount;
        this.effectDuration = effectDuration;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref spellType);
        serializer.SerializeValue(ref validTarget);
        serializer.SerializeValue(ref effectAmount);
        serializer.SerializeValue(ref effectDuration);
    }
}