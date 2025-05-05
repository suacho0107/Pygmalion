using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestStartState : IRequestState
{
    private RequestNPC request;

    private bool isEnd = false;

    public void Enter(RequestNPC request)
    {
        this.request = request;

        
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isEnd)
            {
                isEnd = true;

                request.ReplyButton.interactable = true;
                request.ProfileText.text = "네 알겠습니다.";
                request.InteractText.text = "네 알겠습니다.";
                request.InteractText.color = Color.black;

                request.StartDialogue();
            }
            request.canSend = true;
        }
    }

    public void Exit()
    {
    }
}
