using UnityEngine;
using System.Collections.Generic;
using Utilities.Singletons;
using System.Linq;
using TMPro;

public class PlayerHand : Singleton<PlayerHand> {
    public GameObject CardHandPrefab;
    public GameObject playerHandArea;

    Queue<KeyValuePair<GameObject, Card>> renderedPlayerHandCards = new Queue<KeyValuePair<GameObject, Card>>();

    private List<Card> tempList;

    [Header("Player Hand Card")]
    public int CardWidth = 100;
    public float PlayerHandCardRotationAmount = 10.0f;
    public float PlayerHandCardTranslateXAmount = 7.0f;
    public float PlayerHandCardTranslateYAmount = -3.0f;

    private GameObject cardHovered;
    private GameObject cardDragged;

    void LateUpdate() {
        TransformPositionIntoFan();
    }

    public void AddCardToHand(Card card) {
        // For each new card added, instantiate a new gameobject
        bool newCardExists = false;
        foreach (var existingCard in renderedPlayerHandCards) {
            if (card.Equals(existingCard.Value)) newCardExists = true;
        }

        if (!newCardExists) {
            // Instantiate game object, add it to the dictionary
            GameObject playerCard = Instantiate(CardHandPrefab, new Vector3(0, 0, 0), Quaternion.identity, this.transform);
            playerCard.transform.SetParent(playerHandArea.transform, false);

            CardBuilder.Instance.BuildCard(playerCard, card);

            renderedPlayerHandCards.Enqueue(new KeyValuePair<GameObject, Card>(playerCard, card));
        }
    }

    public void RemoveCardFromHand(GameObject gameObject) {
        renderedPlayerHandCards = new Queue<KeyValuePair<GameObject, Card>>(renderedPlayerHandCards.Where(item => !item.Key.Equals(gameObject)));
        Destroy(gameObject);
    }

    private KeyValuePair<GameObject, Card>? GetKvpForGameObject(GameObject gameObject) {
        foreach (KeyValuePair<GameObject, Card> kvp in renderedPlayerHandCards) {
            if (kvp.Key.Equals(gameObject)) return kvp;
        }

        return null;
    }

    public Card GetCardForGameObject(GameObject gameObject) {
        KeyValuePair<GameObject, Card>? kvp = GetKvpForGameObject(gameObject);

        return kvp?.Value;
    }

    private void TransformPositionIntoFan() {
        float center = renderedPlayerHandCards.Count / 2;

        foreach (KeyValuePair<GameObject, Card> kvp in renderedPlayerHandCards) {
            float distanceFromCenter = GetDistanceAwayFromCenter(center, kvp);

            if (!kvp.Key.Equals(cardDragged)) {
                if (kvp.Key.Equals(cardHovered)) {
                    kvp.Key.transform.SetAsLastSibling(); // Move to front most so its not obscured by other cards
                    kvp.Key.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f); // zoom in
                    kvp.Key.transform.eulerAngles = Vector3.zero; // remove rotation
                    kvp.Key.transform.localPosition = new Vector3((CardWidth + PlayerHandCardTranslateXAmount) * distanceFromCenter, 15.0f, 0f); // move up
                } else {
                    kvp.Key.transform.SetSiblingIndex(renderedPlayerHandCards.ToArray().ToList().IndexOf(kvp));
                    kvp.Key.transform.localScale = new Vector3(1f, 1f, 1f);
                    kvp.Key.transform.eulerAngles = new Vector3(0f, 0f, -(distanceFromCenter * PlayerHandCardRotationAmount));
                    kvp.Key.transform.localPosition = new Vector3((CardWidth + PlayerHandCardTranslateXAmount) * distanceFromCenter, -PlayerHandCardTranslateYAmount * 3.0f - Mathf.Abs(PlayerHandCardTranslateYAmount * distanceFromCenter * 1.4f), 0f);
                }
            }
        }
    }

    private float GetDistanceAwayFromCenter(float center, KeyValuePair<GameObject, Card> kvp) {
        int indexOfKvp = renderedPlayerHandCards.ToArray().ToList().IndexOf(kvp);
        return indexOfKvp - center;
    }

    public void SetCardIsHovered(GameObject card) {
        cardHovered = card;
    }

    public void RemoveCardIsHovered() {
        cardHovered = null;
    }

    public void SetCardIsDragged(GameObject card) {
        cardDragged = card;
        GameboardObjectManager.Instance.SetIsCardBeingDragged(true);
    }

    public void RemoveCardIsDragged() {
        cardDragged = null;
        GameboardObjectManager.Instance.SetIsCardBeingDragged(false);
    }
}