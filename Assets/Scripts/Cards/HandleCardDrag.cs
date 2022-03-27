using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class HandleCardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Hex hexRayCast;
    private Vector3 origin;
    private bool isPlacementValid = true;

    public void OnBeginDrag(PointerEventData eventData) {
        if (!TurnManager.Instance.IsMyTurn()) return;

        transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        origin = transform.position;
        PlayerHand.Instance.SetCardIsDragged(gameObject);
    }

    public void OnDrag(PointerEventData eventData) {
        if (!TurnManager.Instance.IsMyTurn()) {
            GameManager.Instance.ShowToastMessage("Not your turn");
            return;
        }

        Card card = PlayerHand.Instance.GetCardForGameObject(gameObject);
        hexRayCast = Gameboard.Instance.GetHexRayCastHit();
        Dictionary<Hex, HitSpot> hitSpots = new Dictionary<Hex, HitSpot>();

        if (hexRayCast != null) {
            GameboardObject gbo = GameboardObjectManager.Instance.GetGboAtHex(hexRayCast);

            if (card.type == Types.ENCHANTMENT) {
                if (gbo != null && gbo.IsOwner && card.enchantment.validTarget == gbo.GetGboType()) {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.VALID_SPELL, card.attributes.occupiedRadius));
                    isPlacementValid = true;
                } else {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius));
                    isPlacementValid = false;
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
                    isPlacementValid = true;
                } else {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius));
                    isPlacementValid = false;
                }
            } else {
                bool isValidMovement = true;

                if (card.type == Types.PERMANENT || card.type == Types.TRAP) {
                    Players player = GameManager.Instance.GetCurrentPlayer();
            
                    foreach (Hex h in Gameboard.Instance.GetHexesWithinRange(hexRayCast, card.attributes.occupiedRadius)) {
                        if (player == Players.PLAYER_ONE && h.R <= 0) isValidMovement = false;
                        if (player == Players.PLAYER_TWO && h.R >= 0) isValidMovement = false;
                    }
                }

                if ((gbo != null && (!gbo.IsTrap() || gbo.IsOwner)) || !isValidMovement) {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius));
                    isPlacementValid = false;
                } else {
                    hitSpots.Add(hexRayCast, new HitSpot(HexColors.VALID_MOVE, card.attributes.occupiedRadius));
                    isPlacementValid = true;
                }
            }
        }
    
        Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);

        transform.position = eventData.position + new Vector2(21f, -30f);
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (!TurnManager.Instance.IsMyTurn()) return;
        
        Card card = PlayerHand.Instance.GetCardForGameObject(gameObject);

        if (hexRayCast == null) {
            GameManager.Instance.ShowToastMessage("Invalid placement");
            isPlacementValid = false;
        }

        if (card.attributes.isRestrictedToMyTurn && !TurnManager.Instance.IsMyTurn()) {
            GameManager.Instance.ShowToastMessage("Not your turn");
            isPlacementValid = false;
        }

        if (!ResourceManager.Instance.HaveEnoughResources(card.attributes.cost)) {
            GameManager.Instance.ShowToastMessage("Not enough resources");
            isPlacementValid = false;
        }

        if (!isPlacementValid) {
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
