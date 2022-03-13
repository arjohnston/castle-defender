using System.Collections.Generic;

public class Ranger : Deck {
    public Ranger() {
        Stack<Card> d = new Stack<Card>();
        
        for (int i = 0; i < 6; i++)
            d.Push(CreateBowman());

        base.SetDeck(d);
    }

    private Card CreateBowman() {
        return new Card(
            Types.CREATURE,
            Sprites.BOWMAN,
            new Meta{
                title = "Bowman",
                description = "An archer of sorts",
            },
            new Attributes{
                hp = 2,
                cost = 2,
                speed = 1,
                range = 2,
                damage = 30,
                occupiedRadius = 0,
            }
        );
    }
}