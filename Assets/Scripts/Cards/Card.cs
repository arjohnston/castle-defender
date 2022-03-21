using Unity.Netcode;
using System;

public class Card {
    public Meta meta;
    public Attributes attributes;
    public Types type;
    public Sprites sprite;
    public Enchantment enchantment;
    public Spell spell;

    public Card(Types type, Sprites sprite, Meta meta, Attributes attributes, Enchantment enchantment, Spell spell) {
        this.type = type;
        this.sprite = sprite;
        this.meta = meta;
        this.attributes = attributes;
        this.enchantment = enchantment;
        this.spell = spell;
    }
}