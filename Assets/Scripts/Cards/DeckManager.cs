using Utilities.Singletons;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkObject))]
public class DeckManager : NetworkSingleton<DeckManager> {
    // Note: each client is responsible for their own decks, the server only cares about how many
    // of each card each player has, so it can notify other clients if they should show/hide decks
    [SerializeField] private NetworkVariable<int> playerOneDeckCountServer = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> playerTwoDeckCountServer = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> playerOneGraveCountServer = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> playerTwoGraveCountServer = new NetworkVariable<int>(0);

    private int playerOneDeckCount = 0;
    private int playerTwoDeckCount = 0;
    private int playerOneGraveCount = 0;
    private int playerTwoGraveCount = 0;

    private Stack<Card> playerDeck;
    private Stack<Card> playerGrave;

    private List<Card> playerHand;

    public GameObject GameboardDeckPrefab;
    public Material DeckBackMaterial;

    private GameObject P1DeckGameObject;
    private GameObject P1GraveGameObject;
    private GameObject P2DeckGameObject;
    private GameObject P2GraveGameObject;

    [SerializeField] private bool _leftClickMouseChanged = false;
    [SerializeField] private bool _lastLeftMouseButtonStateIsClicked = false;

    void Start() {
        CreateDeckGameObjects();
    }

    void Update() {
        ShowHideGameboardDecks();
        CheckOnLeftClick();
        CheckIfDeckClicked();
    }

    public void InitializeDeck(Deck deck) {
        playerDeck = deck.GetDeck();
        playerGrave = new Stack<Card>();
        playerHand = new List<Card>();

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            SetPlayerOneDeckServerRpc(deck.GetDeck().Count, 0);
        }

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO) {
            SetPlayerTwoDeckServerRpc(deck.GetDeck().Count, 0);
        }
    }

    public void CreateDeckGameObjects() {
        MeshRenderer meshRenderer;

        P1DeckGameObject = Instantiate(
            GameboardDeckPrefab, 
            Gameboard.Instance.GetPlayerOneDeckArea().transform.position,
            Quaternion.identity
        );
        P1DeckGameObject.name = "Deck_p1";
        meshRenderer = P1DeckGameObject.GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = DeckBackMaterial;

        P1GraveGameObject = Instantiate(
            GameboardDeckPrefab, 
            Gameboard.Instance.GetPlayerOneGraveArea().transform.position,
            Quaternion.identity
        );
        P1GraveGameObject.name = "Grave_p1";
        meshRenderer = P1GraveGameObject.GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = DeckBackMaterial;

        P2DeckGameObject = Instantiate(
            GameboardDeckPrefab, 
            Gameboard.Instance.GetPlayerTwoDeckArea().transform.position,
            Quaternion.identity
        );
        P2DeckGameObject.name = "Deck_p2";
        meshRenderer = P2DeckGameObject.GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = DeckBackMaterial;

        P2GraveGameObject = Instantiate(
            GameboardDeckPrefab, 
            Gameboard.Instance.GetPlayerTwoGraveArea().transform.position,
            Quaternion.identity
        );
        P2GraveGameObject.name = "Grave_p2";
        meshRenderer = P2GraveGameObject.GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = DeckBackMaterial;

        P1DeckGameObject.SetActive(false);
        P1GraveGameObject.SetActive(false);
        P2DeckGameObject.SetActive(false);
        P2GraveGameObject.SetActive(false);
    }

    public void ShowHideGameboardDecks() {
        P1DeckGameObject.SetActive(playerOneDeckCount > 0);
        P2DeckGameObject.SetActive(playerTwoDeckCount > 0);
        P1GraveGameObject.SetActive(playerOneGraveCount > 0);
        P2GraveGameObject.SetActive(playerTwoGraveCount > 0);
    }

    private void CheckOnLeftClick() {
        bool isClicked = Mouse.current.leftButton.isPressed;
        if (!_lastLeftMouseButtonStateIsClicked && isClicked) _leftClickMouseChanged = true;
        else _leftClickMouseChanged = false;
        _lastLeftMouseButtonStateIsClicked = Mouse.current.leftButton.isPressed;
    }

    private void CheckIfDeckClicked() {
        if (_leftClickMouseChanged) {
            if (!TurnManager.Instance.IsMyTurn() || TurnManager.Instance.GetGameState() == GameState.SETUP) return;

            GameObject gameboardObjectRayCast = Gameboard.Instance.GetDeckAtRayCastHit();

            if (gameboardObjectRayCast == null) return;

            if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
                if (gameboardObjectRayCast.name.Contains("Deck_p1")) {
                    DrawCard();
                }

                if (gameboardObjectRayCast.name.Contains("Grave_p1")) {
                    // TODO: handle doing something with the graveyard
                }
            } 

            if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO) {
                if (gameboardObjectRayCast.name.Contains("Deck_p2")) {
                    DrawCard();
                }

                if (gameboardObjectRayCast.name.Contains("Grave_p2")) {
                    // TODO: handle doing something with the graveyard
                }
            } 
        }
    }

    public void DrawInitialHandOfCards() {
        for (int i = 0; i < GameSettings.initialPlayerHandSize; i++) {
            DrawCard();
        }
    }

    private void DrawCard() {
        Card card = playerDeck.Pop();
        playerHand.Add(card);

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            SetPlayerOneDeckServerRpc(playerDeck.Count, playerGrave.Count);
        }

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO) {
            SetPlayerTwoDeckServerRpc(playerDeck.Count, playerGrave.Count);
        }
    }

    public List<Card> GetPlayerHand() {
        return playerHand;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetPlayerOneDeckServerRpc(int deckCount, int graveCount) {
        playerOneDeckCountServer.Value = deckCount;
        playerOneGraveCountServer.Value = graveCount;

        UpdateDeckVisualsClientRpc(
            playerOneDeckCountServer.Value, 
            playerTwoDeckCountServer.Value, 
            playerOneGraveCountServer.Value, 
            playerOneGraveCountServer.Value
        );
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetPlayerTwoDeckServerRpc(int deckCount, int graveCount) {
        playerTwoDeckCountServer.Value = deckCount;
        playerTwoGraveCountServer.Value = graveCount;

        UpdateDeckVisualsClientRpc(
            playerOneDeckCountServer.Value, 
            playerTwoDeckCountServer.Value,
            playerOneGraveCountServer.Value, 
            playerOneGraveCountServer.Value
        );
    }

    [ClientRpc]
    public void UpdateDeckVisualsClientRpc(int p1DeckCount, int p2DeckCount, int p1GraveCount, int p2GraveCount) {
        playerOneDeckCount = p1DeckCount;
        playerTwoDeckCount = p2DeckCount;
        playerOneGraveCount = p1GraveCount;
        playerTwoGraveCount = p2GraveCount;
    }
}