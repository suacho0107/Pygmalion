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

    private PlayerMove playerMove;

    public override void Enter()
    {
        //Debug.Log("Enter: TutorialFadeEffect");

        if (isFadeIn)
        {
            fadeEffect.FadeIn(OnAfterFadeEffect);
        }
        else
        {
            // IsMoved로는 플레이어가 멈추지 않음.
            playerMove = FindObjectOfType<PlayerMove>();
            if (null != playerMove)
                playerMove.IsMoved = false;

            fadeEffect.FadeOut(OnAfterFadeEffect);
        }
    }

    public void OnAfterFadeEffect()
    {
        isCompleted = true;
    }

    public override void Execute(TutorialController controller)
    {
        if (isCompleted)
        {
            // 현재 튜토리얼 행동 종료
            controller.SetNextTutorial();

            if (null != playerMove)
                playerMove.IsMoved = true;
        }
    }

    public override void Exit()
    {
        //Debug.Log("Exit: TutorialFadeEffect");
    }
}
