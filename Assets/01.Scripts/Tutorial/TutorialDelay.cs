using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDelay : TutorialBase
{
    [SerializeField] float fDelayTime = 0.5f;

    private bool isTrigger = false;

    public override void Enter()
    {
        Debug.Log("Enter: TutorialDelay");
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
        yield return new WaitForSeconds(fDelayTime);
        controller.SetNextTutorial();
    }


    public override void Exit()
    {
        Debug.Log("Exit: TutorialDelay");
    }
}
