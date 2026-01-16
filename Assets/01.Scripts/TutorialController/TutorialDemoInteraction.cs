using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDemoInteraction : TutorialBase
{
    public override void Enter()
    {
        Debug.Log("Enter: TutorialInteraction");
    }

    public override void Execute(TutorialController controller)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit: TutorialInteraction");
    }
}
