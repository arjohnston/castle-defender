using System.Collections.Generic;

public class Wizard : Deck {
    public Wizard() {
        Stack<Card> d = new Stack<Card>();
        
        for (int i = 0; i < 10; i++)
            d.Push(CreateCleric());

        base.SetDeck(d);
    }

    private Card CreateCleric() {
        return new Card(
            Types.CREATURE,
            Sprites.CLERIC,
            new Meta{
                title = "Cleric",
                description = "A wizard? Knope - a cleric!",
            },
            new Attributes{
                hp = 2,
                cost = 3,
                speed = 1,
                range = 2,
                damage = 3,
                occupiedRadius = 0,
            }
        );
    }
}
