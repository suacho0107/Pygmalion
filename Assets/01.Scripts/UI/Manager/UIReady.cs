using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIReady : MonoBehaviour
{
    PlayerMove playerMove;
    void Start()
    {
        playerMove = FindObjectOfType<PlayerMove>();

        if (null != playerMove && !playerMove.IsMoved)
            playerMove.IsMoved = true;
    }

    void Update()
    {
        
    }
}
