using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : TutorialBase
{
    [SerializeField]
    private Transform triggerObject;    // 플레이어가 충돌해야 하는 오브젝트
    // [SerializeField]
    // private Transform blockedObject; // 글로벌맵 스폰 방지

    private PlayerMove playerMove;

    public bool isTrigger { set; get; } = false;

    public override void Enter()
    {
        playerMove = FindObjectOfType<PlayerMove>();
    }

    public override void Execute(TutorialController controller)
    {
        // Trigger 오브젝트 활성화
        triggerObject.gameObject.SetActive(true);

        // TutorialTrigger 오브젝트의 위치를 플레이어와 동일하게 설정 (Trigger 오브젝트와 충돌할 수 있도록)
        transform.position = playerMove.transform.position;

        if (isTrigger == true)
        {
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        // Trigger 오브젝트 비활성화
        triggerObject.gameObject.SetActive(false);
        //playerMove.IsMoved = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.Equals(triggerObject))
        {
            isTrigger = true;

            col.gameObject.SetActive(false);
        }
    }
}
