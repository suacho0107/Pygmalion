using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestNPC : NPC
{
    [Header("업무 의뢰서")]
    [SerializeField] private    PlayerDesk      playerDesk;

    [SerializeField] public     GameObject[]    sendChat;
    [SerializeField] private    GameObject      replyChat;

    [SerializeField] public    Button          replyButton;

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
    public bool canOff { get; private set; } = false;

    public int  locationIndex;

    private IRequestState currentState;

    private int processNum = 0;
    private bool buttonOff = false;

    private void Start()
    {
        if (replyChat != null)
        {
            replyChat.SetActive(false);
        }

        locationText.text = UIManager.u_instance.locationList[UIManager.u_instance.stageIndex];

        ChangeState(new RequestReadyState());
    }

    void Update()
    {
        if (canSend)
        {
            if (Input.GetKeyDown(KeyCode.Space))
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

    void LateUpdate()
    {
        currentState?.Update();
    }

    public void SaveRequestNPCData()
    {
        if (!isObject)
        {
            npcData.isDialogueChanged = isDialogueChanged;
            npcData.currentIndex = currentIndex;
            npcData.dialogueFileName = dialogueFileName;
            npcData.selectFileName = selectFileName;
            npcData.isInteract = isInteract;

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
        //if (canSend)
        //{
        //    replyChat.SetActive(true);
        //    canSend = false;
        //    isAcceptRequest = true;
        //    replyButton.interactable = false;
        //    playerDesk.startON = true;

        //    ChangeState(new RequestEndState());
        //}
    }

    public void RequestOff()
    {
        processNum++;

        if (1 == processNum)
            canOff = true;
    }

    public void RequestOffEx()
    {
        processNum++;

        if (1 == processNum)
            canOff = true;
    }
}