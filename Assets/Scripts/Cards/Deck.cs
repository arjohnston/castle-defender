using System.Collections.Generic;
using Unity.Netcode;

public abstract class Deck {
    private Stack<Card> deck;
    private string deckName;

    public void SetDeck(Stack<Card> deck) {
        this.deck = deck;
        foreach( Card d in deck)
        {
            //set the max hp and starting attack
            d.attributes.SetDependents();
        }
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