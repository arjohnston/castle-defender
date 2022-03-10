using Unity.Netcode;
using System;

public class Card {
    public Meta meta;
    public Attributes attributes;
    public Types type;
    public Sprites sprite;

    public Card(Types type, Sprites sprite, Meta meta, Attributes attributes) {
        this.type = type;
        this.sprite = sprite;
        this.meta = meta;
        this.attributes = attributes;
    }
}