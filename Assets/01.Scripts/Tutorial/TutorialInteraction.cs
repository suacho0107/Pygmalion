using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInteraction : TutorialBase
{
    [SerializeField]
    private GameObject  interactionObject;
    [SerializeField]
    private bool        isTriggerOn = false;

    public override void Enter()
    {
    }

    public override void Execute(TutorialController controller)
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isTriggerOn)
            {
                interactionObject.gameObject.SetActive(true);
            }
            else
            {
                interactionObject.gameObject.SetActive(false);
            }
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}
