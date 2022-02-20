using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public void Spawn() {
        // GameboardObjectManager gbo = GameObject.Find("GameManager").GetComponentInChildren<GameboardObjectManager>();
        GameboardObjectManager.Instance.Spawn();
        // gbo.Spawn();
    }

    public void Move() {
        // GameboardObjectManager gbo = GameObject.Find("GameManager").GetComponentInChildren<GameboardObjectManager>();
        // gbo.Move();
        GameboardObjectManager.Instance.Move();
    }
}
