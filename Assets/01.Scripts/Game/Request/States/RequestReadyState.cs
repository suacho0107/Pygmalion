using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestReadyState : IRequestState
{
    private RequestNPC  request;

    private int currentChatIndex = 0;

    public void Enter(RequestNPC request)
    {
        this.request = request;
    }

    public void Update()
    {
        if (request.csv2 == null) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            request.sendChat[currentChatIndex].SetActive(true);

            currentChatIndex++;
        }

        if (currentChatIndex >= request.sendChat.Length)
        {
            request.ChangeState(new RequestStartState());
        }
    }

    public void Exit()
    {
    }
}
