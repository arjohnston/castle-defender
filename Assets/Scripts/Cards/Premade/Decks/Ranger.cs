using System.Collections.Generic;

public class Ranger : Deck {
    public Ranger() {
        Stack<Card> d = new Stack<Card>();

        for (int i = 0; i < 5; i++)
            d.Push(CardsLibrary.CreateWall());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateRanger());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateWolf());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateShinobi());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateArcher());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateMinions());    

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateWargRider());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreatePlaguecrafter());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateTwoGoblins());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateOrc());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateDragonling());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateKelensDagger());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateBearTrap());
        
        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateSpikePit());

        // for (int i = 0; i < 1; i++)
        //     d.Push(CardsLibrary.CreatePurge());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateElytrianBlessing());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateSwamp());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateElvenLongbow());

        // for (int i = 0; i < 2; i++)
        //     d.Push(CardsLibrary.CreateCharge());

        // for (int i = 0; i < 2; i++)
        //     d.Push(CardsLibrary.CreateHeal());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateFieryGreaves());

        // for (int i = 0; i < 2; i++)
        //     d.Push(CardsLibrary.CreateArrowStorm());

        base.SetDeck(d);
    }
  
}