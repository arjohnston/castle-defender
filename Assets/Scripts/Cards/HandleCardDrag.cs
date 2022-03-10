using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleCardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Hex hexRayCast;
    private Vector3 origin;

    public void OnBeginDrag(PointerEventData eventData) {
        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        origin = transform.position;
        PlayerHand.Instance.SetCardIsDragged(gameObject);
    }

    public void OnDrag(PointerEventData eventData) {
        Card card = PlayerHand.Instance.GetCardForGameObject(gameObject);
        hexRayCast = Gameboard.Instance.GetHexRayCastHit();

        if (Gameboard.Instance.IsOccupied(hexRayCast)) {
            Gameboard.Instance.HighlightRaycastHitSpot(hexRayCast, HexColors.INVALID_MOVE, card.attributes.occupiedRadius);
        } else {
            Gameboard.Instance.HighlightRaycastHitSpot(hexRayCast, HexColors.VALID_MOVE, card.attributes.occupiedRadius);
        }

        transform.position = eventData.position + new Vector2(21f, -30f);
    }

    public void OnEndDrag(PointerEventData eventData) {
        Card card = PlayerHand.Instance.GetCardForGameObject(gameObject);

        if (
            hexRayCast == null
            || Gameboard.Instance.IsOccupied(hexRayCast)
            || (card.attributes.isRestrictedToMyTurn && !TurnManager.Instance.IsMyTurn())
            ) {
                transform.position = origin;
                transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            
            bool didSpawnSuccessfully = GameboardObjectManager.Instance.Spawn(hexRayCast, card);
            
            if (didSpawnSuccessfully) {
                PlayerHand.Instance.RemoveCardFromHand(gameObject);
            } else {
                transform.position = origin;
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        PlayerHand.Instance.RemoveCardIsDragged();
    }
}
