using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI_Jiyun : MonoBehaviour
{
    [SerializeField] DialogueManager_Jiyun dialogueManager;
    InteractionEvent interactionEvent;
    PlayerMove playerMove; //플레이어 FSM과 연결, 추가 코드
    NPC npc; //= currentNPCZ
    StageNPC stageNpc;
    Statue statue;
    StatueScore statueScore;
    MuseumLobbyCSV csv;

    public List<GameObject> Images;
    public List<GameObject> Portraits;

    public GameObject dialoguePanel;
    //public GameObject descriptionPanel;
    public GameObject namePanel;
    public GameObject ItemImage;

    public Text dialogueText;
    public Text descriptionText;
    private Text nameText;

    public GameObject selectButtons;
    private List<GameObject> selectButtonList = new List<GameObject>();
    private List<Text> selectTextList = new List<Text>();

    private int currentSelectIndex = 0;

    private int currentSelectButtonIndex = 0;
    private List<int> moveNumList = new List<int>();

    public bool isSelecting = false;

    public int lineCount = 0; //대화 카운트
    public int contextCount = 0; //대사 카운트

    public Sprite ButtonDefault;
    public Sprite ButtonHighlighted;


    void Awake()
    {
        //dialogueManager = FindObjectOfType<DialogueManager_Jiyun>();
        dialogueManager = GetComponent<DialogueManager_Jiyun>();
        if (dialogueManager != null)
        {
            Debug.Log($"DialogueManager_Jiyun 할당: {dialogueManager.gameObject.name}");
        }
        playerMove = FindObjectOfType<PlayerMove>(); //플레이어 FSM과 연결, 추가 코드
        stageNpc = FindObjectOfType<StageNPC>();
        statue = FindObjectOfType<Statue>();
        statueScore = FindObjectOfType<StatueScore>();
        nameText = namePanel.GetComponentInChildren<Text>();
        Debug.Log($"nameText: {namePanel.GetComponentInChildren<Text>().gameObject.name}");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelecting)
        {
            SelectButtonInputHandler();
        }
    }

    private void SelectButtonInputHandler()
    {
        //상하 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentSelectButtonIndex == 2 || currentSelectButtonIndex == 3)
            {
                currentSelectButtonIndex -= 2;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentSelectButtonIndex == 0 || currentSelectButtonIndex == 1)
            {
                if (isThereButton(currentSelectButtonIndex, 2))
                {
                    currentSelectButtonIndex += 2;
                }
            }
        }

        //좌우 이동
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentSelectButtonIndex % 2 == 1)
            {
                currentSelectButtonIndex--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentSelectButtonIndex % 2 == 0)
            {
                currentSelectButtonIndex++;
            }
        }

        //선택
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentSelectButtonIndex < selectButtonList.Count) //버튼 범위 안에 있는지 확인
            {
                //OnSelectButtonClicked(selectedIndex, currentSelectButtonIndex);

                currentSelectButtonIndex = Mathf.Clamp(currentSelectButtonIndex, 0, selectButtonList.Count - 1);

                // currentSelectButtonIndex 는 UI 선택지 인덱스

                // 실제 선택지 이동 번호 가져오기
                string moveNumStr = dialogueManager.selects[currentSelectIndex].moveNum[currentSelectButtonIndex];

                if (int.TryParse(moveNumStr, out int selectedIndex))
                {
                    OnSelectButtonClicked(selectedIndex, currentSelectButtonIndex);
                }
                else
                {
                    Debug.LogError("moveNum 파싱 실패");
                }

                //초기화
                currentSelectButtonIndex = 0;
            }
        }

        Debug.Log($"선택된 버튼 인덱스: {currentSelectButtonIndex}"); //delete

        HighlightSelectButton();
    }

    private bool isThereButton(int buttonIndex, int increraseButtonIndex)
    {
        int targetButtonIndex = buttonIndex + increraseButtonIndex;

        return targetButtonIndex < 4;
    }

    private void HighlightSelectButton()
    {
        for (int i = 0; i < selectButtonList.Count; i++)
        {
            Image image = selectButtonList[i].GetComponent<Image>();
            image.sprite = (i == currentSelectButtonIndex) ? ButtonHighlighted : ButtonDefault;
        }
    }




    public void SetSelectButtons()
    {
        selectButtonList.Clear(); //중복추가 방지
        selectTextList.Clear(); //중복추가 방지

        for (int i = 0; i < selectButtons.transform.childCount; i++)
        {
            GameObject selectButton = selectButtons.transform.GetChild(i).gameObject;
            Text selectText = selectButton.transform.GetChild(0).GetComponent<Text>();

            selectButtonList.Add(selectButton);
            selectTextList.Add(selectText);
        }

        currentSelectButtonIndex = 0;
        //HighlightDialogueButton();
    }

    public void ShowDialogue(Dialogue[] _dialogues, string explainNum = null)
    {

        Debug.Log("ShowDialogue() 실행");

        //초기화 및 Setting
        dialogueManager.isDialogue = true;
        dialogueText.text = "";
        descriptionText.text = "";
        nameText.text = "";
        dialogueManager.dialogues = _dialogues;
        playerMove.pState = PlayerMove.PlayerState.Interaction;

        if (!string.IsNullOrEmpty(explainNum)) //explainNum 있으면
        {
            // imageImage를 보관 중인 자료구조에 explainNum 변수를 인덱스로 사용해 이미지 할당 후 활성화.
            // imageImage.SetActive(true);
            if (npc.gameObject.CompareTag("Artwork") && int.TryParse(explainNum, out int explainIndex)) // !npc.isStatue 조건 삭제
            {
                if (explainIndex >= 0 && explainIndex < Images.Count)
                {
                    Images[explainIndex - 1].SetActive(true);
                    dialogueManager.isPopup = true;
                }
            }

            dialogueManager.isExplain = true;

            int explainLine;
            if (int.TryParse(explainNum, out explainLine))
            {
                if (explainLine > 0 && explainLine <= dialogueManager.dialogues.Length)
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
        dialogueManager.isSelect = true;

        if (_selects == null || _selects.Length == 0)
        {
            Debug.LogError("ShowSelect received a null or empty _selects array.");
            return;
        }

        for (int i = 0; i < selectTextList.Count; i++)
        {
            selectTextList[i].text = "";
        }

        dialogueManager.selects = _selects;

        StartCoroutine(SelectWriter());
    }

    public void ShowMessage(string _message, string _name = null)
    {
        dialogueManager.isDialogue = true;
        dialogueManager.isMessage = true;
        playerMove.ActiveInteract = true;

        dialogueText.text = "";
        dialogueManager.message = _message;
        string name = _name;

        if (name != null) //대사에 name 있으면
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(true);
            nameText.text = name;
        }
        else //name 없으면
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(false);
        }

        StartCoroutine(MessageWriter());
    }

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

        if (targetLineCount >= 0 && targetLineCount < dialogueManager.dialogues.Length) //targetLineCount가 0 이상이고, dialogues 안에 있으면
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

    public void EndMessage()
    {
        dialogueManager.isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogueManager.dialogues = null;
        dialogueManager.isNext = false;
        dialogueManager.isExplain = false;
        dialogueManager.isMessage = false;

        playerMove.ActiveInteract = false;

        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        playerMove.ActiveInteract = false; // 추가 코드
    }

    public void EndDialogue()
    {
        //초기화
        dialogueManager.isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogueManager.dialogues = null;
        dialogueManager.isNext = false;
        dialogueManager.isExplain = false;
        npc.isInteract = true; // 미술관장
        dialogueManager.isEnd = true;

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

        dialogueManager.SaveData();

        dialoguePanel.SetActive(false);
        //descriptionPanel.SetActive(false);
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
        dialogueManager.isSelect = false;
        dialogueManager.selects = null;

        selectButtons.SetActive(false);
        //selectBtn1.gameObject.SetActive(false);
        //selectBtn2.gameObject.SetActive(false);
        //selectText1.gameObject.SetActive(false);
        //selectText2.gameObject.SetActive(false);

        //selectBtn3.gameObject.SetActive(false);
        //selectBtn4.gameObject.SetActive(false);
        //selectText3.gameObject.SetActive(false);
        //selectText4.gameObject.SetActive(false);
        //SaveData();
    }

    #region 팝업 이미지 구현
    public IEnumerator DialogueWriter()
    {
        Text contextText;
        

        //Debug.Log("DialogueWriter");
        if (dialogueManager.dialogues[lineCount].name != "") //대사에 name 있으면
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(true);
            contextText = dialogueText;
        }
        else //name 없으면
        {
            //descriptionPanel.SetActive(true);
            namePanel.SetActive(false);
            contextText = descriptionText;
        }

        string replaceText = dialogueManager.dialogues[lineCount].contexts[contextCount];
        replaceText = replaceText.Replace("#", ","); //#을 ,로 변환
        replaceText = replaceText.Replace("@", "\n"); //*을 \n으로 변환

        nameText.text = dialogueManager.dialogues[lineCount].name; //name 출력

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
            contextText.text += replaceText[i];
            yield return new WaitForSeconds(0.03f);
        }

        dialogueManager.isNext = true;
    }
    #endregion

    IEnumerator SelectWriter()
    {
        int offset = dialogueManager.dialogues[lineCount].name == "" ? 0 : 2; //name 존재 여부 판단해서 매개변수 전달

        moveNumList.Clear(); //중복 방지
        currentSelectButtonIndex = 0;

        if (lineCount < dialogueManager.selects.Length)
        {
            currentSelectIndex = lineCount;

            Select select = dialogueManager.selects[lineCount];
            yield return StartCoroutine(WriteSelectOptions(select, offset));
            isSelecting = true;
            HighlightSelectButton();
        }
        else
        {
            Debug.LogError("Select data is out of range.");
        }
    }

    IEnumerator WriteSelectOptions(Select select, int offset)
    {
        for (int i = 0; i < select.contexts.Length && i < 4; i++) // 최대 4개까지
        {
            int buttonIndex = i + offset;

            if (buttonIndex < selectButtonList.Count)
            {
                selectButtonList[buttonIndex].gameObject.SetActive(true);
                selectTextList[buttonIndex].text = "";

                string replacedText = select.contexts[i].Replace("#", ",");

                for (int j = 0; j < replacedText.Length; j++)
                {
                    selectTextList[buttonIndex].text += replacedText[j];
                    yield return new WaitForSeconds(0.03f);
                }

                // moveNum 저장
                if (i < select.moveNum.Length && int.TryParse(select.moveNum[i], out int moveNum))
                {
                    moveNumList.Add(moveNum);
                }
                else
                {
                    moveNumList.Add(0); // 기본값
                }
            }
        }
    }


    //IEnumerator SelectWriter()
    //{

    //    if (dialogueManager.dialogues[lineCount].name == "")
    //    {
    //        for (int i = 0; i < dialogueManager.selects.Length; i++)
    //        {
    //            if (dialogueManager.selects[i].contexts.Length > 1) //선지가 2개 이상 존재하면
    //            {
    //                for (int j = 0; j < dialogueManager.selects[i].contexts.Length; j++)
    //                {
    //                    if (j < selectButtonList.Count) // 배열 범위 내인지 확인
    //                    {
    //                        selectButtonList[j].gameObject.SetActive(true);
    //                        //selectTextList[j].gameObject.SetActive(true);

    //                        string replaceText = dialogueManager.selects[i].contexts[j].Replace("#", ",");

    //                        for (int k = 0; k < replaceText.Length; k++)
    //                        {
    //                            selectTextList[j].text += replaceText[k];
    //                            yield return new WaitForSeconds(0.03f);
    //                        }

    //                        string selectedMoveNum = dialogueManager.selects[i].moveNum[j];
    //                        int selectedMoveNumInt;
    //                        int.TryParse(selectedMoveNum, out selectedMoveNumInt);

    //                        int currentSelectNum = j;// 판별 추가 코드

    //                        selectButtonList[j].onClick.RemoveAllListeners();
    //                        selectButtonList[j].onClick.AddListener(() => OnSelectButtonClicked(selectedMoveNumInt, currentSelectNum)); // 판별 매개변수 추가
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        for (int i = 0; i < dialogueManager.selects.Length; i++)
    //        {
    //            if (dialogueManager.selects[i].contexts.Length > 1) //선지가 2개 이상 존재하면
    //            {
    //                for (int j = 0; j < dialogueManager.selects[i].contexts.Length; j++)
    //                {
    //                    if (i < selectButtonList.Count) // 배열 범위 내인지 확인
    //                    {
    //                        selectButtonList[j + 2].gameObject.SetActive(true);
    //                        //texts[j + 2].gameObject.SetActive(true);

    //                        string replaceText = dialogueManager.selects[i].contexts[j].Replace("#", ",");

    //                        for (int k = 0; k < replaceText.Length; k++)
    //                        {
    //                            selectTextList[j + 2].text += replaceText[k];
    //                            yield return new WaitForSeconds(0.03f);
    //                        }

    //                        string selectedMoveNum = dialogueManager.selects[i].moveNum[j];
    //                        int selectedMoveNumInt;
    //                        int.TryParse(selectedMoveNum, out selectedMoveNumInt);

    //                        int currentSelectNum = j;// 판별 추가 코드

    //                        selectButtonList[j + 2].onClick.RemoveAllListeners();
    //                        selectButtonList[j + 2].onClick.AddListener(() => OnSelectButtonClicked(selectedMoveNumInt, currentSelectNum)); // 판별 매개변수 추가
    //                    }
    //                }
    //            }
    //        }
    //    }

    //}

    IEnumerator MessageWriter()
    {
        string replaceText = dialogueManager.message;

        //context 출력
        for (int i = 0; i < replaceText.Length; i++)
        {
            dialogueText.text += replaceText[i];
            yield return new WaitForSeconds(0.03f);
        }
        dialogueManager.isNext = true;
    }

    public void ItemPopup()
    {
        ItemImage.SetActive(true);
    }

}
