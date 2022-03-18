using Unity.Netcode;

public struct Attributes : INetworkSerializable {
    public int hp;
    public int cost;
    public int speed;
    public int range;
    public int damage;
    public int occupiedRadius;
    public bool isRestrictedToMyTurn;
    public bool flying;

    public Attributes(
        int hp = 5, 
        int cost = 2, 
        int speed = 1, 
        int range = 1, 
        int damage = 1, 
        int occupiedRadius = 0, 
        bool isRestrictedToMyTurn = true,
        bool flying = false
        ) {
        this.hp = hp;
        this.cost = cost;
        this.speed = speed;
        this.range = range;
        this.damage = damage;
        this.occupiedRadius = occupiedRadius;
        this.isRestrictedToMyTurn = isRestrictedToMyTurn;
        this.flying = flying;
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
    }
}
