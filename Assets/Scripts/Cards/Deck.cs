using System.Collections.Generic;
using Unity.Netcode;

public abstract class Deck {
    private Stack<Card> deck;
    private string deckName;

    public void SetDeck(Stack<Card> deck) {
        this.deck = deck;
    }
    public Stack<Card> GetDeck() {
        return deck;
    }
    public void SetDeckName(string name)
    {
        deckName = name;
    }
    public string GetDeckName()
    {
        return deckName;
    }
}