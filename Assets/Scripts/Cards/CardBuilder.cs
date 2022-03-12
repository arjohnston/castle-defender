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

            default:
                return null;
        }
    }
}