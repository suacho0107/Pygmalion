using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInteraction : TutorialBase
{
    [SerializeField]
    private GameObject  interactionObject;
    [SerializeField]
    private bool        isInteractOn = false;

    private PlayerMove  playerMove;
    private bool        isTrigger = false;

    public override void Enter()
    {
        playerMove = FindObjectOfType<PlayerMove>();
    }

    public override void Execute(TutorialController controller)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isTrigger = true;

            if (isInteractOn)
            {
                Debug.Log("true");
                isInteractOn = false;
                interactionObject.gameObject.SetActive(true);
                playerMove.IsMoved = false;
            }
            else
            {
                Debug.Log("false");
                interactionObject.gameObject.SetActive(false);
                playerMove.IsMoved = true;
            }
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        isTrigger = false;
    }
}
