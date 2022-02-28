using System.Collections.Generic;

public class Wizard : Deck {
    public Wizard() {
        Stack<Card> d = new Stack<Card>();
        for (int i = 0; i < 10; i++)
            d.Push(cleric);

        base.SetDeck(d);
    }

    private Card cleric = new Card{
        meta = new Meta{
            title = "Cleric",
            description = "A wizard? Knope - a cleric!",
        },

        attributes = new Attributes{
            cost = 3,
            speed = 1,
            range = 2,
            damage = 3,
            occupiedRadius = 0,
        }
    };
}
