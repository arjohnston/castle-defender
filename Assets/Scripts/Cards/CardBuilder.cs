using UnityEngine;
using Utilities.Singletons;
using TMPro;
using UnityEngine.UI;

public class CardBuilder : Singleton<CardBuilder> {
    [Header("Card Fronts")]
    public Sprite Creature;
    public Sprite Trap;
    public Sprite Spell;
    public Sprite Enchantment;

    [Header("Card Images")]
    public Sprite Castle;
    public Sprite Wall;
    public Sprite Orc;
    public Sprite Ranger;
    public Sprite Wolf;
    public Sprite Shinobi;
    public Sprite Archer;
    public Sprite Minions;
    public Sprite WargRider;
    public Sprite Plaguecrafter;
    public Sprite TwoGoblins;
    public Sprite Dragonling;
    public Sprite KelensDagger;
    public Sprite BearTrap;
    public Sprite SpikePit;
    public Sprite Purge;
    public Sprite ElytrianBlessing;
    public Sprite Swamp;
    public Sprite ElvenLongbow;
    public Sprite Charge;
    public Sprite Heal;
    public Sprite FieryGreaves;
    public Sprite ArrowStorm;
    public Sprite Knight;
    public Sprite Squire;
    public Sprite Assassin;
    public Sprite Dragon;
    public Sprite Catapult;
    public Sprite CavalryLancer;
    public Sprite DeusVult;
    public Sprite TowerArcher;
    public Sprite CallToArms;
    public Sprite ColossalHammer;
    public Sprite ClericsRobe;
    public Sprite ManaElemental;
    public Sprite AncientGolem;
    public Sprite Archmage;
    public Sprite EternalDragon;
    public Sprite Skeletons;
    public Sprite CursedNecromancer;
    public Sprite Phantom;
    public Sprite FrostGiant;
    public Sprite NecroticPlague;
    public Sprite LightningBolt;
    public Sprite Meteor;
    public Sprite LightningStorm;
    public Sprite EarthShatter;
    public Sprite DeepFreeze;
    public Sprite Fireball;
    public Sprite SoulRing;
    public Sprite DevilsCollar;



    public void BuildCard(GameObject gameObject, Card card) {
        TextMeshProUGUI[] playerCardText = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        playerCardText[0].text = card.meta.title;
        playerCardText[1].text = card.meta.description;
        playerCardText[2].text = card.attributes.hp.ToString();
        playerCardText[3].text = card.attributes.damage.ToString();
        playerCardText[4].text = card.attributes.cost.ToString();

        Image[] playerCardImages = gameObject.GetComponentsInChildren<Image>();

        switch(card.type) {
            case Types.CREATURE:
                playerCardImages[0].sprite = Creature;
                break;

            case Types.PERMANENT:
            case Types.WALL:
            case Types.TRAP:
                playerCardImages[0].sprite = Trap;
                break;

            case Types.SPELL:
                playerCardImages[0].sprite = Spell;
                break;

            case Types.ENCHANTMENT:
                playerCardImages[0].sprite = Enchantment;
                break;
        }

        playerCardImages[1].sprite = GetSpriteForCard(card);
    }

    public Sprite GetSpriteForCard(Card card) {
        switch (card.sprite) {
            case Sprites.ORC:
                return Orc;

            case Sprites.CASTLE:
                // return Castle;
                return Orc;

            case Sprites.WALL:
                // return Wall;
                return Orc;

            case Sprites.RANGER:
                // return Ranger;
                return Orc;

            case Sprites.WOLF:
                // return Wolf;
                return Orc;

            case Sprites.SHINOBI:
                // return Shinobi;
                return Orc;
            
            case Sprites.ARCHER:
                // return Archer;
                return Orc;

            case Sprites.MINIONS:
                // return Minions;
                return Orc;

            case Sprites.WARGRIDER:
                // return WargRider;
                return Orc;

            case Sprites.PLAGUECRAFTER:
                // return Plaguecrafter;
                return Orc;
            
            case Sprites.TWOGOBLINS:
                // return TwoGoblins;
                return Orc;

            case Sprites.DRAGONLING:
                // return Dragonling;
                return Orc;

            case Sprites.KELENSDAGGER:
                // return KelensDagger;
                return Orc;

            case Sprites.BEARTRAP:
                // return BearTrap;
                return Orc;

            case Sprites.SPIKEPIT:
                // return SpikePit;
                return Orc;

            case Sprites.PURGE:
                // return Purge;
                return Orc;

            case Sprites.ELYTRIANBLESSING:
                // return ElytrianBlessing;
                return Orc;

            case Sprites.SWAMP:
                // return Swamp;
                return Orc;

            case Sprites.ELVENLONGBOW:
                // return ElvenLongbow;
                return Orc;

            case Sprites.CHARGE:
                // return Charge;
                return Orc;

            case Sprites.HEAL:
                // return Heal;
                return Orc;

            case Sprites.FIERYGREAVES:
                // return FieryGreaves;
                return Orc;

            case Sprites.ARROWSTORM:
                // return ArrowStorm;
                return Orc;

            case Sprites.KNIGHT:
                // return Knight;
                return Orc;

            case Sprites.SQUIRE:
                // return Squire;
                return Orc;
            
            case Sprites.ASSASSIN:
                // return Assassin;
                return Orc;

            case Sprites.DRAGON:
                // return Dragon;
                return Orc;

            case Sprites.CATAPULT:
                // return Catapult;
                return Orc;

            case Sprites.CAVALRYLANCER:
                // return CavalryLancer;
                return Orc;

            case Sprites.DEUSVULT:
                // return DeusVult;
                return Orc;

            case Sprites.TOWERARCHER:
                // return TowerArcher;
                return Orc;

            case Sprites.CALLTOARMS:
                // return CallToArms;
                return Orc;

            case Sprites.COLOSSALHAMMER:
                // return ColossalHammer;
                return Orc;

            case Sprites.CLERICSROBE:
                // return ClericsRobe;
                return Orc;

            case Sprites.MANAELEMENTAL:
                // return ManaElemental;
                return Orc;

            case Sprites.ANCIENTGOLEM:
                // return AncientGolem;
                return Orc;

            case Sprites.ARCHMAGE:
                // return Archmage;
                return Orc;

            case Sprites.ETERNALDRAGON:
                // return EternalDragon;
                return Orc;

            case Sprites.SKELETONS:
                // return Skeletons;
                return Orc;

            case Sprites.CURSEDNECROMANCER:
                // return CursedNecromancer;
                return Orc;

            case Sprites.PHANTOM:
                // return Phantom;
                return Orc;

            case Sprites.FROSTGIANT:
                // return FrostGiant;
                return Orc;

            case Sprites.NECROTICPLAGUE:
                // return NecroticPlague;
                return Orc;

            case Sprites.LIGHTNINGBOLT:
                // return LightningBolt;
                return Orc;

            case Sprites.METEOR:
                // return Meteor;
                return Orc;

            case Sprites.LIGHTNINGSTORM:
                // return LightningBolt;
                return Orc;

            case Sprites.EARTHSHATTER:
                // return EarthShatter;
                return Orc;

            case Sprites.DEEPFREEZE:
                // return DeepFreeze;
                return Orc;

            case Sprites.FIREBALL:
                // return Fireball;
                return Orc;

            case Sprites.SOULRING:
                // return SoulRing;
                return Orc;

            case Sprites.DEVILSCOLLAR:
                // return DevilsCollar;
                return Orc;

            default:
                return null;
        }
    }
}