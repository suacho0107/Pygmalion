using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager_Jiyun : MonoBehaviour
{
    //public List<GameObject> Images;
    //public List<GameObject> Portraits;

    //public GameObject dialoguePanel;
    //public GameObject descriptionPanel;
    //public GameObject namePanel;
    //public GameObject ItemImage;

    //public Text dialogueText;
    //public Text descriptionText;
    //public Text nameText;

    //public GameObject selectButtons;
    //private List<GameObject> selectButtonList = new List<GameObject>();
    //public Button selectBtn1;
    //public Button selectBtn2;
    //public Button selectBtn3;
    //public Button selectBtn4;

    //public GameObject selectTexts;
    //private List<GameObject> selectTextList = new List<GameObject>();
    //public Text selectText1;
    //public Text selectText2;
    //public Text selectText3;
    //public Text selectText4;

    Dialogue[] dialogues;
    Select[] selects;

    InteractionEvent interactionEvent;
    NPC npc; //=currentNPC
    StageNPC stageNpc;
    Statue statue;
    public PlayerMove playerMove; //플레이어 FSM과 연결, 추가 코드
    StatueScore statueScore;
    MuseumLobbyCSV csv;

    public bool isEnd = false;

    bool isDialogue = false;
    bool isNext = false;        //특정 키 입력 대기
    bool isSelect = false;
    bool isExplain = false;     //설명대사인지 구분
    bool isPopup = false;

    bool isMessage = false; // CSV파일 없는 짧은 대사 출력
    string message;

    int lineCount = 0; //대화 카운트
    int contextCount = 0; //대사 카운트

    public void SetNPC(NPC _npc)
    {
        npc = _npc; //NPC 인스턴스 설정

        interactionEvent = npc.interactionEvent; //npc의 interactionEvent 가져오기

        if (npc is StageNPC)
        {
            stageNpc = npc as StageNPC;
            statue = null;
        }
        else if (npc is Statue)
        {
            statue = npc as Statue;
            stageNpc = null;
        }
        else if (npc is NPC)
        {
            stageNpc = null;
            statue = null;
        }
        else
        {
            npc = null;
            stageNpc = null;
            statue = null;
            Debug.LogError("SetNPC: NPC is null.");
        }
        //if (npc != null)
        //{
        //    interactionEvent = npc.interactionEvent; //npc의 interactionEvent 가져오기
        //}
        //else
        //{
        //    Debug.LogError("SetNPC: NPC is null.");
        //}
    }

    private void Start()
    {       

        playerMove = FindObjectOfType<PlayerMove>(); //플레이어 FSM과 연결, 추가 코드
        statueScore = FindObjectOfType<StatueScore>();
        stageNpc = FindObjectOfType<StageNPC>();
        statue = FindObjectOfType<Statue>();
    }

    private void Update()
    {
        if (isDialogue && isNext && !isSelect)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isNext = false;
                dialogueText.text = "";
                descriptionText.text = "";

                if (isMessage)  // CSV파일 없는 짧은 대사 출력 시(아이템 습득 등)
                {
                    EndMessage();
                }
                else // 기본(CSV파일 사용 대화)
                {
                    //skipNum이 있으면
                    if (!string.IsNullOrEmpty(dialogues[lineCount].skipNum[contextCount]))
                    {
                        int skipLine;
                        if (int.TryParse(dialogues[lineCount].skipNum[contextCount], out skipLine))
                        {
                            lineCount = skipLine - 2; //왜 -2인지는 모르겠는데.... 쨌든 이렇게 하면 제대로 돌아감
                            contextCount = 0;
                        }
                    }

                    //eventNum이 있으면: 선지대화
                    if (!string.IsNullOrEmpty(dialogues[lineCount].eventNum[contextCount]))
                    {
                        if (!string.IsNullOrEmpty(npc.selectFileName)) //selectFileName 유무 확인
                        {
                            interactionEvent.LoadSelect(npc.selectFileName);

                            if (interactionEvent.Select != null && interactionEvent.Select.selects.Length > 0)
                            {
                                //한 대화에 선지대사 2번인 거 고쳐보려다 일단 말았음
                                //int eNum =  int.Parse(dialogues[lineCount].eventNum[contextCount]);
                                //Debug.Log("ShowSelect 전: " + interactionEvent.Select.selects);
                                ShowSelect(interactionEvent.Select.selects);
                            }
                            else
                            {
                                Debug.LogWarning("Selects could not be loaded or are empty.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning("npc.selectFileName 공백");
                        }
                    }
                    else //eventNum이 없으면: 선지 없는 그냥 대화
                    {
                        if (++contextCount < dialogues[lineCount].contexts.Length) //line의 contexts.Length 미만이면
                        {
                            StartCoroutine(DialogueWriter()); //같은 name 밑의 context만 변경
                        }
                        else //line을 넘겨야 하면
                        {
                            if (!isExplain) //설명문이 아니면 line도 넘김
                            {
                                contextCount = 0;
                                if (++lineCount < dialogues.Length)
                                {
                                    StartCoroutine(DialogueWriter()); //name과 context 모두 변경
                                }
                                else
                                {
                                    EndDialogue();
                                }
                            }
                            else //설명문이면 걍 끝내
                            {
                                EndDialogue();
                            }
                        }
                    }
                }
            }
        }
    }


    /*
    private void DialogueButtonInputHandler()
    {
        int previousIndex = currentDialogueButtonIndex;

        // 상하 이동 (2열 기준)
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentDialogueButtonIndex - 2 >= 0)
            {
                currentDialogueButtonIndex -= 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentDialogueButtonIndex + 2 < dialogueButtonList.Count)
            {
                currentDialogueButtonIndex += 2;
            }
        }

        // 좌우 이동 (같은 행 내에서만 이동)
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentDialogueButtonIndex % 2 == 1)
            {
                currentDialogueButtonIndex -= 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentDialogueButtonIndex % 2 == 0 &&
                currentDialogueButtonIndex + 1 < dialogueButtonList.Count)
            {
                currentDialogueButtonIndex += 1;
            }
        }

        // 선택
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentDialogueButtonIndex < dialogueButtonList.Count)
            {
                string selectedButtonText = dialogueButtonList[currentDialogueButtonIndex].GetComponentInChildren<Text>().text;

                if (selectedButtonText == "공격한다")
                {
                    player.AttackButton();
                }
                else if (selectedButtonText == "소지품을 확인한디")
                {
                    player.InventoryButton();
                }
                else if (selectedButtonText == "도망친다")
                {
                    player.RunButton();
                }

                dialogueButtons.SetActive(false);
            }
        }

        if (previousIndex != currentDialogueButtonIndex)
        {
            HighlightDialogueButton();
        }
    }

    private bool isThereButton(int pageIndex, int buttonIndex, int increraseButtonIndex)
    {
        int targetButtonIndex = buttonIndex + increraseButtonIndex;

        if (targetButtonIndex > 3)
        {
            targetButtonIndex -= 4;
        }

        int lastButtonIndex = enemy.parts.Count - pageIndex * 4;

        return targetButtonIndex < lastButtonIndex;        
    }

    private void HighlightDialogueButton()
    {
        for (int i = 0; i < dialogueButtonList.Count; i++)
        {
            Image image = dialogueButtonList[i].GetComponent<Image>();
            image.sprite = (i == currentDialogueButtonIndex) ? ButtonHighlighted : ButtonDefault;
        }
    }
    */

    public void OnSelectButtonClicked(int selectedIndex, int currentIndex) // 판별 매개변수 추가(currentIndex)
    {
        if (npc is Statue selectedStatue)
        {
            if (!selectedStatue.isChecked) // 첫 번째 상호작용(조사): 선지 2개 출력
            {
                if (currentIndex == 0)
                {
                    statueScore.checkedCount += 1;
                    statueScore.SaveScore();

                    selectedStatue.isChecked = true;
                    selectedStatue.SaveStatueData();
                    Debug.Log("isChecked");
                }
            }
            else // 두 번째 상호작용(판별): 선지 4개 출력
            {
                if (currentIndex == 0)
                {
                    statueScore.checkedCount += 1;
                    statueScore.SaveScore();

                    selectedStatue.isChecked = true;
                    selectedStatue.SaveStatueData();
                }
                else if (currentIndex == 1)
                {
                    if (selectedStatue.isEnemy)
                    {// 건드린다 --> 정답
                        selectedStatue.isJudged = true;
                        selectedStatue.isCorrect = true;
                        selectedStatue.currentIndex = 3;
                        selectedStatue.explainNum = "1";
                    }
                    else
                    {// 건드린다 --> 오답
                        selectedStatue.isJudged = true;
                        selectedStatue.isCorrect = false;
                    }
                }
                else if (currentIndex == 2)
                {
                    if (selectedStatue.isEnemy)
                    {// 이상 없음 --> 오답
                        selectedStatue.isJudged = true;
                        selectedStatue.isCorrect = false;
                        selectedStatue.explainNum = "2";
                    }
                    else
                    {// 이상 없음 --> 정답
                        selectedStatue.isJudged = true;
                        selectedStatue.isCorrect = true;
                        selectedStatue.explainNum = "3";
                    }
                }
            }
        }

        int targetLineCount = (int)selectedIndex - 1;

        if (targetLineCount >= 0 && targetLineCount < dialogues.Length) //targetLineCount가 0 이상이고, dialogues 안에 있으면
        {
            lineCount = targetLineCount; //lineCount 강제 변경
            contextCount = 0; //contextCount 초기화
            EndSelect(); //Select End하기
            StartCoroutine(DialogueWriter()); //변경한 lineCount, contextCount로 DialogueWriter 실행
        }
        else if (selectedIndex == 0) // 선지 선택 직후 대화 종료
        {
            EndSelect();
            EndDialogue();
        }
        else
        {
            Debug.LogError("Selected dialogue index is out of bounds. Ending dialogue.");
            EndDialogue();
        }
    }


    void SetUIStateEnd()
    {
        UIManager.u_instance.SetUIState(Define.UI.UIState.End);
    }

    void EndMessage()
    {
        isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;
        isExplain = false;
        isMessage = false;

        playerMove.ActiveInteract = false;

        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        playerMove.ActiveInteract = false; // 추가 코드
    }

    void EndDialogue()
    {
        //초기화
        isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;
        isExplain = false;
        npc.isInteract = true; // 미술관장
        isEnd = true;

        playerMove.ActiveInteract = false;

        if (npc is StageNPC selectedNPC)
        {
            // 미술관장
            if (selectedNPC.dialogueFileName == "Tutorial2_dialogue")
            {
                selectedNPC.TutorialFin();
            }
            else if (selectedNPC.dialogueFileName == "Museum-Guard1_dialogue")
            {
                InventoryUI.instance.GetQuestItem(10402);
                selectedNPC.questEnd = true;
            }
        }

        if (npc is Statue selectedStatue)
        {
            selectedStatue.CheckResult();

            // 판별 결과 UI 출력
            if (npc.dialogueFileName == "Check3_dialogue")
            {
                Invoke("SetUIStateEnd", 1.5f);
            }
        }

        SaveData();

        dialoguePanel.SetActive(false);
        descriptionPanel.SetActive(false);
        namePanel.SetActive(false);

        if (ItemImage != null)
        {
            ItemImage.SetActive(false);
        }

        // 모든 이미지 비활성화
        foreach (var image in Images)
        {
            image.SetActive(false);
        }

        foreach (var portrait in Portraits)
        {
            portrait.SetActive(false);
        }
    }

    void EndSelect()
    {
        isSelect = false;
        selects = null;

        selectBtn1.gameObject.SetActive(false);
        selectBtn2.gameObject.SetActive(false);
        selectText1.gameObject.SetActive(false);
        selectText2.gameObject.SetActive(false);

        selectBtn3.gameObject.SetActive(false);
        selectBtn4.gameObject.SetActive(false);
        selectText3.gameObject.SetActive(false);
        selectText4.gameObject.SetActive(false);
        //SaveData();
    }

    #region 팝업 이미지 구현
    

    public void ItemPopup()
    {
        ItemImage.SetActive(true);
    }

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

    void SaveData()
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