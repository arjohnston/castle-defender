using System.Collections.Generic;

public class Warrior : Deck {
    public Warrior() {
        Stack<Card> d = new Stack<Card>();

        for (int i = 0; i < 5; i++)
            d.Push(CardsLibrary.CreateWall());
        
        for (int i = 0; i < 3; i++)
            d.Push(CardsLibrary.CreateKnight());

         for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateSquire());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateArcher());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateAssassin());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateDragon());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateCatapult());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateCavalryLancer());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateMinions());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateTwoGoblins());

        for (int i = 0; i < 4; i++)
            d.Push(CardsLibrary.CreateDeusVult());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateTowerArcher());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateCallToArms());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateFieryGreaves());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateColossalHammer());
        
        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateClericsRobe());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateHeal());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateArrowStorm());

        base.SetDeck(d);
    }

}
