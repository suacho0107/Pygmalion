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
        Debug.Log("Enter: TutorialInteraction");

        playerMove = FindObjectOfType<PlayerMove>();

        if (isInteractOn && playerMove != null)
        {
            playerMove.IsMoved = false;
            playerMove.IsAnimation = false;
            playerMove.WalkSound.Stop();
            playerMove.SetIdleState();
        }
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
            }
            else
            {
                if (interactionObject.TryGetComponent<RequestNPC>(out var requestNPC))
                {
                    if (requestNPC.canOff)
                    {
                        interactionObject.gameObject.SetActive(false);
                        if (playerMove != null)
                        {
                            playerMove.IsMoved = true;
                            playerMove.IsAnimation = true;
                        }

                        UIManager.u_instance.Set_UIState(Define.UI.UIState.Start);
                    }
                    else
                        return;
                }
                else
                {
                    interactionObject.gameObject.SetActive(false);
                    if (playerMove != null)
                        playerMove.IsMoved = true;
                }
            }

            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit: TutorialInteraction");
        isTrigger = false;
    }
}
