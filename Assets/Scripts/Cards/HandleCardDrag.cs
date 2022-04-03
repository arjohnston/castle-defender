using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class HandleCardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Hex hexRayCast;
    private Vector3 origin;
    private bool isPlacementValid = true;
    Dictionary<Hex, HitSpot> hitSpots = new Dictionary<Hex, HitSpot>();

    void Update() {
        Gameboard.Instance.HighlightRaycastHitSpot(hitSpots);
    }

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
        hitSpots = new Dictionary<Hex, HitSpot>();

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

// note: for some reason when surrounding hitspots.add statement with if(!hitSpots.ContainsKey(hexRayCast)) on the ones that dont have them added, it causes a bug where
// the player can place a card on one of their existing units and they will lose resources and the card, and not summon a unit.
// for this reason, temporarily commented out hitSpots conditionals to not cause this bug
                if ((gbo != null && (!gbo.IsTrap() || gbo.IsOwner)) || !isValidMovement) {
                    // if (!hitSpots.ContainsKey(hexRayCast))
                        hitSpots.Add(hexRayCast, new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius));
                    // hitSpots[hexRayCast] = new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius);
                    isPlacementValid = false;
                }
                
                if (card.type == Types.CREATURE && IsValidCreaturePlacement(hexRayCast)) {
                    // if (!hitSpots.ContainsKey(hexRayCast))
                        hitSpots.Add(hexRayCast, new HitSpot(HexColors.VALID_MOVE, card.attributes.occupiedRadius));
                    // hitSpots[hexRayCast] = new HitSpot(HexColors.VALID_MOVE, card.attributes.occupiedRadius);
                    isPlacementValid = true;
                }
                if (card.type == Types.CREATURE && !IsValidCreaturePlacement(hexRayCast)) {
                    // if (!hitSpots.ContainsKey(hexRayCast))
                        hitSpots.Add(hexRayCast, new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius));
                    // hitSpots[hexRayCast] = new HitSpot(HexColors.INVALID_MOVE, card.attributes.occupiedRadius);
                    isPlacementValid = false;
                }
                else {
                    // if (!hitSpots.ContainsKey(hexRayCast))
                        hitSpots.Add(hexRayCast, new HitSpot(HexColors.VALID_MOVE, card.attributes.occupiedRadius));
                    // hitSpots[hexRayCast] = new HitSpot(HexColors.VALID_MOVE, card.attributes.occupiedRadius);
                    isPlacementValid = true;
                }
            }
        }

        transform.position = eventData.position + new Vector2(21f, -30f);
    }

    public void OnEndDrag(PointerEventData eventData) {
        hitSpots = new Dictionary<Hex, HitSpot>();
        if (!TurnManager.Instance.IsMyTurn()) return;
        
        Card card = PlayerHand.Instance.GetCardForGameObject(gameObject);

        if (hexRayCast == null) {
            GameManager.Instance.ShowToastMessage("Invalid placement");
            isPlacementValid = false;
        }

        if (card.attributes.isRestrictedToMyTurn && !TurnManager.Instance.IsMyTurn()) {
            GameManager.Instance.ShowToastMessage("Not your turn");
            SoundManager.Instance.Play(Sounds.INVALID);
            isPlacementValid = false;
        }

        if (!ResourceManager.Instance.HaveEnoughResources(card.attributes.cost + PlayerHand.Instance.GetResourceCostModifier())) {
            GameManager.Instance.ShowToastMessage("Not enough resources");
            SoundManager.Instance.Play(Sounds.INVALID);
            isPlacementValid = false;
        }

        if (card.type == Types.CREATURE && !IsValidCreaturePlacement(hexRayCast)) {
            GameManager.Instance.ShowToastMessage("Invalid Creature Placement");
            SoundManager.Instance.Play(Sounds.INVALID);
        }

        if (!isPlacementValid) {
            transform.position = origin;
            transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            GameboardObjectManager.Instance.UseCard(hexRayCast, card);
            PlayerHand.Instance.RemoveCardFromHand(gameObject);
            ResourceManager.Instance.UseResource(card.attributes.cost + PlayerHand.Instance.GetResourceCostModifier());
        }

        PlayerHand.Instance.RemoveCardIsDragged();
    }
    private bool IsValidCreaturePlacement(Hex spawnLocation) {
        // if hex that has been dragged to is within 5 tiles of center of castle bloom, return true, else return false
            Hex castleLocation = GameboardObjectManager.Instance.GetCastle().GetHexPosition();
            int distance = Cube.GetDistanceToHex(castleLocation, spawnLocation);
            if(distance > GameSettings.creaturePlacementRange) {
                return false;
            }
            
            return true;

    }
}
