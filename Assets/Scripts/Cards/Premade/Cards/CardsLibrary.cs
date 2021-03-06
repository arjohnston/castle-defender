using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardsLibrary {
    public static Card CreateWall() {
        return new Card(
            Types.PERMANENT,
            Sprites.WALL,
            new Meta{
                title = "Wall",
                description = "A permanent wall, defending units behind where you place it",
            },
            new Attributes{
                hp = 5,
                cost = 0,
                occupiedRadius = 0,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateRanger() {
        return new Card(
            Types.CREATURE,
            Sprites.RANGER,
            new Meta{
                title = "Ranger",
                description = "A unit trained in the way of stealth archery",
            },
            new Attributes{
                hp = 3,
                cost = 3,
                speed = 2,
                range = 8,
                damage = 1,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateWolf() {
        return new Card(
            Types.CREATURE,
            Sprites.WOLF,
            new Meta{
                title = "Wolf",
                description = "A beast of the forest",
            },
            new Attributes{
                hp = 2,
                cost = 2,
                speed = 3,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateDemonSpawn() {
        return new Card(
            Types.CREATURE,
            Sprites.DemonSpawn,
            new Meta{
                title = "Demon Spawn",
                description = "A demon freed from the underworld!",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                speed = 3,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateArcher() {
        return new Card(
            Types.CREATURE,
            Sprites.ARCHER,
            new Meta{
                title = "Archer",
                description = "An archer of sorts",
            },
            new Attributes{
                hp = 2,
                cost = 2,
                speed = 1,
                range = 6,
                damage = 1,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateMinions() {
        return new Card(
            Types.CREATURE,
            Sprites.MINIONS,
            new Meta{
                title = "Minions",
                description = "A ragtag horde of minions, itching to do your bidding",
            },
            new Attributes{
                hp = 1,
                cost = 3,
                speed = 3,
                range = 1,
                damage = 1,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateWargRider() {
        return new Card(
            Types.CREATURE,
            Sprites.WARGRIDER,
            new Meta{
                title = "Warg Rider",
                description = "A powerful mounted unit, built for piercing enemy lines",
            },
            new Attributes{
                hp = 4,
                cost = 4,
                speed = 4,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreatePlaguecrafter() {
        return new Card(
            Types.CREATURE,
            Sprites.PLAGUECRAFTER,
            new Meta{
                title = "Plaguecrafter",
                description = "A shaman skilled in the ways of plague magic",
            },
            new Attributes{
                hp = 3,
                cost = 4,
                speed = 3,
                range = 1,
                damage = 3,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateTwoGoblins() {
        return new Card(
            Types.CREATURE,
            Sprites.TWOGOBLINS,
            new Meta{
                title = "Goblin Tagteam",
                description = "Two pesky goblins working together, looking for mayhem",
            },
            new Attributes{
                hp = 2,
                cost = 2,
                speed = 2,
                range = 1,
                damage = 1,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateOrc() {
        return new Card(
            Types.CREATURE,
            Sprites.ORC,
            new Meta{
                title = "Orc",
                description = "A massive orc that would crush anything in its way. Getting there is a different story though",
            },
            new Attributes{
                hp = 5,
                cost = 2,
                speed = 1,
                range = 1,
                damage = 4,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateDragonling() {
        return new Card(
            Types.CREATURE,
            Sprites.DRAGONLING,
            new Meta{
                title = "Dragonling",
                description = "A baby dragon",
            },
            new Attributes{
                hp = 1,
                cost = 3,
                speed = 3,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateKelensDagger() {
        return new Card(
            Types.ENCHANTMENT,
            Sprites.KELENSDAGGER,
            new Meta{
                title = "Kelen's Dagger",
                description = "Equipped creature gains +2 Attack. Equipped creature can attack from up to 3 units away, however battle is conducted normally as if both units are engaging from melee range.",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                damage = 2,
            },
            new Enchantment{
                validTarget = Types.CREATURE,
                damageModifier = 2,
                rangeModifier = 3
            }, 
            new Spell{}
        );
    }

    public static Card CreateBearTrap() {
        return new Card(
            Types.TRAP,
            Sprites.BEARTRAP,
            new Meta{
                title = "Bear Trap",
                description = "Deals 3 damage to an enemy unit that steps on it.",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                damage = 3,
                range = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateSpikePit() {
        return new Card(
            Types.TRAP,
            Sprites.SPIKEPIT,
            new Meta{
                title = "Spike Pit",
                description = "Deals 2 damage in a 2 unit wide aoe to any unit(s) that step on it.",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                damage = 2,
                range = 1,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreatePurge() {
        return new Card(
            Types.SPELL,
            Sprites.PURGE,
            new Meta{
                title = "Purge",
                description = "Remove all enchantments on target creature.",
            },
            new Attributes{
                hp = 1,
                cost = 3,
                isRestrictedToMyTurn = false,
                occupiedRadius = 0,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.REMOVE_ENCHANTMENTS,
                validTarget = Targets.ENEMY,
            }
        );
    }

    public static Card CreateElytrianBlessing() {
        return new Card(
            Types.ENCHANTMENT,
            Sprites.ELYTRIANBLESSING,
            new Meta{
                title = "Elytrian Blessing",
                description = "Equipped creature gains flying.",
            },
            new Attributes{
                hp = 1,
                cost = 2,
            },
            new Enchantment{
                validTarget = Types.CREATURE,
                flying = true,
            }, 
            new Spell{}
        );
    }

    public static Card CreateSwamp() {
        return new Card(
            Types.TRAP,
            Sprites.SWAMP,
            new Meta{
                title = "Swamp",
                description = "Target creature is stuck and cannot move for one turn",
            },
            new Attributes{
                hp = 2,
                cost = 1,
                damage = 0,
                range = 0,
                speedModifier = 0.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateElvenLongbow() {
        return new Card(
            Types.ENCHANTMENT,
            Sprites.ELVENLONGBOW,
            new Meta{
                title = "Elven Longbow",
                description = "Equipped creature gets +2 ATK. Equipped creature can forfeit moving on a turn to deal 1 damage to target creature.",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                damage = 2,
            },
            new Enchantment{
                validTarget = Types.CREATURE,
                damageModifier = 2,
                doubleStrike = true,
            }, 
            new Spell{}
        );
    }

    public static Card CreateCharge() {
        return new Card(
            Types.SPELL,
            Sprites.CHARGE,
            new Meta{
                title = "Charge",
                description = "Place this spell on a monster to order them to charge towards the nearest enemy wall",
            },
            new Attributes{
                hp = 1,
                cost = 1,
                isRestrictedToMyTurn = false,
                occupiedRadius = 0,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.CHARGE_WALL,
                validTarget = Targets.ALLY,
            }
        );
    }

    public static Card CreateHeal() {
        return new Card(
            Types.SPELL,
            Sprites.HEAL,
            new Meta{
                title = "Heal",
                description = "Heals a target for 4 HP",
            },
            new Attributes{
                hp = 4,
                cost = 3,
                isRestrictedToMyTurn = false,
                occupiedRadius = 0,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.HEAL,
                validTarget = Targets.ALLY,
                effectAmount = 4,
            }
        );
    }

    public static Card CreateFieryGreaves() {
        return new Card(
            Types.ENCHANTMENT,
            Sprites.FIERYGREAVES,
            new Meta{
                title = "Fiery Greaves",
                description = "Increase speed on equipped creature. The creature can now move twice as fast",
            },
            new Attributes{
                hp = 1,
                cost = 3,
            },
            new Enchantment{
                validTarget = Types.CREATURE,
                speedModifier = 2,
            }, 
            new Spell{}
        );
    }

    public static Card CreateArrowStorm() {
        return new Card(
            Types.SPELL,
            Sprites.ARROWSTORM,
            new Meta{
                title = "Arrow Storm",
                description = "Shoot down a flying creature. Creature is a ground unit for four turns",
            },
            new Attributes{
                hp = 1,
                cost = 1,
                isRestrictedToMyTurn = false,
                occupiedRadius = 0,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.GROUND_FLYING_CREATURE,
                validTarget = Targets.ENEMY,
                effectDuration = 4,
            }
        );
    }

    public static Card CreateKnight() {
        return new Card(
            Types.CREATURE,
            Sprites.KNIGHT,
            new Meta{
                title = "Knight",
                description = "A battle-hardened warrior, well-versed in the arts of warcraft and chivalry",
            },
            new Attributes{
                hp = 4,
                cost = 3,
                speed = 4,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateSquire() {
        return new Card(
            Types.CREATURE,
            Sprites.SQUIRE,
            new Meta{
                title = "Squire",
                description = "Only the ward of a knight, but may become a hero one day",
            },
            new Attributes{
                hp = 3,
                cost = 2,
                speed = 2,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateCannon() {
        return new Card(
            Types.CREATURE,
            Sprites.Cannon,
            new Meta{
                title = "Cannon",
                description = "A slow, but devastating artillery piece",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                speed = 1,
                range = 5,
                damage = 4,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateDragon() {
        return new Card(
            Types.CREATURE,
            Sprites.DRAGON,
            new Meta{
                title = "Dragon",
                description = "Spawn a dragon (can fly over walls or traps)",
            },
            new Attributes{
                hp = 3,
                cost = 4,
                speed = 4,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                flying = true,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateCatapult() {
        return new Card(
            Types.CREATURE,
            Sprites.CATAPULT,
            new Meta{
                title = "Trebuchet ",
                description = "A terrifying siege weapon. Does +3 damage to structures.",
            },
            new Attributes{
                hp = 3,
                cost = 3,
                speed = 1,
                range = 10,
                damage = 1,
                occupiedRadius = 0,
                permanentDamageModifier = 3,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }
 
    public static Card CreateGriffin() {
        return new Card(
            Types.CREATURE,
            Sprites.Griffin,
            new Meta{
                title = "Griffin",
                description = "First Attack deals 2 bonus damage.",
            },
            new Attributes{
                hp = 4,
                cost = 5,
                speed = 5,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                firstAttackDamageModifier = 2,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }
 
    public static Card CreateDeusVult() {
        return new Card(
            Types.ENCHANTMENT,
            Sprites.DEUSVULT,
            new Meta{
                title = "Deus Vult",
                description = "Equipped creature gets +1 to all stats, and is immune to all enemy spell and trap effects.",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                speed = 1,
                damage = 1,
            },
            new Enchantment{
                validTarget = Types.CREATURE,
                healthModifier = 1,
                damageModifier = 1,
            }, 
            new Spell{}
        );
    }

    public static Card CreateTowerArcher() {
        return new Card(
            Types.ENCHANTMENT,
            Sprites.TOWERARCHER,
            new Meta{
                title = "Tower Archer",
                description = "Equipped castle does 1 damage to all creatures within 3 spaces of castle each turn",
            },
            new Attributes{
                hp = 1,
                cost = 4,
                range = 3,
                damage = 1,
            },
            new Enchantment{
                validTarget = Types.PERMANENT,
                damageModifier = 1,
                doesDamageEachTurn = true,
            }, 
            new Spell{}
        );
    }

    public static Card CreateCallToArms() {
        return new Card(
            Types.SPELL,
            Sprites.CALLTOARMS,
            new Meta{
                title = "Call to Arms",
                description = "For the next 3 turns (including this one) all your creatures cost 1 less resource to summon.",
            },
            new Attributes{
                hp = 1,
                cost = 0,
                isRestrictedToMyTurn = false,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.REDUCE_RESOURCE_COST_CREATURES,
                validTarget = Targets.ANY,
                effectAmount = -1,
                effectDuration = 3,
            }
        );
    }

    public static Card CreateColossalHammer() {
        return new Card(
            Types.ENCHANTMENT,
            Sprites.COLOSSALHAMMER,
            new Meta{
                title = "Colossal Hammer",
                description = "Equipped creature gets +2 ATK",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                damage = 2,
            },
            new Enchantment{
                validTarget = Types.CREATURE,
                damageModifier = 2,
            }, 
            new Spell{}
        );
    }

    public static Card CreateClericsRobe() {
        return new Card(
            Types.ENCHANTMENT,
            Sprites.CLERICSROBE,
            new Meta{
                title = "Cleric's Robe",
                description = "Equipped creature gets +2 HP",
            },
            new Attributes{
                hp = 2,
                cost = 2,
            },
            new Enchantment{
                validTarget = Types.CREATURE,
                healthModifier = 2,
            }, 
            new Spell{}
        );
    }

    public static Card CreateManaElemental() {
        return new Card(
            Types.CREATURE,
            Sprites.MANAELEMENTAL,
            new Meta{
                title = "Mana Elemental",
                description = "Gains +1 Attack for every enchantment equipped",
            },
            new Attributes{
                hp = 3,
                cost = 3,
                speed = 2,
                range = 1,
                damage = 1,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateAncientGolem() {
        return new Card(
            Types.CREATURE,
            Sprites.ANCIENTGOLEM,
            new Meta{
                title = "Ancient Golem",
                description = "Takes -1 damage from spells and ranged units, but takes +1 damage from melee units. Unaffected by traps.",
            },
            new Attributes{
                hp = 8,
                cost = 6,
                speed = 1,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                speedModifier = 1.0f,
                spellModifier = -1,
                rangedHealthModifier = -1,
                meleeHealthModifier = 1,
                trapImmunity = true,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateArchmage() {
        return new Card(
            Types.CREATURE,
            Sprites.ARCHMAGE,
            new Meta{
                title = "Archmage",
                description = "Can attack up to 3 units away, increases all spell effects by 1.",
            },
            new Attributes{
                hp = 4,
                cost = 7,
                speed = 2,
                range = 3,
                damage = 2,
                occupiedRadius = 0,
                speedModifier = 1.0f,
                spellModifier = 1,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateEternalDragon() {
        return new Card(
            Types.CREATURE,
            Sprites.ETERNALDRAGON,
            new Meta{
                title = "Eternal Dragon",
                description = "A majestic dragon from ages past",
            },
            new Attributes{
                hp = 3,
                cost = 4,
                speed = 4,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                flying = true,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateSkeletons() {
        return new Card(
            Types.CREATURE,
            Sprites.SKELETONS,
            new Meta{
                title = "Skeletons",
                description = "Spawn two skeletons",
            },
            new Attributes{
                hp = 2,
                cost = 2,
                speed = 2,
                range = 1,
                damage = 1,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateCursedNecromancer() {
        return new Card(
            Types.CREATURE,
            Sprites.CURSEDNECROMANCER,
            new Meta{
                title = "Cursed Necromancer",
                description = "Spawn a 1 ATK/1HP ghoul every turn",
            },
            new Attributes{
                hp = 3,
                cost = 9,
                speed = 1,
                range = 1,
                damage = 0,
                occupiedRadius = 0,
                spawnGhoulEveryTurn = true,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateGhoul() {
        return new Card(
            Types.CREATURE,
            Sprites.GHOUL,
            new Meta{
                title = "Ghoul",
                description = "A Weakling creature, more suited as cannon fodder than a warrior!",
            },
            new Attributes{
                hp = 1,
                cost = 1,
                speed = 1,
                range = 1,
                damage = 1,
                occupiedRadius = 0,
                ethereal = true,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreatePhantom() {
        return new Card(
            Types.CREATURE,
            Sprites.PHANTOM,
            new Meta{
                title = "Phantom",
                description = "Flying ghost",
            },
            new Attributes{
                hp = 2,
                cost = 3,
                speed = 3,
                range = 1,
                damage = 2,
                occupiedRadius = 0,
                flying = true,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateFrostGiant() {
        return new Card(
            Types.CREATURE,
            Sprites.FROSTGIANT,
            new Meta{
                title = "Frost Giant",
                description = "A towering mythical beings from the far North",
            },
            new Attributes{
                hp = 4,
                cost = 6,
                speed = 1,
                range = 2,
                damage = 3,
                occupiedRadius = 0,
                speedModifier = 1.0f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateNecroticPlague() {
        return new Card(
            Types.TRAP,
            Sprites.NECROTICPLAGUE,
            new Meta{
                title = "Necrotic Plague",
                description = "Create a trap that deals 1 damage per turn for each monster in the trap. Creatures move at half speed while trapped. Destroyed once no creatures are in the activated trap.",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                range = 0,
                speedModifier = 0.5f,
            },
            new Enchantment{}, 
            new Spell{}
        );
    }

    public static Card CreateLightningBolt() {
        return new Card(
            Types.SPELL,
            Sprites.LIGHTNINGBOLT,
            new Meta{
                title = "Lightning Bolt",
                description = "Do 1 damage to target creature and 1 damage to two surrounding creatures. Creatures must be within 1 tile to receive damage",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                damage = 1,
                isRestrictedToMyTurn = false,
                occupiedRadius = 1,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.DAMAGE,
                validTarget = Targets.ENEMY,
                effectAmount = 1,
            }
        );
    }

    public static Card CreateMeteor() {
        return new Card(
            Types.SPELL,
            Sprites.METEOR,
            new Meta{
                title = "Meteor",
                description = "Cast down a meteor for 3 damage to all creatures within 1 space",
            },
            new Attributes{
                hp = 1,
                cost = 3,
                damage = 3,
                isRestrictedToMyTurn = false,
                occupiedRadius = 1,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.DAMAGE,
                validTarget = Targets.ENEMY,
                effectAmount = 3,
            }
        );
    }

    public static Card CreateLightningStorm() {
        return new Card(
            Types.SPELL,
            Sprites.LIGHTNINGSTORM,
            new Meta{
                title = "Lightning Storm",
                description = "Cause a storm of lightning in an area around the target, dealing 2 damage to each target",
            },
            new Attributes{
                hp = 1,
                cost = 4,
                damage = 2,
                isRestrictedToMyTurn = false,
                occupiedRadius = 1,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.DAMAGE,
                validTarget = Targets.ENEMY,
                effectAmount = 2,
            }
        );
    }

    public static Card CreateEarthShatter() {
        return new Card(
            Types.SPELL,
            Sprites.EARTHSHATTER,
            new Meta{
                title = "Earth Shatter",
                description = "Deals 1 damage in a 3 unit wide aoe. Enemies are stunned for one turn and cannot move or attack.",
            },
            new Attributes{
                hp = 1,
                cost = 2,
                damage = 1,
                isRestrictedToMyTurn = false,
                occupiedRadius = 1,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.DAMAGE_AND_STUN,
                validTarget = Targets.ENEMY,
                effectAmount = 1,
                effectDuration = 1,
            }
        );
    }

    public static Card CreateDeepFreeze() {
        return new Card(
            Types.SPELL,
            Sprites.DEEPFREEZE,
            new Meta{
                title = "Deep Freeze",
                description = "Freezes all units in a 5 unit wide aoe. Units cannot move or attack for 3 turns.",
            },
            new Attributes{
                hp = 1,
                cost = 4,
                isRestrictedToMyTurn = false,
                occupiedRadius = 2,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.STUN,
                validTarget = Targets.ENEMY,
                effectDuration = 3,
            }
        );
    }

    public static Card CreateFireball() {
        return new Card(
            Types.SPELL,
            Sprites.FIREBALL,
            new Meta{
                title = "Fireball",
                description = "Blast a fireball at any enemy or permanent for 2 damage",
            },
            new Attributes{
                hp = 1,
                cost = 1,
                damage = 2,
                isRestrictedToMyTurn = false,
                occupiedRadius = 0,
            },
            new Enchantment{}, 
            new Spell{
                spellType = SpellTypes.DAMAGE,
                validTarget = Targets.ENEMY,
                effectAmount = 2,
            }
        );
    }

    public static Card CreateSoulRing() {
        return new Card(
            Types.ENCHANTMENT,
            Sprites.SOULRING,
            new Meta{
                title = "Soul Ring",
                description = "Equipped creature can deal 1 damage per turn to any opposing creature",
            },
            new Attributes{
                hp = 1,
                cost = 3,
            },
            new Enchantment{
                validTarget = Types.CREATURE,
                doesDamageEachTurn = true,
            }, 
            new Spell{}
        );
    }

    public static Card CreateDevilsCollar() {
        return new Card(
            Types.ENCHANTMENT,
            Sprites.DEVILSCOLLAR,
            new Meta{
                title = "Devil's Collar",
                description = "Target creature gets + 2 ATK and -1 HP while equipped",
            },
            new Attributes{
                hp = 1,
                cost = 1,
                damage = 2,
            },
            new Enchantment{
                validTarget = Types.CREATURE,
                damageModifier = 2,
                healthModifier = -1,
            }, 
            new Spell{}
        );
    }

}
