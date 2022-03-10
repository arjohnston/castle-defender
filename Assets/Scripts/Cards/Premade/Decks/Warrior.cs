using System.Collections.Generic;

public class Warrior : Deck {
    public Warrior() {
        Stack<Card> d = new Stack<Card>();
        for (int i = 0; i < 10; i++)
            d.Push(CreateOrc());

        base.SetDeck(d);
    }

    private Card CreateOrc() {
        return new Card(
            Types.CREATURE,
            Sprites.ORC,
            new Meta{
                title = "Orc",
                description = "A meany orc guy",
            },
            new Attributes{
                hp = 4,
                cost = 2,
                speed = 1,
                range = 1,
                damage = 4,
                occupiedRadius = 0,
            }
        );
    }
}
