using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestNPC : NPC
{
    public static RequestNPC r_instance { get; private set; }

    [Header("업무 의뢰서")]
    [SerializeField] private    PlayerDesk      playerDesk;

    [SerializeField] public     GameObject[]    sendChat;
    [SerializeField] private    GameObject      replyChat;

    [SerializeField] private    Button          replyButton;

    [SerializeField] private    Text            interactText;
    [SerializeField] private    Text            profileText;
    [SerializeField] public     Text            locationText;

    public Button       ReplyButton => replyButton;
    public Text         InteractText => interactText;
    public Text         ProfileText => profileText;

    public CompanyOfficeCSV csv2;

    public bool isStartTutorial { get; private set; } = false;
    public bool isAcceptRequest = false;
    public bool canSend = false;
    public bool canOff = false;

    public int  locationIndex;

    private IRequestState currentState;

    private void Start()
    {
        if (replyChat != null)
        {
            replyChat.SetActive(false);
        }

        locationText.text = UIManager.u_instance.locationList[UIManager.u_instance.stageIndex];

        ChangeState(new RequestReadyState());
    }

    void LateUpdate()
    {
        currentState?.Update();
    }

    public new void SaveNPCData()
    {
        if (!isObject)
        {
            npcData.isDialogueChanged = isDialogueChanged;
            npcData.currentIndex = currentIndex;
            npcData.dialogueFileName = dialogueFileName;
            npcData.selectFileName = selectFileName;
            npcData.isInteract = isInteract;

            if (dialogueFileName == "request1_dialogue")
            {
                canOff = true;
            }

            string json = JsonUtility.ToJson(npcData);
            //File.WriteAllText(filePath, json);
            Debug.Log(gameObject.name + " / NPC 데이터 저장");
        }
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

            ChangeState(new RequestEndState());
        }
    }

}