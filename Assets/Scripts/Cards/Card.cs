using Unity.Netcode;
using System;

public class Card {
    public Meta meta;
    public Attributes attributes;

    public Card(Meta meta, Attributes attributes) {
        this.meta = meta;
        this.attributes = attributes;
    }
}