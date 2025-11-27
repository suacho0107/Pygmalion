using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInteraction : TutorialBase
{
    [SerializeField] GameObject  interactionObject;
    [SerializeField] bool        isInteractOn = false;
    [SerializeField] bool        isAuto = false;

    private PlayerMove  playerMove;
    private bool        isTrigger = false;

    public override void Enter()
    {
        playerMove = FindObjectOfType<PlayerMove>();
    }

    public override void Execute(TutorialController controller)
    {
        if (Input.GetKeyDown(KeyCode.F) || isAuto)
        {
            isTrigger = true;

            if (isInteractOn)
            {
                isInteractOn = false;
                interactionObject.gameObject.SetActive(true);
                
                if (playerMove != null)
                    playerMove.IsMoved = false;
            }
            else
            {
                interactionObject.gameObject.SetActive(false);
                if (playerMove != null)
                    playerMove.IsMoved = true;
            }

            //if (RequestNPC.r_instance.canOff)
                controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        isTrigger = false;
    }
}
