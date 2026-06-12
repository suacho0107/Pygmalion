using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestEndState : IRequestState
{
    private RequestNPC request;

    private bool isEnd = false;
    private bool waitRelease = true;

    public void Enter(RequestNPC request)
    {
        this.request = request;
        request.ChangeDialogueFile(1);
        request.explainNum = "";
        request.InteractText.text = "메세지 입력";
        request.InteractText.color = new Color(103f / 255f, 102f / 255f, 102f / 255f);

        isEnd = false;
        waitRelease = true; // 상태 진입 때마다 대기 시작
    }

    public void Update()
    {
        if (request.ReplyButton.interactable == false)
        {
            if (waitRelease)
            {
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    waitRelease = false;    // 다음 입력부터 허용
                }
                return;
            }

            if (!isEnd && Input.GetKeyDown(KeyCode.Space))
            {
                request.StartDialogue();
                isEnd = true;
            }
        }
    }

    public void Exit()
    {
    }
}
