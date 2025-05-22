using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDelay : TutorialBase
{
    private bool isTrigger = false;

    public override void Enter()
    {
    }

    public override void Execute(TutorialController controller)
    {
        if (!isTrigger)
        {
            isTrigger = true;
            StartCoroutine(DelayNext(controller));
        }
    }

    IEnumerator DelayNext(TutorialController controller)
    {
        yield return new WaitForSeconds(0.5f);
        controller.SetNextTutorial();
    }


    public override void Exit()
    {
    }
}
