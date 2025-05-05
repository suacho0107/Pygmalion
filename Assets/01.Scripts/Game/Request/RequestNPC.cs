using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestNPC : NPC
{
    [Header("¾÷¹« ÀÇ·Ú¼­")]
    [SerializeField] private PlayerDesk playerDesk;
    [SerializeField] public GameObject[] sendChat;
    [SerializeField] private GameObject replyChat;
    [SerializeField] private Button replyButton;
    [SerializeField] private Text interactText;
    [SerializeField] private Text profileText;
    [SerializeField] private Text locationText;

    public Button       ReplyButton => replyButton;
    public Text         InteractText => interactText;
    public Text         ProfileText => profileText;

    public CompanyOfficeCSV csv2;
    public bool isStartTutorial { get; private set; } = false;
    public bool isAcceptRequest = false;
    public bool canSend = false;

    private IRequestState currentState;

    private void Start()
    {
        if (replyChat != null)
        {
            replyChat.SetActive(false);
        }

        ChangeState(new RequestReadyState());
    }

    void LateUpdate()
    {
        currentState?.Update();
    }

    public void ChangeState(IRequestState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    public void SendButton()
    {
        if (canSend)
        {
            replyChat.SetActive(true);
            canSend = false;
            isAcceptRequest = true;
            replyButton.interactable = false;
            playerDesk.startON = true;

            UIManager.u_instance.UpdateStartUI();

            Debug.Log($"RequestNPC: {locationText}");

            ChangeState(new RequestEndState());
        }
    }

}