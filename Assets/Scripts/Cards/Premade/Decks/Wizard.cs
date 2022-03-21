using System.Collections.Generic;

public class Wizard : Deck {
    public Wizard() {
        Stack<Card> d = new Stack<Card>();

        for (int i = 0; i < 5; i++)
            d.Push(CardsLibrary.CreateWall());
        
        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateManaElemental());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateAncientGolem());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateArchmage());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateEternalDragon());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateSkeletons());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateCursedNecromancer());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreatePhantom());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateFrostGiant());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateMinions());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateNecroticPlague());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateLightningBolt());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateMeteor());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateLightningStorm());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateEarthShatter());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateDeepFreeze());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateFireball());

        for (int i = 0; i < 1; i++)
            d.Push(CardsLibrary.CreateSoulRing());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateDevilsCollar());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateClericsRobe());

        for (int i = 0; i < 2; i++)
            d.Push(CardsLibrary.CreateFieryGreaves());

        base.SetDeck(d);
    }

}
