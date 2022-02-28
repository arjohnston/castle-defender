using System.Collections.Generic;
using Unity.Netcode;

public abstract class Deck {
    private Stack<Card> deck;

    public void SetDeck(Stack<Card> deck) {
        this.deck = deck;
    }

    public Stack<Card> GetDeck() {
        return deck;
    }
}