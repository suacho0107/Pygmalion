using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialFadeEffect : TutorialBase
{
    [SerializeField]
    private FadeEffect fadeEffect;
    [SerializeField]
    private bool isFadeIn = false;
    public bool isCompleted = false;

    public override void Enter()
    {
        if (isFadeIn == true)
        {
            fadeEffect.FadeIn(OnAfterFadeEffect);
        }
        else
        {
            fadeEffect.FadeOut(OnAfterFadeEffect);
        }
    }

    public void OnAfterFadeEffect()
    {
        isCompleted = true;
    }

    public override void Execute(TutorialController controller)
    {
        if (isCompleted == true)
        {
            // 현재 튜토리얼 행동 종료
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
    }
}
