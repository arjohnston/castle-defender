using UnityEngine;
using System.Collections.Generic;
using Utilities.Singletons;

public class PlayerHand : Singleton<PlayerHand> {
    public GameObject CardHandPrefab;
    public GameObject playerHandArea;

    private Dictionary<GameObject, Card> renderedPlayerHandCards = new Dictionary<GameObject, Card>();

    void Start() {
        Debug.Log("Hit");

        // TODO: This isn't adhering to the player area. How come? How to spawn a UI element as a child? Or gameobject?
        List<Card> newListOfCards = DeckManager.Instance.GetPlayerHand();
        GameObject playerCard = Instantiate(CardHandPrefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
        playerCard.transform.SetParent(playerHandArea.transform, false);
        // renderedPlayerHandCards.Add(playerCard, newCard);
    }

    void LateUpdate() {
        // UpdatePlayerHandDisplay();
    }

    private void UpdatePlayerHandDisplay() {
        AddRemoveCardsFromRender();
        TransformPositionIntoFan();
    }

    private void AddRemoveCardsFromRender() {
        List<Card> newListOfCards = DeckManager.Instance.GetPlayerHand();

        // Logger.Instance.LogInfo("New List Size: " + (newListOfCards == null ? "NULL " : newListOfCards.Count.ToString()));

        if (newListOfCards == null) return;

        // For each new card added, instantiate a new gameobject
        foreach (Card newCard in newListOfCards) {
            bool newCardExists = false;
            foreach (var existingCard in renderedPlayerHandCards) {
                if (newCard.Equals(existingCard.Value)) newCardExists = true;
            }

            if (!newCardExists) {
                // Logger.Instance.LogInfo("Size1: " + renderedPlayerHandCards.Count);
                // Instantiate game object, add it to the dictionary
                GameObject playerCard = Instantiate(CardHandPrefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
                playerCard.transform.SetParent(playerHandArea.transform, false);
                renderedPlayerHandCards.Add(playerCard, newCard);
                // Logger.Instance.LogInfo("Size2: " + renderedPlayerHandCards.Count);
            }
        }

        // For each new card removed, destroy the gameobject
        foreach(var existingCard in renderedPlayerHandCards) {
            bool existingCardShouldBeRemoved = false;
            foreach (Card newCard in newListOfCards) {
                if (existingCard.Equals(newCard)) existingCardShouldBeRemoved = true;
            }

            if (existingCardShouldBeRemoved) {
                // Logger.Instance.LogInfo("Size3: " + renderedPlayerHandCards.Count);

                Destroy(existingCard.Key);
                renderedPlayerHandCards.Remove(existingCard.Key);
                // Logger.Instance.LogInfo("Size4: " + renderedPlayerHandCards.Count);
            }
        }

        Logger.Instance.LogInfo("Size: " + renderedPlayerHandCards.Count);
    }

    private void TransformPositionIntoFan() {
        // Iterate through each card in `renderedPlayerHandCards`
        // Make a helper function that determines its rotation for its position

        // For when we add, remove cards from the dictionary - is the order preserved?
        //     If not, then we may need to utilize a queue to strongly enforce order
    }

    // Add Draggable to the game object that's instantiated
}