using UnityEngine;
using TMPro;
using Utilities.Singletons;
using Unity.Netcode;

public class AnalyticsManager : NetworkSingleton<AnalyticsManager> {
    public TextMeshProUGUI totalTime;
    public TextMeshProUGUI amountOfTurns;
    public TextMeshProUGUI playerWon;

    public TextMeshProUGUI playerOneCardsPlayed;
    public TextMeshProUGUI playerOneNumberOfMoves;
    public TextMeshProUGUI playerOneTotalDamage;
    public TextMeshProUGUI playerOneCreaturesPlayed;
    public TextMeshProUGUI playerOneTrapsPlayed;
    public TextMeshProUGUI playerOneSpellsPlayed;
    public TextMeshProUGUI playerOneEnchantmentsPlayed;

    public TextMeshProUGUI playerTwoCardsPlayed;
    public TextMeshProUGUI playerTwoNumberOfMoves;
    public TextMeshProUGUI playerTwoTotalDamage;
    public TextMeshProUGUI playerTwoCreaturesPlayed;
    public TextMeshProUGUI playerTwoTrapsPlayed;
    public TextMeshProUGUI playerTwoSpellsPlayed;
    public TextMeshProUGUI playerTwoEnchantmentsPlayed;

    private int _amountOfTurns = 0;

    private int _playerOneCardsPlayed = 0;
    private int _playerOneNumberOfMoves = 0;
    private int _playerOneTotalDamage = 0;
    private int _playerOneCreaturesPlayed = 0;
    private int _playerOneTrapsPlayed = 0;
    private int _playerOneSpellsPlayed = 0;
    private int _playerOneEnchantmentsPlayed = 0;

    private int _playerTwoCardsPlayed = 0;
    private int _playerTwoNumberOfMoves = 0;
    private int _playerTwoTotalDamage = 0;
    private int _playerTwoCreaturesPlayed = 0;
    private int _playerTwoTrapsPlayed = 0;
    private int _playerTwoSpellsPlayed = 0;
    private int _playerTwoEnchantmentsPlayed = 0;

    public void IncrementAnalytic(Analytics analytic, int value) {
        switch (analytic) {
            case Analytics.AMOUNT_OF_TURNS:
                _amountOfTurns += value;
                amountOfTurns.text = _amountOfTurns.ToString();
                break;

            case Analytics.PLAYER_ONE_CARDS_PLAYED:
                _playerOneCardsPlayed += value;
                playerOneCardsPlayed.text = _playerOneCardsPlayed.ToString();
                break;

            case Analytics.PLAYER_ONE_NUMBER_OF_MOVES:
                _playerOneNumberOfMoves += value;
                playerOneNumberOfMoves.text = _playerOneNumberOfMoves.ToString();
                break;

            case Analytics.PLAYER_ONE_TOTAL_DAMAGE:
                _playerOneTotalDamage += value;
                playerOneTotalDamage.text = _playerOneTotalDamage.ToString();
                break;

            case Analytics.PLAYER_ONE_CREATURES_PLAYED:
                _playerOneCreaturesPlayed += value;
                playerOneCreaturesPlayed.text = _playerOneCreaturesPlayed.ToString();
                break;

            case Analytics.PLAYER_ONE_TRAPS_PLAYED:
                _playerOneTrapsPlayed += value;
                playerOneTrapsPlayed.text = _playerOneTrapsPlayed.ToString();
                break;

            case Analytics.PLAYER_ONE_SPELLS_PLAYED:
                _playerOneSpellsPlayed += value;
                playerOneSpellsPlayed.text = _playerOneSpellsPlayed.ToString();
                break;

            case Analytics.PLAYER_ONE_ENCHANTMENTS_PLAYED:
                _playerOneEnchantmentsPlayed += value;
                playerOneEnchantmentsPlayed.text = _playerOneEnchantmentsPlayed.ToString();
                break;

            case Analytics.PLAYER_TWO_CARDS_PLAYED:
                _playerTwoCardsPlayed += value;
                playerTwoCardsPlayed.text = _playerTwoCardsPlayed.ToString();
                break;

            case Analytics.PLAYER_TWO_NUMBER_OF_MOVES:
                _playerTwoNumberOfMoves += value;
                playerTwoNumberOfMoves.text = _playerTwoNumberOfMoves.ToString();
                break;

            case Analytics.PLAYER_TWO_TOTAL_DAMAGE:
                _playerTwoTotalDamage += value;
                playerTwoTotalDamage.text = _playerTwoTotalDamage.ToString();
                break;

            case Analytics.PLAYER_TWO_CREATURES_PLAYED:
                _playerTwoCreaturesPlayed += value;
                playerTwoCreaturesPlayed.text = _playerTwoCreaturesPlayed.ToString();
                break;

            case Analytics.PLAYER_TWO_TRAPS_PLAYED:
                _playerTwoTrapsPlayed += value;
                playerTwoTrapsPlayed.text = _playerTwoTrapsPlayed.ToString();
                break;

            case Analytics.PLAYER_TWO_SPELLS_PLAYED:
                _playerTwoSpellsPlayed += value;
                playerTwoSpellsPlayed.text = _playerTwoSpellsPlayed.ToString();
                break;

            case Analytics.PLAYER_TWO_ENCHANTMENTS_PLAYED:
                _playerTwoEnchantmentsPlayed += value;
                playerTwoEnchantmentsPlayed.text = _playerTwoEnchantmentsPlayed.ToString();
                break;
        }
    }

    public void SetAnalytic(Analytics analytic, string value) {
        Debug.Log("Setting analytics");
        switch (analytic) {
            case Analytics.TOTAL_TIME:
                totalTime.text = value;
                break;

            case Analytics.PLAYER_WON:
                Debug.Log("Setting player won");
                playerWon.text = value;
                break;
        }
    }

    public void Track(Analytics analytic, int value = 1) {
        TrackAnalyticServerRpc(analytic, value);
    }

    [ServerRpc(RequireOwnership=false)]
    public void TrackAnalyticServerRpc(Analytics analytic, int value) {
        TrackAnalyticClientRpc(analytic, value);
    }

    [ClientRpc]
    public void TrackAnalyticClientRpc(Analytics analytic, int value) {
        TrackAnalytic(analytic, value);
    }

    public void TrackAnalytic(Analytics analytic, int value = 1) {
        AnalyticsManager.Instance.IncrementAnalytic(analytic, value);
    }
}