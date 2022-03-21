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

            // TODO: If hex ray cast with occupied radius intersects with a valid target, then hit
            // Probably need a helper function for spells to determine if the target is appropriate:
            // enemy versus ally, correct type, etc.

            // TODO: At spell end, if at least one valid target exists in occupied radius, then
            // do the effect on that target but not invalid targets (enemy vs ally) in the radius

            if (card.type == Types.ENCHANTMENT) {
                if (gbo != null && gbo.IsOwner && card.enchantment.validTarget == gbo.GetGboType()) {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.VALID_SPELL, card.attributes.occupiedRadius));
                } else {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius));
                }
            } else if (card.type == Types.SPELL) {
                List<Hex> targetHexes = Gameboard.Instance.GetHexesWithinRange(hexRayCast, card.attributes.occupiedRadius, true);
                List<GameboardObject> targets = new List<GameboardObject>();

                foreach (Hex hex in targetHexes) {
                    GameboardObject gameboardObject = GameboardObjectManager.Instance.GetGboAtHex(hex);

                    // if I'm the owner and target is ally
                    // or I'm not the owner and target is enemy
                    if (gameboardObject != null) {
                        if ((gameboardObject.IsOwner && card.spell.validTarget == Targets.ALLY) || (!gameboardObject.IsOwner && card.spell.validTarget == Targets.ENEMY))
                            targets.Add(gameboardObject);
                    }
                }

                if (card.spell.validTarget == Targets.ANY || targets.Count > 0) {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.VALID_SPELL, card.attributes.occupiedRadius));
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

        List<Hex> targetHexes = Gameboard.Instance.GetHexesWithinRange(hexRayCast, card.attributes.occupiedRadius, true);
        List<GameboardObject> targets = new List<GameboardObject>();

        foreach (Hex hex in targetHexes) {
            GameboardObject gameboardObject = GameboardObjectManager.Instance.GetGboAtHex(hex);

            // if I'm the owner and target is ally
            // or I'm not the owner and target is enemy
            if (gameboardObject != null) {
                if ((gameboardObject.IsOwner && card.spell.validTarget == Targets.ALLY) || (!gameboardObject.IsOwner && card.spell.validTarget == Targets.ENEMY)) 
                    targets.Add(gameboardObject);
            }
        }

        if (card.type == Types.SPELL && card.spell.validTarget != Targets.ANY && targets.Count <= 0) {
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
