using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInteraction : TutorialBase
{
    [SerializeField]
    private GameObject  interactionObject;
    [SerializeField]
    private bool        isInteractOn = false;

    private bool        isTrigger = false;

    public override void Enter()
    {
    }

    public override void Execute(TutorialController controller)
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Space))
        {
            isTrigger = true;

            if (isInteractOn)
            {
                Debug.Log("true");
                isInteractOn = false;
                interactionObject.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("false");
                interactionObject.gameObject.SetActive(false);
            }
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        isTrigger = false;
    }
}
