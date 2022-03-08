using System.Collections.Generic;

public class Ranger : Deck {
    public Ranger() {
        Stack<Card> d = new Stack<Card>();
        for (int i = 0; i < 5; i++)
            d.Push(CreateBowman());

        base.SetDeck(d);
    }

    private Card CreateBowman() {
        return new Card(
            new Meta{
                title = "Bowman",
                description = "An archer of sorts",
            },
            new Attributes{
                cost = 2,
                speed = 1,
                range = 2,
                damage = 3,
                occupiedRadius = 0,
            }
        );
    }
}