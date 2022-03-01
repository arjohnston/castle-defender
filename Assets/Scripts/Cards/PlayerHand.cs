using UnityEngine;
using System.Collections.Generic;
using Utilities.Singletons;

public class PlayerHand : Singleton<PlayerHand> {
    public GameObject CardHandPrefab;
    public GameObject playerHandArea;

    private Dictionary<GameObject, Card> renderedPlayerHandCards = new Dictionary<GameObject, Card>();

    void Start() {
        // TODO:
        // This was my first take at spawning a gameobject that adheres to the player area. It does not work.
        // Need to have a solid foundation for adding a card into the player area, and then we can do things like
        // turn it in to a fan of cards, etc.
        // This code is just temporary to get one card spawn to work, and then it can be removed
        List<Card> newListOfCards = DeckManager.Instance.GetPlayerHand();
        GameObject playerCard = Instantiate(CardHandPrefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
        playerCard.transform.SetParent(playerHandArea.transform, false);
    }

    void LateUpdate() {
        // TODO: re-enable this to get the other cards functions to be called once per frame
        // UpdatePlayerHandDisplay();
    }

    private void UpdatePlayerHandDisplay() {
        AddRemoveCardsFromRender();
        TransformPositionIntoFan();
    }

    private void AddRemoveCardsFromRender() {
        List<Card> newListOfCards = DeckManager.Instance.GetPlayerHand();

        // TODO: If new list of cards is null, then that means the player hand should be cleared (e.g., all removed)
        if (newListOfCards == null) return;

        // For each new card added, instantiate a new gameobject
        foreach (Card newCard in newListOfCards) {
            bool newCardExists = false;
            foreach (var existingCard in renderedPlayerHandCards) {
                // TODO: this equals method is not unique between two cards of the same type (e.g., 2 orc cards)
                // the Equals method in the Card.cs class needs to be updated to always be unique. There should be
                // some unique attribute already within each instantiated class (like a hash code). Need to re-do
                // that GetHashCode() method.
                if (newCard.Equals(existingCard.Value)) newCardExists = true;
            }

            if (!newCardExists) {
                // Instantiate game object, add it to the dictionary
                GameObject playerCard = Instantiate(CardHandPrefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
                playerCard.transform.SetParent(playerHandArea.transform, false);
                renderedPlayerHandCards.Add(playerCard, newCard);
            }
        }

        // For each new card removed, destroy the gameobject
        foreach(var existingCard in renderedPlayerHandCards) {
            bool existingCardShouldBeRemoved = false;
            foreach (Card newCard in newListOfCards) {
                if (existingCard.Equals(newCard)) existingCardShouldBeRemoved = true;
            }

            // TODO: After re-doing the GetHashCode() in the Card class from above, then we need to ensure the cards are appropriately
            // being added and removed.
            if (existingCardShouldBeRemoved) {
                Destroy(existingCard.Key);
                renderedPlayerHandCards.Remove(existingCard.Key);
            }
        }
    }

    private void TransformPositionIntoFan() {
        // TODO: Iterate through each card in `renderedPlayerHandCards`
        // Make a helper function that determines its rotation for its position

        // For when we add, remove cards from the dictionary - is the order preserved?
        //     If not, then we may need to utilize a queue to strongly enforce order
    }

    // TODO: Add Draggable to the game object that's instantiated
}