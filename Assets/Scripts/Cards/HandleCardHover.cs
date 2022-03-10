using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleCardHover : MonoBehaviour
{
    public void HandleOnCardHoverEnter() {
        PlayerHand.Instance.SetCardIsHovered(gameObject);
    }

    public void HandleOnCardHoverExit() {
        PlayerHand.Instance.RemoveCardIsHovered();
    }
}
