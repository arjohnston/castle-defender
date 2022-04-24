using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Singletons;

public class CardDrawAnimation : Singleton<CardDrawAnimation>
{
    // Start is called before the first frame update
    private Vector3 p1DrawStartLocation;
    private Vector3 p2DrawStartLocation;
    //PlayerHand.Instance.GetPlayerHandArea
    private GameObject gameboardDeckPrefab;
    private float animationDuration = 3f;
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
        for (int a = 0; a < totalAllowedAnimations; a++) {
            if(animationsToDo>1)
            if (animating[a])
            {
                deck[a].transform.position = Vector3.Lerp(p1DrawStartLocation,
                   (p1DrawStartLocation + new Vector3(0f, 0f, -40f)), animationProgress[a]);
                if (animationProgress[a] > 1f)
                {
                    if (deck[a].transform.position == p1DrawStartLocation + new Vector3(0f, 0f, -40f))
                    {
                        //can do something once it reaches the end of path
                    }
                    animating[a] = false;
                    Destroy(deck[a]);
                }
            }else if (!animating[a] && animationsToDo>=1)
            {
                animationsToDo--;
                animationProgress[a] = 0;
                deck[a] = Instantiate(gameboardDeckPrefab, p1DrawStartLocation, Quaternion.identity);
                animating[a] = true;
            }
            
        }
    }
    public void PlayerDraw()
    {
            animationsToDo++;
    }
}
