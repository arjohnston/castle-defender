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
    public Sprite Ghoul;



    public void BuildCard(GameObject gameObject, Card card, bool ignoreSpellEffects = true) {
        TextMeshProUGUI[] playerCardText = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        playerCardText[0].text = card.meta.title;
        playerCardText[1].text = card.meta.description;

        if (card.type == Types.SPELL || card.type == Types.ENCHANTMENT) {
            // Spells & enchantments do not have HP/DMG since they're ethereal
            playerCardText[2].text = "";
            playerCardText[3].text = "";
        } else {
            playerCardText[2].text = card.attributes.hp.ToString();
            playerCardText[3].text = card.attributes.damage.ToString();
        }

        if (card.type == Types.ENCHANTMENT) {
            // Card is light, so increase contrast
            playerCardText[0].color = Color.black; // title
            playerCardText[4].color = Color.black; // cost
        }
        
        playerCardText[4].text = card.attributes.cost.ToString();

        Image[] playerCardImages = gameObject.GetComponentsInChildren<Image>();

        switch(card.type) {
            case Types.CREATURE:
                playerCardImages[0].sprite = Creature;
                break;

            case Types.PERMANENT:
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

        playerCardImages[1].sprite = GetSprite(card.sprite);

        if (ignoreSpellEffects) {
            playerCardImages[2].gameObject.SetActive(false); // Stunned
            playerCardImages[3].gameObject.SetActive(false); // Grounded
        }
    }

    public Sprite GetSprite(Sprites sprite) {
        switch (sprite) {
            case Sprites.ORC:
                return Orc;

            case Sprites.CASTLE:
                // return Castle;
                return Orc;

            case Sprites.WALL:
                return Wall;

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
                return Minions;

            case Sprites.WARGRIDER:
                // return WargRider;
                return Orc;

            case Sprites.PLAGUECRAFTER:
                return Plaguecrafter;
            
            case Sprites.TWOGOBLINS:
                return TwoGoblins;

            case Sprites.DRAGONLING:
                return Dragonling;

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
                return Swamp;

            case Sprites.ELVENLONGBOW:
                // return ElvenLongbow;
                return Orc;

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
                return TowerArcher;

            case Sprites.CALLTOARMS:
                // return CallToArms;
                return Orc;

            case Sprites.COLOSSALHAMMER:
                return ColossalHammer;

            case Sprites.CLERICSROBE:
                return ClericsRobe;

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
                return LightningStorm;

            case Sprites.EARTHSHATTER:
                // return EarthShatter;
                return Orc;

            case Sprites.DEEPFREEZE:
                // return DeepFreeze;
                return Orc;

            case Sprites.FIREBALL:
                return Fireball;

            case Sprites.SOULRING:
                return SoulRing;

            case Sprites.DEVILSCOLLAR:
                return DevilsCollar;

            case Sprites.GHOUL:
                // return Ghoul;
                return Orc;

            default:
                return null;
        }
    }
}