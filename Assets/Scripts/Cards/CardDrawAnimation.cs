using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Singletons;

public class CardDrawAnimation : Singleton<CardDrawAnimation>
{
    // Start is called before the first frame update
    private Vector3 p1DrawStartLocation;
    private Vector3 p2DrawStartLocation;
    private Vector3 player;
    private int player2Adjustment = 1;
    //PlayerHand.Instance.GetPlayerHandArea
    private GameObject gameboardDeckPrefab;
    private float animationDuration = 2f;
    private float[] animationProgress;
    private bool[] animating;
    private GameObject[] deck;
    private int animationsToDo=0;
    private int totalAllowedAnimations = 5;
    void Awake()
    {
        gameboardDeckPrefab = DeckManager.Instance.GameboardDeckPrefab;
        animating = new bool[totalAllowedAnimations];
        animationProgress = new float[totalAllowedAnimations];
        deck = new GameObject[totalAllowedAnimations];
       p1DrawStartLocation = Gameboard.Instance.GetPlayerOneDeckArea().transform.position;
       p2DrawStartLocation = Gameboard.Instance.GetPlayerTwoDeckArea().transform.position;

    }
    // Update is called once per frame
    void Update()
    {
        for (int n = 0; n < totalAllowedAnimations; n++)
        {
            animationProgress[n] += Time.deltaTime / animationDuration;
        }
        PlayerDrawCheck();
    }
    public void PlayerDrawCheck()
    {
        if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_ONE)
        {
            player = p1DrawStartLocation;

        }
        else if (GameManager.Instance.GetCurrentPlayer() == Players.PLAYER_TWO)
        {
            player = p2DrawStartLocation;
            player2Adjustment = -1;
        }
        for (int a = 0; a < totalAllowedAnimations; a++) {
            if (animating[a])
            {
                deck[a].transform.position = Vector3.Lerp(player,
                   (player + new Vector3(0f, 0f, -30f *player2Adjustment)), animationProgress[a]);
                if (animationProgress[a] > 1f)
                {
                    if (deck[a].transform.position == player + new Vector3(0f, 0f, -40f))
                    {
                        //can do something once it reaches the end of path
                    }
                    animating[a] = false;
                    Destroy(deck[a]);
                }
            }else if (!animating[a] && animationsToDo>0)
            {
                animationsToDo--;
                animationProgress[a] = 0f;
                deck[a] = Instantiate(gameboardDeckPrefab, player, Quaternion.identity);
                animating[a] = true;
            }
            
        }
    }
    public void PlayerDraw()
    {
            animationsToDo++;
    }
}
