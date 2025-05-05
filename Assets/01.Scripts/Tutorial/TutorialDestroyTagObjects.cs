using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDestroyTagObjects : TutorialBase
{
    [SerializeField]
    private PlayerMove      playerController;
    [SerializeField]
    private GameObject[]    objectList;
    [SerializeField]
    private string          tagName;

    public override void Enter()
    {
        // 플레이어 이동, 공격 가능
        playerController.IsMoved = true;
        // playerController.IsAttacked = true;

        // 파괴해야할 오브젝트들 활성화
        for (int i = 0; i < objectList.Length; ++i)
        {
            objectList[i].SetActive(true);
        }
    }

    public override void Execute(TutorialController controller)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tagName);

        if (objects.Length == 0)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        playerController.IsMoved = false;
        // playerController.IsAttacked = false;
    }
}
