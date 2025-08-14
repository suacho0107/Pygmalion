using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager_Legacy: MonoBehaviour
{
    [SerializeField] List<GameObject> Images;
    [SerializeField] List<GameObject> Portraits;

    [SerializeField] GameObject dialoguePanel;
    [SerializeField] GameObject descriptionPanel;
    [SerializeField] GameObject namePanel;
    [SerializeField] GameObject ItemImage;

    [SerializeField] GameObject dialogueNext;

    [SerializeField] Text dialogueText;
    [SerializeField] Text descriptionText;
    [SerializeField] Text nameText;

    [SerializeField] Button selectBtn1;
    [SerializeField] Button selectBtn2;
    [SerializeField] Button selectBtn3;
    [SerializeField] Button selectBtn4;

    [SerializeField] Text selectText1;
    [SerializeField] Text selectText2;
    [SerializeField] Text selectText3;
    [SerializeField] Text selectText4;

    Dialogue[] dialogues;
    Select[] selects;

    InteractionEvent interactionEvent;
    NPC npc; //= currentNPCZ
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

    public int selectedNum; // 선지 별 NPC 상호작용

    public void SetNPC(NPC _npc)
    {
        npc = _npc; //NPC 인스턴스 설정

        interactionEvent = npc.interactionEvent; //npc의 interactionEvent 가져오기

        if(npc is StageNPC)
        {
            stageNpc = npc as StageNPC;
            statue = null;
        }
        else if(npc is Statue)
        {
            statue = npc as Statue;
            stageNpc = null;
        }
        else if(npc is NPC)
        {
            stageNpc = null;
            statue = null;
        }
        else
        {
            npc = null;
            stageNpc= null;
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
        foreach (var image in Images)
        {
            image.gameObject.SetActive(false);
        }
        
        dialoguePanel.SetActive(false);
        descriptionPanel.SetActive(false);
        namePanel.SetActive(false);

        selectBtn1.gameObject.SetActive(false);
        selectBtn2.gameObject.SetActive(false);
        selectBtn3.gameObject.SetActive(false);
        selectBtn4.gameObject.SetActive(false);

        if(ItemImage != null)
        {
            ItemImage.SetActive(false);
        }

        playerMove = FindObjectOfType<PlayerMove>(); //플레이어 FSM과 연결, 추가 코드
        statueScore = FindObjectOfType<StatueScore>();
        stageNpc = FindObjectOfType<StageNPC>();
        statue = FindObjectOfType<Statue>();
    }

    private void Update()
    {
        if(isNext)
        {
            dialogueNext.SetActive(true);
        }
        else
        {
            dialogueNext.SetActive(false);
        }

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
                else            // 기본(CSV파일 사용 대화)
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

    public void ShowDialogue(Dialogue[] _dialogues, string explainNum = null)
    {
        isDialogue = true;
        dialogueText.text = "";
        descriptionText.text = "";
        nameText.text = "";
        dialogues = _dialogues;
        playerMove.pState = PlayerMove.PlayerState.Interaction;

        if (!string.IsNullOrEmpty(explainNum)) //explainNum 있으면
        {
            // imageImage를 보관 중인 자료구조에 explainNum 변수를 인덱스로 사용해 이미지 할당 후 활성화.
            // imageImage.SetActive(true);
            if (npc.gameObject.CompareTag("Artwork") && int.TryParse(explainNum, out int explainIndex)) // !npc.isStatue 조건 삭제
            {
                if (explainIndex >= 0 && explainIndex < Images.Count)
                {
                    Images[explainIndex-1].SetActive(true);
                    isPopup = true;
                }
            }
            
            isExplain = true;

            int explainLine;
            if (int.TryParse(explainNum, out explainLine))
            {
                if (explainLine > 0 && explainLine <= dialogues.Length)
                {
                    lineCount = explainLine - 1; //explainLine번째 line으로 이동; 근데 왜 -1인진 모르겠음... 그냥 잘 돌아감
                    contextCount = 0;
                }
                else //예외처리
                {
                    Debug.LogError("Invalid explainNum. Starting from the first dialogue.");
                    lineCount = 0; // explainNum이 잘못된 경우 첫 번째 대화로 시작
                }
            }
            else //예외처리
            {
                Debug.LogError("Failed to parse explainNum. Starting from the first dialogue.");
                lineCount = 0; // explainNum 파싱 실패 시 첫 번째 대화로 시작
            }


        }
        else //explainNum 없으면 그냥 처음부터
        {
            lineCount = 0;
        }

        StartCoroutine(DialogueWriter()); //대화 시작
    }

    public void ShowSelect(Select[] _selects)
    {
        isSelect = true;

        if (_selects == null || _selects.Length == 0)
        {
            Debug.LogError("ShowSelect received a null or empty _selects array.");
            return;
        }

        selectText1.text = "";
        selectText2.text = "";
        selectText3.text = "";
        selectText4.text = "";
        selects = _selects;

        StartCoroutine(SelectWriter());
    }

    public void ShowMessage(string _message, string _name = null)
    {
        isDialogue = true;
        isMessage = true;
        playerMove.ActiveInteract = true;

        dialogueText.text = "";
        descriptionText.text = "";
        message = _message;
        string name = _name;

        if (name != null) //대사에 name 있으면
        {
            dialoguePanel.SetActive(true);
            descriptionPanel.SetActive(false);
            namePanel.SetActive(true);
            nameText.text = name;
        }
        else //name 없으면
        {
            descriptionPanel.SetActive(true);
            dialoguePanel.SetActive(false);
            namePanel.SetActive(false);
        }

        StartCoroutine(MessageWriter());
    }

    public void OnSelectButtonClicked(int selectedIndex, int currentIndex) // 판별 매개변수 추가(currentIndex)
    {
        if(npc is Statue selectedStatue)
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

        if(npc is StageNPC selectedNPC) // 선지 별 NPC 상호작용
        {
            if (currentIndex == 0) selectedNum = 1;
            else if (currentIndex == 1) selectedNum = 2;
            else if (currentIndex == 2) selectedNum = 3;
            else if (currentIndex == 3) selectedNum = 4;
            //Debug.Log("currentIndex, selectedNum: " + currentIndex + ", " + selectedNum);
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

        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        descriptionPanel.SetActive(false);
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

        if (SceneManager.GetActiveScene().name.StartsWith("Museum")) // 미술관 NPC
        {
            if (npc is StageNPC selectedNPC)
            {
                if (selectedNPC.dialogueFileName == "Tutorial2_dialogue") // 미술관장
                {
                    selectedNPC.TutorialFin();
                }
                //else if (selectedNPC.dialogueFileName == "Museum-Guard1_dialogue") // 경비원 StageNPC로 이동
                //{
                //    InventoryUI.instance.GetAnItem(10402);
                //    selectedNPC.questEnd = true;
                //}
            }
        }

        if(npc is Statue selectedStatue)
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
        
        if(ItemImage !=  null)
        {
            // 추가 - 플레이어 움직임 제한 해제
            playerMove.IsMoved = true;
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

        // 결과 UI 출력 - 1. 미술관 대화 파일
        if (npc.dialogueFileName == "Check3_dialogue")
        {
            Invoke("SetUIStateEnd", 1.5f);
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
    IEnumerator DialogueWriter()
    {
        //Debug.Log("DialogueWriter");
        if (dialogues[lineCount].name != "") //대사에 name 있으면
        {
            dialoguePanel.SetActive(true);
            descriptionPanel.SetActive(false);
            namePanel.SetActive(true);
        }
        else //name 없으면
        {
            descriptionPanel.SetActive(true);
            dialoguePanel.SetActive(false);
            namePanel.SetActive(false);
        }

        string replaceText = dialogues[lineCount].contexts[contextCount];
        replaceText = replaceText.Replace("#", ","); //#을 ,로 변환
        replaceText = replaceText.Replace("@", "\n"); //*을 \n으로 변환

        nameText.text = dialogues[lineCount].name; //name 출력

        // 초상화 출력
        foreach (var portrait in Portraits)
        {
            portrait.SetActive(false);
        }

        for (int i = 0; i < Portraits.Count; i++)
        {
            if (Portraits[i].name == nameText.text)
            {
                Portraits[i].SetActive(true);
                break;
            }
        }

        //context 출력
        for (int i = 0; i < replaceText.Length; i++)
        {
            dialogueText.text += replaceText[i];
            descriptionText.text += replaceText[i];
            yield return new WaitForSeconds(0.03f);
        }

        isNext = true;
    }
    #endregion

    IEnumerator SelectWriter()
    {
        Button[] buttons = { selectBtn1, selectBtn2, selectBtn3, selectBtn4 };
        Text[] texts = { selectText1, selectText2, selectText3, selectText4 };

        if (dialogues[lineCount].name == "")
        {
            for (int i = 0; i < selects.Length; i++)
            {
                if (selects[i].contexts.Length > 1) //선지가 2개 이상 존재하면
                {
                    for (int j = 0; j < selects[i].contexts.Length; j++)
                    {
                        if (i < buttons.Length) // 배열 범위 내인지 확인
                        {
                            buttons[j].gameObject.SetActive(true);
                            texts[j].gameObject.SetActive(true);

                            string replaceText = selects[i].contexts[j].Replace("#", ",");

                            for (int k = 0; k < replaceText.Length; k++)
                            {
                                texts[j].text += replaceText[k];
                                yield return new WaitForSeconds(0.03f);
                            }

                            string selectedMoveNum = selects[i].moveNum[j];
                            int selectedMoveNumInt;
                            int.TryParse(selectedMoveNum, out selectedMoveNumInt);

                            int currentSelectNum = j;// 판별 추가 코드

                            buttons[j].onClick.RemoveAllListeners();
                            buttons[j].onClick.AddListener(() => OnSelectButtonClicked(selectedMoveNumInt, currentSelectNum)); // 판별 매개변수 추가
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < selects.Length; i++)
            {
                if (selects[i].contexts.Length > 1) //선지가 2개 이상 존재하면
                {
                    for (int j = 0; j < selects[i].contexts.Length; j++)
                    {
                        if (i < buttons.Length) // 배열 범위 내인지 확인
                        {
                            buttons[j + 2].gameObject.SetActive(true);
                            texts[j + 2].gameObject.SetActive(true);

                            string replaceText = selects[i].contexts[j].Replace("#", ",");

                            for (int k = 0; k < replaceText.Length; k++)
                            {
                                texts[j + 2].text += replaceText[k];
                                yield return new WaitForSeconds(0.03f);
                            }

                            string selectedMoveNum = selects[i].moveNum[j];
                            int selectedMoveNumInt;
                            int.TryParse(selectedMoveNum, out selectedMoveNumInt);

                            int currentSelectNum = j;// 판별 추가 코드

                            buttons[j + 2].onClick.RemoveAllListeners();
                            buttons[j + 2].onClick.AddListener(() => OnSelectButtonClicked(selectedMoveNumInt, currentSelectNum)); // 판별 매개변수 추가
                        }
                    }
                }
            }
        }

    }

    IEnumerator MessageWriter()
    {
        string replaceText = message;

        //context 출력
        for (int i = 0; i < replaceText.Length; i++)
        {
            if(namePanel.activeSelf)
            {
                dialogueText.text += replaceText[i];
                yield return new WaitForSeconds(0.03f);
            }
            else
            {
                descriptionText.text += replaceText[i];
                yield return new WaitForSeconds(0.03f);
            }
        }
        isNext = true;
    }

    public void ItemPopup()
    {
        // 추가 - 플레이어 움직임 제한
        playerMove.IsMoved = false;
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

    public int LineCount() // 대사에 맞춰서 NPC 상태 변경
    {
        int _lineCount = lineCount;
        return _lineCount;
    }
}