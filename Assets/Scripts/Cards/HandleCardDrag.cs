using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class HandleCardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Hex hexRayCast;
    private Vector3 origin;

    public void OnBeginDrag(PointerEventData eventData) {
        if (!TurnManager.Instance.IsMyTurn()) return;

        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        origin = transform.position;
        PlayerHand.Instance.SetCardIsDragged(gameObject);
    }

    public void OnDrag(PointerEventData eventData) {
        if (!TurnManager.Instance.IsMyTurn()) return;

        Card card = PlayerHand.Instance.GetCardForGameObject(gameObject);
        hexRayCast = Gameboard.Instance.GetHexRayCastHit();
        Dictionary<Hex, HitSpot> hitSpots = new Dictionary<Hex, HitSpot>();

        if (hexRayCast != null) {
            GameboardObject gbo = GameboardObjectManager.Instance.GetGboAtHex(hexRayCast);

            if (card.type == Types.ENCHANTMENT || card.type == Types.SPELL) {
                if (gbo != null && (gbo.IsOwner || card.type == Types.SPELL) && (card.type != Types.ENCHANTMENT || card.enchantment.validTarget == gbo.GetGboType())) {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.VALID_SPELL, card.attributes.occupiedRadius)); // TODO: spell occupied radius
                } else {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius));
                }
            } else {
                if (gbo != null && (!gbo.IsTrap() || gbo.IsOwner)) {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius));
                } else {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.VALID_MOVE, card.attributes.occupiedRadius));
                }
            }
        }
    
        Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);

        transform.position = eventData.position + new Vector2(21f, -30f);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!TurnManager.Instance.IsMyTurn()) return;
        
        Card card = PlayerHand.Instance.GetCardForGameObject(gameObject);

        bool returnToHand = false;

        if (
            hexRayCast == null || // not on gameboard
            (card.attributes.isRestrictedToMyTurn && !TurnManager.Instance.IsMyTurn()) || // not my turn
            !ResourceManager.Instance.HaveEnoughResources(card.attributes.cost) // dont have enough resources
        ) returnToHand = true;

        GameboardObject target = GameboardObjectManager.Instance.GetGboAtHex(hexRayCast);

        if (card.type == Types.ENCHANTMENT && (target == null || !target.IsOwner || card.enchantment.validTarget != target.GetGboType())) {
            returnToHand = true;
        }

        if (card.type == Types.SPELL && target == null) {
            // Allied versus enemy spells
            returnToHand = true;
        }

        if ((card.type == Types.CREATURE || card.type == Types.PERMANENT || card.type == Types.TRAP)) {
            if (target != null && (!target.IsTrap() || target.IsOwner)) returnToHand = true;
        }

        if (returnToHand) {
            transform.position = origin;
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            GameboardObjectManager.Instance.UseCard(hexRayCast, card);
            PlayerHand.Instance.RemoveCardFromHand(gameObject);
            ResourceManager.Instance.UseResource(card.attributes.cost);
        }

        PlayerHand.Instance.RemoveCardIsDragged();
    }
}
