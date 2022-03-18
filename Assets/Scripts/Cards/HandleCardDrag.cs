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
            if (Gameboard.Instance.IsOccupied(hexRayCast)) {
                hitSpots.Add(hexRayCast, new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius));
            } else {
                hitSpots.Add(hexRayCast, new HitSpot(HexColors.VALID_MOVE, card.attributes.occupiedRadius));
            }
        }
    
        Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);

        transform.position = eventData.position + new Vector2(21f, -30f);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!TurnManager.Instance.IsMyTurn()) return;
        
        Card card = PlayerHand.Instance.GetCardForGameObject(gameObject);

        if (
            hexRayCast == null
            || Gameboard.Instance.IsOccupied(hexRayCast)
            || (card.attributes.isRestrictedToMyTurn && !TurnManager.Instance.IsMyTurn())
            ) {
                transform.position = origin;
                transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            if (ResourceManager.Instance.HaveEnoughResources(card.attributes.cost)) {
                GameboardObjectManager.Instance.UseCard(hexRayCast, card);
                PlayerHand.Instance.RemoveCardFromHand(gameObject);
                ResourceManager.Instance.UseResource(card.attributes.cost);
            } else {
                transform.position = origin;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        PlayerHand.Instance.RemoveCardIsDragged();
    }
}
