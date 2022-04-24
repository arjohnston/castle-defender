using Utilities.Singletons;
using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Linq;

[RequireComponent(typeof(NetworkObject))]
public class DeckManager : NetworkSingleton<DeckManager> {
    // Note: each client is responsible for their own decks, the server only cares about how many
    // of each card each player has, so it can notify other clients if they should show/hide decks
    [SerializeField] private NetworkVariable<int> playerOneDeckCountServer = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> playerTwoDeckCountServer = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> playerOneGraveCountServer = new NetworkVariable<int>(0);
    [SerializeField] private NetworkVariable<int> playerTwoGraveCountServer = new NetworkVariable<int>(0);

    private Stack<Card> playerDeck;
    private Stack<Card> playerGrave;
    public GameObject deckCount;
    private string deckName;
    public Camera userCamera;
    private Vector2 oldMousePos;

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
        hideDeckCount();
    }

    public void InitializeDeck(Deck deck) {
        playerDeck = Shuffle(deck.GetDeck());
        deckName = deck.GetDeckName();
        playerGrave = new Stack<Card>();

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            SetPlayerOneDeckServerRpc(deck.GetDeck().Count, 0);
        }

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO) {
            SetPlayerTwoDeckServerRpc(deck.GetDeck().Count, 0);
        }
    }

    public Stack<Card> Shuffle(Stack<Card> deck) {
        System.Random rnd = new System.Random();
        return new Stack<Card>(deck.OrderBy(x => rnd.Next()));
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
            Quaternion.Euler(new Vector3(0, 180.0f, 0))
        );
        P2DeckGameObject.name = "Deck_p2";
        meshRenderer = P2DeckGameObject.GetComponentInChildren<MeshRenderer>();
        meshRenderer.material = DeckBackMaterial;

        P2GraveGameObject = Instantiate(
            GameboardDeckPrefab, 
            Gameboard.Instance.GetPlayerTwoGraveArea().transform.position,
            Quaternion.Euler(new Vector3(0, 180.0f, 0))
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
        P1DeckGameObject.SetActive(playerOneDeckCountServer.Value > 0);
        P2DeckGameObject.SetActive(playerTwoDeckCountServer.Value > 0);
        P1GraveGameObject.SetActive(playerOneGraveCountServer.Value > 0);
        P2GraveGameObject.SetActive(playerTwoGraveCountServer.Value > 0);

        P1DeckGameObject.transform.localScale = new Vector3(1.0f, SetHeightScale(playerOneDeckCountServer.Value), 1.0f);
        P2DeckGameObject.transform.localScale = new Vector3(1.0f, SetHeightScale(playerTwoDeckCountServer.Value), 1.0f);
        P1GraveGameObject.transform.localScale = new Vector3(1.0f, SetHeightScale(playerOneGraveCountServer.Value), 1.0f);
        P2GraveGameObject.transform.localScale = new Vector3(1.0f, SetHeightScale(playerTwoGraveCountServer.Value), 1.0f);
    }

    private float SetHeightScale(int deckCount) {
        return 0.06f + (deckCount - 1) * 0.06f;
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

            // Check to see if graveyard is clicked
            // Note: we do not check for Deck clicks here since its auto-drawn at the start of each turn
            if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
                if (gameboardObjectRayCast.name.Contains("Grave_p1")) {
                    // TODO: handle doing something with the graveyard
                }
                else
                {
                    deckCount.SetActive(true);
                    Vector3 deckPos = deckCount.transform.position;
                    deckPos.y = Mouse.current.position.y.ReadValue();
                    deckPos.x = Mouse.current.position.x.ReadValue();
                    deckCount.transform.position = deckPos;
                    deckCount.GetComponent<TMPro.TextMeshProUGUI>().text = "Class: "+ deckName + "\n"+"Cards Remaining:" + playerOneDeckCountServer.Value;

                }
            } 

            if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO) {
                if (gameboardObjectRayCast.name.Contains("Grave_p2")) {
                    // TODO: handle doing something with the graveyard
                }
                else //if Deck_P2
                {
                    deckCount.SetActive(true);
                    Vector3 deckPos = deckCount.transform.position;
                    deckPos.y = Mouse.current.position.y.ReadValue();
                    deckPos.x = Mouse.current.position.x.ReadValue();
                    deckCount.transform.position = deckPos;
                    deckCount.GetComponent<TMPro.TextMeshProUGUI>().text = "Class: " + deckName + "\n" + "Cards Remaining:" + playerTwoDeckCountServer.Value;

                }
            } 
        }
    }
    public void hideDeckCount()
    {
        Vector2 currentMouse = new Vector2(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue());
        if (oldMousePos != currentMouse)
        {
            deckCount.SetActive(false);
        }
        oldMousePos = currentMouse;
    }
    public void DrawInitialHandOfCards() {
        for (float i = GameSettings.initialPlayerHandSize; i > 0; i--) {
            Invoke("DrawCard",(i-(.5f*i)));
        }
    }

    public void DrawCard() {
        if (PlayerHand.Instance.Count() >= GameSettings.maxPlayerHandSize) return;
        if (playerDeck.Count <= 0) return;

        Card card = playerDeck.Pop();
        PlayerHand.Instance.AddCardToHand(card);
        CardDrawAnimation.Instance.PlayerDraw();

        SetPlayerDeck();
    }

    public void AddToGrave(Card card) {
        playerGrave.Push(card);

        SetPlayerDeck();
    }

    private void SetPlayerDeck() {
        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE) {
            SetPlayerOneDeckServerRpc(playerDeck.Count, playerGrave.Count);
        }

        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO) {
            SetPlayerTwoDeckServerRpc(playerDeck.Count, playerGrave.Count);
        }
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetPlayerOneDeckServerRpc(int deckCount, int graveCount) {
        playerOneDeckCountServer.Value = deckCount;
        playerOneGraveCountServer.Value = graveCount;
    }

    [ServerRpc(RequireOwnership=false)]
    public void SetPlayerTwoDeckServerRpc(int deckCount, int graveCount) {
        playerTwoDeckCountServer.Value = deckCount;
        playerTwoGraveCountServer.Value = graveCount;
    }
}