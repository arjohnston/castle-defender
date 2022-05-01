using UnityEngine;
using System.Collections.Generic;
using Utilities.Singletons;
using System.Linq;
using TMPro;
using System;

public class PlayerHand : Singleton<PlayerHand> {
    public GameObject CardHandPrefab;
    public GameObject playerHandArea;
    public GameObject cardSpawnLocation;
    public float[] drawTimer; 
    public bool[] drawAnimation;
    public GameObject[] drawGBO;
    public Card[] drawCardData;
    public int drawAnimationsToDo = 0;

    Queue<KeyValuePair<GameObject, Card>> renderedPlayerHandCards = new Queue<KeyValuePair<GameObject, Card>>();
    Queue<KeyValuePair<GameObject, Card>> playerHandAnimations = new Queue<KeyValuePair<GameObject, Card>>();
    private List<Card> tempList;

    [Header("Player Hand Card")]
    public int CardWidth = 200;
    public float PlayerHandCardRotationAmount = 10.0f;
    public float PlayerHandCardTranslateXAmount = 10.0f;
    public float PlayerHandCardTranslateYAmount = -3.0f;

    private GameObject cardHovered;
    private GameObject cardDragged;

    private int _resourceCostModifier = 0;
    private int _resourceCostModifierTurnDuration = 0;

    private void Awake()
    {
        drawTimer = new float[6];
        drawAnimation = new bool[6];
        drawGBO = new GameObject[6];
        drawCardData = new Card[6];
    }
    void Update() {
        SetCostModifierForPlayerHandCards();
        for (int n = 0; n < 5; n++) drawTimer[n] += Time.deltaTime / 2.5f;
        ManageDrawAnimation();
    }

    void LateUpdate() {
        TransformPositionIntoFan();
    }

    public int GetResourceCostModifier() {
        return _resourceCostModifier;
    }

    public void SetResourceCostModifier(int modifier, int duration) {
        _resourceCostModifier = modifier;
        _resourceCostModifierTurnDuration = duration;
    }

    public void DecrementResourceCostModifierDuration() {
        if (_resourceCostModifierTurnDuration > 0) _resourceCostModifierTurnDuration -= 1;
        else {
            _resourceCostModifier = 0;
            _resourceCostModifierTurnDuration = 0;
        }
    }

    private void SetCostModifierForPlayerHandCards() {
        foreach (KeyValuePair<GameObject, Card> kvp in renderedPlayerHandCards) {
            TextMeshProUGUI[] playerCardText = kvp.Key.GetComponentsInChildren<TextMeshProUGUI>();

            playerCardText[4].text = Math.Max(0, (kvp.Value.attributes.cost + _resourceCostModifier)).ToString();
        }
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
            playerCard.transform.position = cardSpawnLocation.transform.position;
            playerCard.transform.eulerAngles = new Vector3(0f, 0f, -40f);
            CardBuilder.Instance.BuildCard(playerCard, card);

            playerHandAnimations.Enqueue(new KeyValuePair<GameObject, Card>(playerCard, card));
            FillNextAnimationSlot(card, playerCard);
        }
    }
    public void ManageDrawAnimation()
    {
        for (int a = 0; a < 6; a++)
        {
                if (drawAnimation[a])
                {
                    drawGBO[a].transform.position = Vector3.Lerp(cardSpawnLocation.transform.position,
                        playerHandArea.transform.position, drawTimer[a]);
                drawGBO[a].transform.eulerAngles = new Vector3(0f, 0f, -40+(drawTimer[a]*40));
                    if (drawTimer[a] > 1f)
                    { 
                        drawAnimation[a] = false;
                        renderedPlayerHandCards.Enqueue(playerHandAnimations.Dequeue());
                    }
                }
        }
    }
    public void FillNextAnimationSlot( Card card, GameObject gbo)
    {
        drawAnimationsToDo++;
        for(int i =0; i <6; i++)
        {
            if (!drawAnimation[i] && drawAnimationsToDo>0)
            {
                drawAnimation[i] = true;
                drawAnimationsToDo--;
                drawTimer[i] = 0f;
                drawGBO[i] = gbo;
                drawCardData[i] = card;
            }
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

        int indexOfHoveredCard = -1;

        foreach (KeyValuePair<GameObject, Card> kvp in renderedPlayerHandCards) {
            float distanceFromCenter = GetDistanceAwayFromCenter(center, kvp);

            if (!kvp.Key.Equals(cardDragged)) {
                if (kvp.Key.Equals(cardHovered)) {
                    indexOfHoveredCard = renderedPlayerHandCards.ToArray().ToList().IndexOf(kvp);
                }
            }
        }

        foreach (KeyValuePair<GameObject, Card> kvp in renderedPlayerHandCards) {
            float distanceFromCenter = GetDistanceAwayFromCenter(center, kvp);

            if (!kvp.Key.Equals(cardDragged)) {
                if (kvp.Key.Equals(cardHovered)) {
                    kvp.Key.transform.SetAsLastSibling(); // Move to front most so its not obscured by other cards
                    kvp.Key.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f); // zoom in
                    kvp.Key.transform.eulerAngles = Vector3.zero; // remove rotation
                    kvp.Key.transform.localPosition = new Vector3((CardWidth + PlayerHandCardTranslateXAmount) * distanceFromCenter, 120.0f, 0f); // move up
                } else {
                    int index = renderedPlayerHandCards.ToArray().ToList().IndexOf(kvp);
                    if (indexOfHoveredCard > -1 && index >= indexOfHoveredCard) index -= 1;

                    kvp.Key.transform.SetSiblingIndex(index);
                    kvp.Key.transform.localScale = new Vector3(1f, 1f, 1f);
                    kvp.Key.transform.eulerAngles = new Vector3(0f, 0f, -(distanceFromCenter * PlayerHandCardRotationAmount));
                    kvp.Key.transform.localPosition = new Vector3((CardWidth + PlayerHandCardTranslateXAmount) * distanceFromCenter, -PlayerHandCardTranslateYAmount * 4.0f - Mathf.Abs(PlayerHandCardTranslateYAmount * distanceFromCenter * 1.4f), 0f);
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

    public bool IsCardDragged() {
        return cardDragged != null;
    }

    public void RemoveCardIsDragged() {
        cardDragged = null;
        GameboardObjectManager.Instance.SetIsCardBeingDragged(false);
    }

    public int Count() {
        return renderedPlayerHandCards.Count + playerHandAnimations.Count;
    }
}