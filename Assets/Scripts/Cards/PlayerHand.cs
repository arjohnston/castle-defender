using UnityEngine;
using System.Collections.Generic;
using Utilities.Singletons;
using System.Linq;

public class PlayerHand : Singleton<PlayerHand> {
    public GameObject CardHandPrefab;
    public GameObject playerHandArea;

    // TODO: Should this be a array?
    Queue<KeyValuePair<GameObject, Card>> renderedPlayerHandCards = new Queue<KeyValuePair<GameObject, Card>>();

    private List<Card> tempList;

    private float _playerHandCardRotationAmount = 10f;
    private float _playerHandTranslateYAmount = 7f;
    private float _playerHandTranslateXAmount = -3f;
    private float _cardWidth = 100;

    void Start() {
        Stack<Card> stack = new Ranger().GetDeck();

        tempList = new List<Card>();

        foreach (Card card in stack) {
            tempList.Add(card);
        }
    }

    void LateUpdate() {
        // TODO: re-enable this to get the other cards functions to be called once per frame
        UpdatePlayerHandDisplay();
    }

    private void UpdatePlayerHandDisplay() {
        AddRemoveCardsFromRender();
        TransformPositionIntoFan();
    }

    private void AddRemoveCardsFromRender() {
        // TODO: re-enable
        // List<Card> newListOfCards = DeckManager.Instance.GetPlayerHand();

        // TODO: delete
        List<Card> newListOfCards = tempList;

        // TODO: If new list of cards is null, then that means the player hand should be cleared (e.g., all removed)
        if (newListOfCards == null) return;

        // For each new card added, instantiate a new gameobject
        foreach (Card newCard in newListOfCards) {
            bool newCardExists = false;
            foreach (var existingCard in renderedPlayerHandCards) {
                if (newCard.Equals(existingCard.Value)) newCardExists = true;
            }

            if (!newCardExists) {
                // Instantiate game object, add it to the dictionary
                GameObject playerCard = Instantiate(CardHandPrefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
                playerCard.transform.SetParent(playerHandArea.transform, false);
                // playerCard.GetComponent<TextMesh>("Title") = newCard.meta.title;
                renderedPlayerHandCards.Enqueue(new KeyValuePair<GameObject, Card>(playerCard, newCard));
            }
        };

        List<GameObject> keysToRemove = new List<GameObject>();

        // For each new card removed, destroy the gameobject
        foreach(var existingCard in renderedPlayerHandCards) {
            bool existingCardShouldBeRemoved = true;
            foreach (Card newCard in newListOfCards) {
                if (existingCard.Value.Equals(newCard)) existingCardShouldBeRemoved = false;
            }

            if (existingCardShouldBeRemoved) {
                Destroy(existingCard.Key);
                keysToRemove.Add(existingCard.Key);
            }
        }

        renderedPlayerHandCards = new Queue<KeyValuePair<GameObject, Card>>(renderedPlayerHandCards.Where(item => !keysToRemove.Contains(item.Key)));
    }

    private void TransformPositionIntoFan() {
        float center = renderedPlayerHandCards.Count / 2;

        foreach (KeyValuePair<GameObject, Card> kvp in renderedPlayerHandCards) {
            float distanceFromCenter = GetDistanceAwayFromCenter(center, kvp);

            kvp.Key.transform.eulerAngles = new Vector3(0f, 0f, -(distanceFromCenter * _playerHandCardRotationAmount));
            kvp.Key.transform.localPosition = new Vector3((_cardWidth + _playerHandTranslateXAmount) * distanceFromCenter, -Mathf.Abs(_playerHandTranslateYAmount * distanceFromCenter), 0f);
        }
        
    }

    private float GetDistanceAwayFromCenter(float center, KeyValuePair<GameObject, Card> kvp) {
        int indexOfKvp = renderedPlayerHandCards.ToArray().ToList().IndexOf(kvp);
        return indexOfKvp - center;
    }

    // TODO: Add Draggable to the game object that's instantiated
}