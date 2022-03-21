using Unity.Netcode;

public struct Attributes : INetworkSerializable {
    public int hp;
    public int cost;
    public int speed;
    public int range;
    public int damage;
    public int occupiedRadius;
    public bool isRestrictedToMyTurn;

    // creature mechanics
    public bool flying;
    public int permanentDamageModifier;
    public int firstAttackDamageModifier;
    public bool spawnGhoulEveryTurn;
    public bool ethereal; // whether or not the card goes to the graveyard when creature is destroyed
    public bool trapImmunity;
    public int meleeHealthModifier; // takes X amount more dmg when attacked in melee
    public int rangedHealthModifier; // takes X amount more dmg when attacked in range
    public int spellModifier; // takes X amount more damage from spells

    // trap mechanics
    public float speedModifier;

    // spell mechanics

    // enchantment mechanics

    public Attributes(
        int hp = 5, 
        int cost = 2, 
        int speed = 1, 
        int range = 1, 
        int damage = 1, 
        int occupiedRadius = 0, 
        bool isRestrictedToMyTurn = true,
        bool flying = false,
        int permanentDamageModifier = 0,
        int firstAttackDamageModifier = 0,
        bool spawnGhoulEveryTurn = false,
        bool ethereal = false,
        float speedModifier = 1.0f,
        bool trapImmunity = false,
        int meleeHealthModifier = 0,
        int rangedHealthModifier = 0,
        int spellModifier = 0
        ) {
        this.hp = hp;
        this.cost = cost;
        this.speed = speed;
        this.range = range;
        this.damage = damage;
        this.occupiedRadius = occupiedRadius;
        this.isRestrictedToMyTurn = isRestrictedToMyTurn;
        this.flying = flying;
        this.permanentDamageModifier = permanentDamageModifier;
        this.firstAttackDamageModifier = firstAttackDamageModifier;
        this.spawnGhoulEveryTurn = spawnGhoulEveryTurn;
        this.ethereal = ethereal;
        this.speedModifier = speedModifier;
        this.trapImmunity = trapImmunity;
        this.meleeHealthModifier = meleeHealthModifier;
        this.rangedHealthModifier = rangedHealthModifier;
        this.spellModifier = spellModifier;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref hp);
        serializer.SerializeValue(ref cost);
        serializer.SerializeValue(ref speed);
        serializer.SerializeValue(ref range);
        serializer.SerializeValue(ref damage);
        serializer.SerializeValue(ref occupiedRadius);
        serializer.SerializeValue(ref isRestrictedToMyTurn);
        serializer.SerializeValue(ref flying);
        serializer.SerializeValue(ref permanentDamageModifier);
        serializer.SerializeValue(ref firstAttackDamageModifier);
        serializer.SerializeValue(ref spawnGhoulEveryTurn);
        serializer.SerializeValue(ref ethereal);
        serializer.SerializeValue(ref speedModifier);
        serializer.SerializeValue(ref trapImmunity);
        serializer.SerializeValue(ref meleeHealthModifier);
        serializer.SerializeValue(ref rangedHealthModifier);
        serializer.SerializeValue(ref spellModifier);
    }
}
