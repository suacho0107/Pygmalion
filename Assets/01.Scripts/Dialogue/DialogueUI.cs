using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour //합병 후 DialogueUI_Jiyun -> DialogueUI로 변경
{
    #region References
    [Header("Managers &  References")]
    DialogueManager dialogueManager; //합병 후 DialogueManager_Jiyun -> DialogueManager로 변경
    InteractionEvent interactionEvent;
    PlayerMove playerMove; //플레이어 FSM과 연결, 추가 코드
    NPC npc; //= currentNPCZ
    StageNPC stageNpc;
    Statue statue;
    StatueScore statueScore;
    MuseumLobbyCSV csv;
    #endregion

    #region variables
    [Header("Images & Portraits")]
    public List<GameObject> Images;
    public List<GameObject> Portraits;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public GameObject namePanel;

    public GameObject ItemImage;

    public Text dialogueText;
    public Text descriptionText;
    private Text nameText;

    public GameObject dialogueNext;

    [Header("Select UI")]
    public GameObject selectButtons;
    private List<GameObject> selectButtonList = new List<GameObject>();
    private List<Text> selectTextList = new List<Text>();

    [Header("Select Control")]
    private int currentSelectIndex = 0;

    public int currentSelectButtonIndex = 0;
    private List<int> moveNumList = new List<int>();

    public bool isSelecting = false;

    public int buttonIndexNPC = 0; // NPC 상호작용 시 버튼 인덱스 전달

    [Header("DialogueFlow Management")]
    public int lineCount = 0; //대화 카운트
    public int contextCount = 0; //대사 카운트

    [Header("DialoguePanel Sprites")]
    public Sprite DialoguePanel;
    public Sprite DescriptionPanel;

    [Header("Button Sprites")]
    public Sprite ButtonDefault;
    public Sprite ButtonHighlighted;

    [Header("Current Portrait")]
    private GameObject currentPortrait;
    #endregion

    #region Unity Methods
    void Awake()
    {
        dialogueManager = GetComponent<DialogueManager>(); //합병 후 DialogueManager_Jiyun -> DialogueManager로 변경
        if (dialogueManager != null) //DialogueManager 예외처리
        {
            //Debug.Log($"DialogueManager 할당: {dialogueManager.gameObject.name}"); //합병 후 DialogueManager_Jiyun -> DialogueManager로 변경
        }
        //stageNpc = FindObjectOfType<StageNPC>();
        //statue = FindObjectOfType<Statue>();
        statueScore = FindObjectOfType<StatueScore>();
        nameText = namePanel.GetComponentInChildren<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerMove = FindObjectOfType<PlayerMove>(); //플레이어 FSM과 연결, 추가 코드
    }

    // Update is called once per frame
    void Update()
    {
        if (isSelecting)
        {
            SelectButtonInputHandler();
        }

        if (dialogueManager.isNext)
        {
            dialogueNext.SetActive(true);
        }
        else
        {
            dialogueNext.SetActive(false);
        }
    }
    #endregion

    public void SetNPC(NPC _npc)
    {
        npc = _npc;
    }

    #region Select Navigation
    private void SelectButtonInputHandler()
    {
        //상하 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (currentSelectButtonIndex == 2 || currentSelectButtonIndex == 3) //하단에서
            {
                if (isThereButton(currentSelectButtonIndex, -2)) //상단 확인
                {
                    currentSelectButtonIndex -= 2;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (currentSelectButtonIndex == 0 || currentSelectButtonIndex == 1) //상단에서
            {
                if (isThereButton(currentSelectButtonIndex, 2)) //하단 확인
                {
                    currentSelectButtonIndex += 2;
                }
            }
        }

        //좌우 이동
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (currentSelectButtonIndex == 1 || currentSelectButtonIndex == 3) //우측에서
            {
                if (isThereButton(currentSelectButtonIndex, -1)) //좌측 확인
                {
                    currentSelectButtonIndex -= 1;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (currentSelectButtonIndex == 0 || currentSelectButtonIndex == 2) //좌측에서
            {
                if (isThereButton(currentSelectButtonIndex, 1)) //우측 확인
                {
                    currentSelectButtonIndex += 1;
                }
            }
        }

        //선택
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("스페이스바");
            if (currentSelectButtonIndex < selectButtonList.Count)
            {
                // 🔹 UI 인덱스 → 실제 선택지 인덱스로 변환
                // portrait(이름)가 있으면 선택지가 UI에서 2칸 뒤부터 시작한다고 가정
                //int selectStartIndex = dialogueManager.dialogues[lineCount].name == "" ? 0 : 2;
                //int actualSelectIndex = currentSelectButtonIndex - selectStartIndex;

                //Debug.Log($"선택된 버튼 인덱스: {currentSelectButtonIndex}"); //delete

                string moveNumString = dialogueManager.selects[currentSelectIndex].moveNum[currentSelectButtonIndex];

                if (int.TryParse(moveNumString, out int selectedIndex))
                {
                    //Debug.Log("currentselecbuttonindex: " + currentSelectButtonIndex);
                    buttonIndexNPC = currentSelectButtonIndex;
                    //Debug.Log("인덱스 전달");
                    OnSelectButtonSelected(selectedIndex, currentSelectButtonIndex);
                }
                else if (string.IsNullOrWhiteSpace(moveNumString))
                {
                    EndSelect();
                    EndDialogue();
                }
                else
                {
                    //Debug.LogError("moveNum Parsing 실패");
                    Debug.LogError($"moveNum Parsing 실패: '{moveNumString}' (Index: {currentSelectButtonIndex})");
                }

                //초기화
                isSelecting = false;
                currentSelectButtonIndex = 0;
            }
        }
        HighlightSelectButton();
    }

    private bool isThereButton(int buttonIndex, int increraseButtonIndex)
    {
        int targetButtonIndex = buttonIndex + increraseButtonIndex;

        return targetButtonIndex >= 0 &&
           targetButtonIndex < selectButtonList.Count &&
           selectButtonList[targetButtonIndex].gameObject.activeSelf;
    }

    private void HighlightSelectButton()
    {
        for (int i = 0; i < selectButtonList.Count; i++)
        {
            Image image = selectButtonList[i].GetComponent<Image>();
            image.sprite = (i == currentSelectButtonIndex) ? ButtonHighlighted : ButtonDefault;
        }
    }

    public void OnSelectButtonSelected(int selectedIndex, int _currentSelectButtonIndex) //판별 매개변수 추가(currentIndex)
    {
        if(npc is StageNPC selectedStageNPC)
        {
            
        }

        if (npc is Statue selectedStatue)
        {
            if (!selectedStatue.isChecked) //첫 번째 상호작용(조사): 선지 2개 출력
            {
                if (_currentSelectButtonIndex == 0)
                {
                    statueScore.checkedCount += 1;
                    statueScore.SaveScore();

                    selectedStatue.isChecked = true;
                    selectedStatue.SaveStatueData();
                    Debug.Log("isChecked");
                }
            }
            else //두 번째 상호작용(판별): 선지 4개 출력
            {
                if (_currentSelectButtonIndex == 0)
                {
                    statueScore.checkedCount += 1;
                    statueScore.SaveScore();

                    selectedStatue.isChecked = true;
                    selectedStatue.SaveStatueData();
                }
                else if (_currentSelectButtonIndex == 1)
                {
                    //Debug.Log("_currentSelectButtonIndex == 1");
                    if (selectedStatue.isEnemy)
                    {//건드린다 --> 정답
                        selectedStatue.isJudged = true;
                        selectedStatue.isCorrect = true;
                        selectedStatue.currentIndex = 3;
                        selectedStatue.explainNum = "1";
                    }
                    else
                    {//건드린다 --> 오답
                        Debug.Log("건드린다 오답");
                        selectedStatue.isJudged = true;
                        selectedStatue.isCorrect = false;
                    }
                }
                else if (_currentSelectButtonIndex == 2)
                {
                    //Debug.Log("_currentSelectButtonIndex == 2");
                    if (selectedStatue.isEnemy)
                    {//이상 없음 --> 오답
                        selectedStatue.isJudged = true;
                        selectedStatue.isCorrect = false;
                        selectedStatue.explainNum = "2";
                    }
                    else
                    {//이상 없음 --> 정답
                        Debug.Log("이상없음 정답");
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
        else if (selectedIndex == 0) //선지 선택 직후 대화 종료
        {
            EndSelect();
            EndDialogue();
        }
        else
        {
            Debug.LogError("Selected dialogue index is out of bounds. Ending dialogue.");
            EndSelect();
            EndDialogue();
        }
    }
    #endregion

    #region Dialogue
    public void ShowDialogue(Dialogue[] _dialogues, string explainNum = null)
    {
        //초기화 & Setting
        dialogueManager.dialogues = _dialogues;
        dialogueManager.isDialogue = true;
        if(playerMove != null)
        {
            playerMove.pState = PlayerMove.PlayerState.Interaction;
        }
        dialoguePanel.SetActive(true);
        dialogueText.text = "";
        descriptionText.text = "";
        nameText.text = "";

        //explainNum 있으면
        if (!string.IsNullOrEmpty(explainNum))
        {
            dialogueManager.isExplain = true;

            #region Image Popup
            if (npc.gameObject.CompareTag("Artwork") && int.TryParse(explainNum, out int explainIndex))
            {
                if (explainIndex >= 0 && explainIndex < Images.Count)
                {
                    Debug.Log($"explainIndex : {explainIndex}");
                    Images[explainIndex - 1].SetActive(true);
                    dialogueManager.isPopup = true;
                }
            }
            #endregion

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
                    lineCount = 0; //explainNum이 잘못된 경우 첫 번째 대화로 시작
                }
            }
            else //예외처리
            {
                Debug.LogError("Failed to parse explainNum. Starting from the first dialogue.");
                lineCount = 0; //explainNum 파싱 실패 시 첫 번째 대화로 시작
            }


        }
        else //explainNum 없으면 그냥 처음부터
        {
            lineCount = 0;
        }
        StartCoroutine(DialogueWriter()); //대화 시작
    }

    public void EndDialogue()
    {
        //초기화
        dialogueManager.dialogues = null;
        dialogueManager.isDialogue = false;
        dialogueManager.isExplain = false;
        dialogueManager.isNext = false;
        dialogueManager.isEnd = true;
        if(playerMove != null)
        {
            playerMove.ActiveInteract = false;
        }
        lineCount = 0;
        contextCount = 0;
        //npc.isInteract = true; //미술관장

        if (npc is StageNPC selectedNPC)
        {
            selectedNPC.isInteract = true;
            //미술관장
            if (selectedNPC.dialogueFileName == "Tutorial2_dialogue")
            {
                selectedNPC.TutorialFin();
            }
            else if (selectedNPC.dialogueFileName == "Museum-Guard1_dialogue")
            {
                InventoryUI.instance.GetQuestItem(10402);
                selectedNPC.questEnd = true;
            }
            else if (selectedNPC.dialogueFileName == "Check3_dialogue")
            {
                //Invoke("Set_UIStateEnd", 1.5f);
                UIManager.u_instance.Set_UIState(Define.UI.UIState.End);
            }
        }

        if (npc is Statue selectedStatue)
        {
            selectedStatue.isInteract = true;
            selectedStatue.CheckResult();
        }

        dialogueManager.SaveData();

        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);

        if (ItemImage != null)
        {
            ItemImage.SetActive(false);
        }

        #region Image Popup
        foreach (var image in Images)
        {
            image.SetActive(false);
        }
        #endregion

        foreach (var portrait in Portraits)
        {
            portrait.SetActive(false);
        }
    }
    #endregion

    #region Message
    public void ShowMessage(string _message, string _name = null)
    {
        //초기화 & Setting
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

    public void EndMessage()
    {
        //초기화
        dialogueManager.dialogues = null;
        dialogueManager.isDialogue = false;
        dialogueManager.isExplain = false;
        dialogueManager.isNext = false;
        dialogueManager.isMessage = false;
        playerMove.ActiveInteract = false;

        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);

        lineCount = 0;
        contextCount = 0;
    }
    #endregion

    #region Select UI
    public void SetSelectButtons()
    {
        //Debug.Log("SetSelectButtons() 실행"); //delete

        //중복추가 방지
        selectButtonList.Clear();
        selectTextList.Clear();

        for (int i = 0; i < selectButtons.transform.childCount; i++)
        {
            GameObject selectButton = selectButtons.transform.GetChild(i).gameObject;
            Text selectText = selectButton.transform.GetChild(0).GetComponent<Text>();

            selectButtonList.Add(selectButton);
            selectTextList.Add(selectText);

            selectButton.SetActive(false);

            //Debug.Log($"selectButtonList[{i}] = {selectButton.name}"); //delete
        }

        currentSelectButtonIndex = 0;
        //HighlightDialogueButton();
    }
    public void ShowSelect(Select _select)
    {
        dialogueManager.isSelect = true;

        if (_select == null)
        {
            Debug.LogError("ShowSelect received a null _select.");
            return;
        }

        lineCount = 0;

        // 선택지 UI 초기화
        for (int i = 0; i < selectTextList.Count; i++)
        {
            selectTextList[i].text = "";
        }

        // dialogueManager가 선택지 하나만 가지도록 수정
        dialogueManager.selects = new Select[] { _select };

        // 선택지 출력
        StartCoroutine(SelectWriter());
    }

    //public void ShowSelect(Select[] _selects)
    //{
    //    dialogueManager.isSelect = true;

    //    if (_selects == null || _selects.Length == 0)
    //    {
    //        Debug.LogError("ShowSelect received a null or empty _selects array.");
    //        return;
    //    }

    //    lineCount = 0;

    //    Debug.Log($"ShowSelect 호출됨: lineCount = {lineCount}, _selects.Length = {_selects.Length}");


    //    for (int i = 0; i < selectTextList.Count; i++)
    //    {
    //        selectTextList[i].text = "";
    //    }

    //    dialogueManager.selects = _selects;

    //    StartCoroutine(SelectWriter());
    //}

    void EndSelect()
    {
        //초기화
        dialogueManager.selects = null;
        dialogueManager.isSelect = false;

        selectButtons.SetActive(false);

        //SaveData();
    }
    #endregion

    #region Item Popup
    public void ItemPopup()
    {
        ItemImage.SetActive(true);
    }
    #endregion

    #region NPC State Management
    public int LineCount() // 대사에 맞춰서 NPC 상태 변경
    {
        int _lineCount = lineCount;
        return _lineCount;
    }
    #endregion

    #region Coroutines
    public IEnumerator DialogueWriter()
    {
        Text contextText;
        Image dialoguePanelImage = dialoguePanel.GetComponent<Image>();

        if (dialogueManager.dialogues[lineCount].name != "") //대사에 name 있으면
        {
            dialoguePanelImage.sprite = DialoguePanel;
            dialoguePanel.SetActive(true);
            namePanel.SetActive(true);
            contextText = dialogueText;
        }
        else //name 없으면
        {
            dialoguePanelImage.sprite = DescriptionPanel;
            dialoguePanel.SetActive(true);
            namePanel.SetActive(false);
            contextText = descriptionText;
        }

        string replaceText = dialogueManager.dialogues[lineCount].contexts[contextCount];
        replaceText = replaceText.Replace("#", ","); //#을 ,로 변환
        replaceText = replaceText.Replace("@", "\n"); //*을 \n으로 변환

        nameText.text = dialogueManager.dialogues[lineCount].name; //name 출력

        // 초상화 출력
        if(Portraits != null)
        {
            foreach (var portrait in Portraits)
            {
                portrait.SetActive(false);
            }
        }

        for (int i = 0; i < Portraits.Count; i++)
        {
            if (Portraits[i].name == nameText.text)
            {
                currentPortrait = Portraits[i];
                currentPortrait.SetActive(true);
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

    IEnumerator SelectWriter()
    {
        //Guard
        if (dialogueManager.selects == null || dialogueManager.selects.Length == 0)
        {
            Debug.LogError("dialogueManager.selects is Null or Empty.");
            yield break;
        }

        //Debug.Log($"SelectWriter ����: lineCount = {lineCount}, selects.Length = {dialogueManager.selects.Length}");   

        //UI
        Image dialoguePanelImage = dialoguePanel.GetComponent<Image>();
        dialoguePanelImage.sprite = DescriptionPanel;
        namePanel.SetActive(false);
        if (currentPortrait != null) //Guard
        {
            currentPortrait.SetActive(false);
        }

        moveNumList.Clear(); //중복 방지

        if (lineCount < dialogueManager.selects.Length)
        {
            currentSelectIndex = lineCount;

            Select select = dialogueManager.selects[lineCount];
            yield return StartCoroutine(WriteSelectOptions(select));
            isSelecting = true;
            //HighlightSelectButton();
        }
        else
        {
            Debug.LogError("Select data is out of range.");
        }
    }

    IEnumerator WriteSelectOptions(Select select)
    {
        //Debug.Log($"WriteSelectOptions(Select {select}");

        SetSelectButtons();

        selectButtons.SetActive(true);
        HighlightSelectButton();

        //Debug.Log($"select.contexts.Length = {select.contexts.Length}");

        for (int i = 0; i < select.contexts.Length && i < 4; i++) // 최대 4개까지
        {
            if (i < selectButtonList.Count)
            {

                selectButtonList[i].gameObject.SetActive(true);
                selectTextList[i].text = "";

                string replacedText = select.contexts[i].Replace("#", ",");

                for (int j = 0; j < replacedText.Length; j++)
                {
                    selectTextList[i].text += replacedText[j];
                    yield return new WaitForSeconds(0.03f);
                }

                //moveNum 저장
                if (i < select.moveNum.Length && int.TryParse(select.moveNum[i], out int moveNum))
                {
                    moveNumList.Add(moveNum);
                }
                else
                {
                    moveNumList.Add(0); //기본값
                }
            }
        }
        isSelecting = true;
    }
    #endregion
}
