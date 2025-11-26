using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour //합병 후 DialogueManager_Jiyun -> DialogueManager로 변경
{
    [Header("Dialogue Data")]
    public Dialogue[] dialogues;
    public Select[] selects;

    #region References
    [Header("Managers & References")]
    DialogueUI dialogueUI; //합병 후 DialogueUI_Jiyun -> DialogueUI로 변경
    InteractionEvent interactionEvent;
    PlayerMove playerMove; //플레이어 FSM과 연결, 추가 코드
    NPC npc; //= currentNPCZ
    StageNPC stageNpc;
    Statue statue;
    StatueScore statueScore;
    MuseumLobbyCSV csv;
    #endregion

    #region variables
    [Header("Dialogue State Flags")]
    public bool isDialogue = false; //현재 대화 중인지
    public bool isExplain = false; //설명 대사인지 구분
    public bool isSelect = false; //선택지 모드 진입 여부
    public bool isNext = false; //특정 키 입력 대기
    public bool isPopup = false; //팝업 대화 상태
    public bool isEnd = false; //대화 종료 여부

    [Header("Message Mode")]
    public bool isMessage = false; //CSV파일 없는 짧은 대사 출력
    public string message; //출력할 메시지 내용
    #endregion

    #region Unity Methods
    void Awake()
    {
        dialogueUI = FindObjectOfType<DialogueUI>();
        playerMove = FindObjectOfType<PlayerMove>(); //플레이어 FSM과 연결, 추가 코드
        //stageNpc = FindObjectOfType<StageNPC>();
        //statue = FindObjectOfType<Statue>();
        statueScore = FindObjectOfType<StatueScore>();
    }

    void Start()
    {
        if (dialogueUI.ItemImage != null)
        {
            dialogueUI.ItemImage.SetActive(false);
        } 

        dialogueUI.dialoguePanel.SetActive(false);
        dialogueUI.namePanel.SetActive(false);
        dialogueUI.selectButtons.SetActive(false);

        #region Image Popup
        if (null != dialogueUI.Images)
        {
            foreach (var image in dialogueUI.Images)
            {
                image.gameObject.SetActive(false);
            }
        }

        #endregion
    }

    void Update()
    {
        if (isDialogue && !isSelect)
        {
            DialogueInputHandler();
        }
    }
    #endregion

    private void DialogueInputHandler()
    {
        //0. Space 입력 아니면 return
        if (!Input.GetKeyDown(KeyCode.Space) || dialogueUI.ignoreInputFrame)
        {
            return;
        }

        //1. ContextTyping() 중이면 Skip
        if (dialogueUI.isContextTyping &&!isSelect)
        {
            dialogueUI.skipContextTyping = true;
            Debug.Log($"skipContextTyping = {dialogueUI.skipContextTyping}");
            return;
        }

        //2. isNext 아니면 return
        if (!isNext)
        {
            return;
        }

        //다음 대사 진행 전 초기화
        isNext = false;
        dialogueUI.dialogueText.text = "";
        dialogueUI.descriptionText.text = "";

        //3. csv 없는 짧은 메시지
        if (isMessage)
        {
            dialogueUI.EndMessage();
        }

        //4. csv 있는 대사
        var currentLine = dialogues[dialogueUI.lineCount];
        var currentContext = dialogueUI.contextCount;

        //4-1. skipNum 처리
        if (!string.IsNullOrEmpty(currentLine.skipNum[currentContext]))
        {
            if (int.TryParse(currentLine.skipNum[currentContext], out int skipLine))
            {
                dialogueUI.lineCount = skipLine - 2; //왜 -2인지는 모르겠는데.... 쨌든 이렇게 하면 제대로 돌아감
                dialogueUI.contextCount = 0;
            }
        }

        //4-2. eventNum 처리 (선택지 있는 대화)
        if (!string.IsNullOrEmpty(currentLine.eventNum[currentContext]))
        {
            EventDialogueHandler(currentLine.eventNum[currentContext]);
        }
        else //4-3. 일반 대사 처리
        {
            NormalDialogueHandler();
        }

        //else //기본(CSV파일 사용 대화)
        //{
        //    //skipNum이 있으면
        //    if (!string.IsNullOrEmpty(dialogues[dialogueUI.lineCount].skipNum[dialogueUI.contextCount]))
        //    {
        //        int skipLine;
        //        if (int.TryParse(dialogues[dialogueUI.lineCount].skipNum[dialogueUI.contextCount], out skipLine))
        //        {
        //            dialogueUI.lineCount = skipLine - 2; //왜 -2인지는 모르겠는데.... 쨌든 이렇게 하면 제대로 돌아감
        //            dialogueUI.contextCount = 0;
        //        }
        //    }

        //    //eventNum이 있으면: 선지대화
        //    if (!string.IsNullOrEmpty(dialogues[dialogueUI.lineCount].eventNum[dialogueUI.contextCount]))
        //    {
        //        if (!string.IsNullOrEmpty(npc.selectFileName)) //selectFileName 유무 확인
        //        {
        //            interactionEvent.LoadSelect(npc.selectFileName);

        //            //Guard
        //            if (interactionEvent == null)
        //            {
        //                Debug.LogError("interactionEvent is NULL!!!");
        //                return;
        //            }
        //            if (interactionEvent.Select == null)
        //            {
        //                Debug.LogError($"interactionEvent.Select is NULL. Failed to load select file: {npc.selectFileName}");
        //                return;
        //            }
        //            if (interactionEvent.Select.selects == null)
        //            {
        //                Debug.LogError("interactionEvent.Select.selects is NULL.");
        //                return;
        //            }

        //            if (interactionEvent.Select != null && interactionEvent.Select.selects.Length > 0)
        //            {
        //                int eNum;
        //                if (int.TryParse(dialogues[dialogueUI.lineCount].eventNum[dialogueUI.contextCount], out eNum))
        //                {
        //                    // eventNum은 1부터 시작한다고 가정 → 배열은 0부터 시작하므로 -1
        //                    int index = eNum - 1;

        //                    if (interactionEvent.Select != null &&
        //                        index >= 0 &&
        //                        index < interactionEvent.Select.selects.Length)
        //                    {
        //                        //사용하는 Select 하나만 넘기기
        //                        var selectData = interactionEvent.Select.selects[index];
        //                        dialogueUI.ShowSelect(selectData);
        //                    }
        //                    else
        //                    {
        //                        Debug.LogWarning($"Select index {index} is invalid.");
        //                    }
        //                }

        //            }
        //            else
        //            {
        //                Debug.LogWarning("Selects could not be loaded or are empty.");
        //            }
        //        }
        //        else
        //        {
        //            Debug.LogWarning("npc.selectFileName is Null or Empty");
        //        }
        //    }
        //    else //eventNum이 없으면: 선지 없는 그냥 대화
        //    {
        //        if (++dialogueUI.contextCount < dialogues[dialogueUI.lineCount].contexts.Length) //line의 contexts.Length 미만이면
        //        {
        //            dialogueUI.DialogueWriter(); //같은 name 밑의 context만 변경
        //        }
        //        else //line을 넘겨야 하면
        //        {
        //            if (!isExplain) //설명문이 아니면 line도 넘김
        //            {
        //                dialogueUI.contextCount = 0;
        //                if (++dialogueUI.lineCount < dialogues.Length)
        //                {
        //                    dialogueUI.DialogueWriter(); //name과 context 모두 변경
        //                }
        //                else
        //                {
        //                    dialogueUI.EndDialogue();
        //                }
        //            }
        //            else //설명문이면 걍 끝내
        //            {
        //                dialogueUI.EndDialogue();
        //            }
        //        }
        //    }
        //}
    }

    private void EventDialogueHandler(string eventNumStr)
    {
        //Guard
        if (string.IsNullOrEmpty(npc.selectFileName))
        {
            Debug.LogWarning($"npc.SelectFileName({npc.selectFileName}) is Null or Enpty");
            return;
        }
        interactionEvent.LoadSelect(npc.selectFileName);

        //Guard
        if (interactionEvent?.Select?.selects == null || interactionEvent.Select.selects.Length == 0)
        {
            Debug.LogWarning("Selects could not be loaded or are empty");
            return;
        }

        if (!int.TryParse(eventNumStr, out int eNum))
        {
            return;
        }

        int index = eNum - 1; // 1-based → 0-based

        if (index >= 0 && index < interactionEvent.Select.selects.Length)
        {
            var selectData = interactionEvent.Select.selects[index];
            dialogueUI.ShowSelect(selectData);
        }
        else
        {
            Debug.LogWarning($"Selectindex {index} is invalied.");
        }
    }

    private void NormalDialogueHandler()
    {
        var line = dialogues[dialogueUI.lineCount];

        if (++dialogueUI.contextCount < line.contexts.Length)
        {
            dialogueUI.DialogueWriter(); //같은 line 밑의 context만 변경
        }
        else
        {
            dialogueUI.contextCount = 0;

            if (!isExplain)
            {
                if (++dialogueUI.lineCount < dialogues.Length)
                {
                    dialogueUI.DialogueWriter(); //다음 line으로 이동
                }
                else
                {
                    dialogueUI.EndDialogue(); //대화 종료
                }
            }
            else //설명문이면
            {
                dialogueUI.EndDialogue(); //대화 종료
            }
        }
    }

    public void SetNPC(NPC _npc)
    {
        npc = _npc; //NPC 인스턴스 설정
        dialogueUI.SetNPC(_npc);

        interactionEvent = npc.interactionEvent; //npc의 interactionEvent 가져오기

        if (npc is StageNPC)
        {
            stageNpc = npc as StageNPC;
            statue = null;
            //Debug.Log("** StageNPC **");
        }
        else if (npc is Statue)
        {
            stageNpc = null;
            statue = npc as Statue;
            //Debug.Log("** Statue **");
        }
        else if (npc is NPC)
        {
            stageNpc = null;
            statue = null;
            //Debug.Log("** NPC **");
        }
        else
        {
            npc = null;
            stageNpc = null;
            statue = null;
            Debug.LogError("SetNPC: NPC is null.");
        }
    }

    public NPC CurrentNPC => npc;

    public bool DialogueTrue()
    {
        if (isDialogue) return true;
        else return false;
    }

    public bool MessageTrue()
    {
        if (isMessage) return true;
        else return false;
    }

    public void SaveData()
    {
        if (npc is StageNPC stageNpcInstance)
        {
            stageNpcInstance.SaveStageNPCData();
        }
        else if (npc is Statue statueInstance)
        {
            statueInstance.SaveStatueData();
        }
        else if (npc is NPC npcInstance)
        {
            npcInstance.SaveNPCData();
        }
    }
}