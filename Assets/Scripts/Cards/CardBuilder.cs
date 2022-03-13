using UnityEngine;
using Utilities.Singletons;
using TMPro;
using UnityEngine.UI;

public class CardBuilder : Singleton<CardBuilder> {
    [Header("Card Fronts")]
    public Sprite Creature;
    public Sprite Trap;
    public Sprite Permanent;
    public Sprite Spell;

    [Header("Card Images")]
    public Sprite Bowman;
    public Sprite Orc;
    public Sprite Cleric;
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
                playerCardImages[0].sprite = Permanent;
                break;

            case Types.TRAP:
                playerCardImages[0].sprite = Trap;
                break;

            case Types.SPELL:
                playerCardImages[0].sprite = Spell;
                break;
        }

        playerCardImages[1].sprite = GetSpriteForCard(card);
    }

    public Sprite GetSpriteForCard(Card card) {
        // TODO: Create a mapping of available creature types & their images.
        // Similar to switch-case above, but create another enum class with all
        // possible cards (orc, fireball, etc)
        switch (card.sprite) {
            case Sprites.BOWMAN:
                return Bowman;

            case Sprites.ORC:
                return Orc;

            case Sprites.CLERIC:
                return Cleric;

            case Sprites.RANGER:
                return Ranger;

            case Sprites.WOLF:
                return Wolf;

            case Sprites.SHINOBI:
                return Shinobi;
            
            case Sprites.ARCHER:
                return Archer;

            case Sprites.MINIONS:
                return Minions;

            case Sprites.WARGRIDER:
                return WargRider;

            case Sprites.PLAGUECRAFTER:
                return Plaguecrafter;
            
            case Sprites.TWOGOBLINS:
                return TwoGoblins;

            case Sprites.DRAGONLING:
                return Dragonling;

            case Sprites.KELENSDAGGER:
                return KelensDagger;

            case Sprites.BEARTRAP:
                return BearTrap;

            case Sprites.SPIKEPIT:
                return SpikePit;

            case Sprites.PURGE:
                return Purge;

            case Sprites.ELYTRIANBLESSING:
                return ElytrianBlessing;

            case Sprites.SWAMP:
                return Swamp;

            case Sprites.ELVENLONGBOW:
                return ElvenLongbow;

            case Sprites.CHARGE:
                return Charge;

            case Sprites.HEAL:
                return Heal;

            case Sprites.FIERYGREAVES:
                return FieryGreaves;

            case Sprites.ARROWSTORM:
                return ArrowStorm;

            case Sprites.KNIGHT:
                return Knight;

            case Sprites.SQUIRE:
                return Squire;
            
            case Sprites.ASSASSIN:
                return Assassin;

            case Sprites.DRAGON:
                return Dragon;

            case Sprites.CATAPULT:
                return Catapult;

            case Sprites.CAVALRYLANCER:
                return CavalryLancer;

            case Sprites.DEUSVULT:
                return DeusVult;

            case Sprites.TOWERARCHER:
                return TowerArcher;

            case Sprites.CALLTOARMS:
                return CallToArms;

            case Sprites.COLOSSALHAMMER:
                return ColossalHammer;

            case Sprites.CLERICSROBE:
                return ClericsRobe;

            case Sprites.MANAELEMENTAL:
                return ManaElemental;

            case Sprites.ANCIENTGOLEM:
                return AncientGolem;

            case Sprites.ARCHMAGE:
                return Archmage;

            case Sprites.ETERNALDRAGON:
                return EternalDragon;

            case Sprites.SKELETONS:
                return Skeletons;

            case Sprites.CURSEDNECROMANCER:
                return CursedNecromancer;

            case Sprites.PHANTOM:
                return Phantom;

            case Sprites.FROSTGIANT:
                return FrostGiant;

            case Sprites.NECROTICPLAGUE:
                return NecroticPlague;

            case Sprites.LIGHTNINGBOLT:
                return LightningBolt;

            case Sprites.METEOR:
                return Meteor;

            case Sprites.LIGHTNINGSTORM:
                return LightningBolt;

            case Sprites.EARTHSHATTER:
                return EarthShatter;

            case Sprites.DEEPFREEZE:
                return DeepFreeze;

            case Sprites.FIREBALL:
                return Fireball;

            case Sprites.SOULRING:
                return SoulRing;

            case Sprites.DEVILSCOLLAR:
                return DevilsCollar;

            default:
                return null;
        }
    }
}