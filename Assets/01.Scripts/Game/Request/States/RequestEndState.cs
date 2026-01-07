using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestEndState : IRequestState
{
    private RequestNPC request;

    private bool isEnd = false;

    public void Enter(RequestNPC request)
    {
        this.request = request;
        request.ChangeDialogueFile(1);
        request.explainNum = "";
        request.InteractText.text = "메세지 입력";
        request.InteractText.color = new Color(103f / 255f, 102f / 255f, 102f / 255f);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isEnd)
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
