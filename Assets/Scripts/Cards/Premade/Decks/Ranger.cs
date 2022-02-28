using System.Collections.Generic;

public class Ranger : Deck {
    public Ranger() {
        Stack<Card> d = new Stack<Card>();
        for (int i = 0; i < 10; i++)
            d.Push(bowman);

        base.SetDeck(d);
    }

    private Card bowman = new Card{
        meta = new Meta{
            title = "Bowman",
            description = "An archer of sorts",
        },

        attributes = new Attributes{
            cost = 2,
            speed = 1,
            range = 2,
            damage = 3,
            occupiedRadius = 0,
        }
    };
}