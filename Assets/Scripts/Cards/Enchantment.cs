using Unity.Netcode;
using System;

public struct Enchantment : INetworkSerializable, IEquatable<Enchantment> {
    public Types validTarget; // What type it can attack (e.g., creature, permanent, etc)
    public int damageModifier;
    public int healthModifier;
    public int rangeModifier;
    public int speedModifier;
    public bool flying;
    public bool spellImmunity; // immune to enemy spells
    public bool trapImmunity;
    public int enchantmentDamageModifier; // +X attack per enchant
    public bool doubleStrike; // Uses move action to attack twice
    public bool doesDamageEachTurn; // Use range + damage modifier to determine how much

    public Enchantment(
        Types validTarget = Types.CREATURE,
        int damageModifier = 0,
        int healthModifier = 0,
        int rangeModifier = 0,
        int speedModifier = 0,
        bool flying = false,
        bool spellImmunity = false,
        bool trapImmunity = false,
        int enchantmentDamageModifier = 0,
        bool doubleStrike = false,
        bool doesDamageEachTurn = false
    ) {
        this.validTarget = validTarget;
        this.damageModifier = damageModifier;
        this.healthModifier = healthModifier;
        this.rangeModifier = rangeModifier;
        this.speedModifier = speedModifier;
        this.flying = flying;
        this.spellImmunity = spellImmunity;
        this.trapImmunity = trapImmunity;
        this.enchantmentDamageModifier = enchantmentDamageModifier;
        this.doubleStrike = doubleStrike;
        this.doesDamageEachTurn = doesDamageEachTurn;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref validTarget);
        serializer.SerializeValue(ref damageModifier);
        serializer.SerializeValue(ref healthModifier);
        serializer.SerializeValue(ref rangeModifier);
        serializer.SerializeValue(ref speedModifier);
        serializer.SerializeValue(ref flying);
        serializer.SerializeValue(ref spellImmunity);
        serializer.SerializeValue(ref trapImmunity);
        serializer.SerializeValue(ref enchantmentDamageModifier);
        serializer.SerializeValue(ref doubleStrike);
        serializer.SerializeValue(ref doesDamageEachTurn);
    }

    public bool Equals(Enchantment other) {
      if (other == null) return false;

      if (validTarget == other.validTarget && 
        damageModifier == other.damageModifier && 
        healthModifier == other.healthModifier &&
        rangeModifier == other.rangeModifier &&
        speedModifier == other.speedModifier &&
        flying == other.flying &&
        spellImmunity == other.spellImmunity &&
        trapImmunity == other.trapImmunity &&
        enchantmentDamageModifier == other.enchantmentDamageModifier &&
        doubleStrike == other.doubleStrike &&
        doesDamageEachTurn == other.doesDamageEachTurn
        )
         return true;
      else
         return false;
   }

   public override bool Equals(Object obj) {
      if (obj == null)
         return false;

      Enchantment enchantmentObj = (Enchantment)obj;
      if (enchantmentObj == null)
         return false;
      else
         return Equals(enchantmentObj);
   }

   public override int GetHashCode() {
      return validTarget.GetHashCode() ^ damageModifier.GetHashCode() ^ healthModifier.GetHashCode() ^ rangeModifier.GetHashCode() ^ speedModifier.GetHashCode() ^ flying.GetHashCode() ^ spellImmunity.GetHashCode() ^ trapImmunity.GetHashCode() ^ enchantmentDamageModifier.GetHashCode() ^ doubleStrike.GetHashCode() ^ doesDamageEachTurn.GetHashCode();
   }

   public static bool operator == (Enchantment enchantment1, Enchantment enchantment2) {
      if (((object)enchantment1) == null || ((object)enchantment2) == null)
         return Object.Equals(enchantment1, enchantment2);

      return enchantment1.Equals(enchantment2);
   }

   public static bool operator != (Enchantment enchantment1, Enchantment enchantment2) {
      if (((object)enchantment1) == null || ((object)enchantment2) == null)
         return ! Object.Equals(enchantment1, enchantment2);

      return ! (enchantment1.Equals(enchantment2));
   }
}