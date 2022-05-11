using UnityEngine;
using Utilities.Singletons;
using TMPro;
using UnityEngine.UI;
using System;

public class CardBuilder : Singleton<CardBuilder>
{
    [Header("Card Fronts")]
    public Sprite Creature;
    public Sprite Trap;
    public Sprite Spell;
    public Sprite Enchantment;

    [Header("Card Images")]
    public Sprite Castle;
    public Sprite Wall;
    public Sprite Orc;
    public Sprite Cannon;
    public Sprite Ranger;
    public Sprite Wolf;
    //public Sprite Shinobi;
    public Sprite Archer;
    public Sprite Minions;
    public Sprite WargRider;
    public Sprite DemonSpawn;
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
    //public Sprite Assassin;
    public Sprite Dragon;
    public Sprite Catapult;
    public Sprite Griffin;
    public Sprite DeusVult;
    public Sprite TowerArcher;
    public Sprite CallToArms;
    public Sprite ColossalHammer;
    public Sprite ClericsRobe;
   // public Sprite ManaElemental;
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



    public void BuildCard(GameObject gameObject, Card card, bool ignoreSpellEffects = true)
    {
        TextMeshProUGUI[] playerCardText = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        playerCardText[0].text = card.meta.title;
        playerCardText[1].text = card.meta.description;

        if (card.type == Types.SPELL || card.type == Types.ENCHANTMENT)
        {
            // Spells & enchantments do not have HP/DMG since they're ethereal
            playerCardText[2].text = "";
            playerCardText[3].text = "";
        }
        else
        {
            playerCardText[2].text = card.attributes.hp.ToString();
            playerCardText[3].text = card.attributes.damage.ToString();
        }

        if (card.type == Types.ENCHANTMENT)
        {
            // Card is light, so increase contrast
            playerCardText[0].color = Color.black; // title
            playerCardText[4].color = Color.black; // cost
        }

        if (card.attributes.hp < card.attributes.maxHP)//creature is damaged
        {
            playerCardText[2].color = Color.red;
            playerCardText[2].fontStyle = FontStyles.Bold;
        }
        else if (card.attributes.hp > card.attributes.maxHP)
        {
            playerCardText[2].color = Color.green;
            playerCardText[2].fontStyle = FontStyles.Bold;
        }
        else
        {
            playerCardText[2].color = Color.white;
        }
        if (card.attributes.damage < card.attributes.startingAttack)//creature attack is debuffed
        {
            playerCardText[3].color = Color.red;
            playerCardText[3].fontStyle = FontStyles.Bold;
        }
        else if (card.attributes.damage > card.attributes.startingAttack)
        {
            playerCardText[3].color = Color.green;
            playerCardText[3].fontStyle = FontStyles.Bold;
        }

        playerCardText[4].text = Math.Max(0, card.attributes.cost).ToString();
        // but does actual cost get stopped form falling below 0?

        Image[] playerCardImages = gameObject.GetComponentsInChildren<Image>();

        switch (card.type)
        {
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

        if (ignoreSpellEffects)
        {
            playerCardImages[2].gameObject.SetActive(false); // Stunned
            playerCardImages[3].gameObject.SetActive(false); // Grounded
        }
    }

    public Sprite GetSprite(Sprites sprite)
    {
        switch (sprite)
        {
            case Sprites.ORC:
                return Orc;

            case Sprites.CASTLE:
                return Castle;

            case Sprites.WALL:
                return Wall;

            case Sprites.RANGER:
                // return Ranger;
                return Orc;

            case Sprites.WOLF:
                return Wolf;

            case Sprites.DemonSpawn:
                // return Shinobi;
                return DemonSpawn;

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

            case Sprites.Cannon:
                // return Assassin;
                return Cannon;

            case Sprites.DRAGON:
                return Dragon;

            case Sprites.CATAPULT:
                return Catapult;

            case Sprites.Griffin:
                return Griffin;

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
                // return ManaElemental;
                return Orc;

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
                return LightningStorm;

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

            case Sprites.GHOUL:
                return Ghoul;

            default:
                return null;
        }
    }
}