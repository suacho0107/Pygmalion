using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI_Jiyun : MonoBehaviour
{
    DialogueManager_Jiyun dialogueManager;
    InteractionEvent interactionEvent;
    NPC npc; //= currentNPCZ
    StageNPC stageNpc;
    Statue statue;
    PlayerMove playerMove; //플레이어 FSM과 연결, 추가 코드
    StatueScore statueScore;
    MuseumLobbyCSV csv;

    Dialogue[] dialogues;
    Select[] selects;

    public List<GameObject> Images;
    public List<GameObject> Portraits;

    public GameObject dialoguePanel;
    public GameObject descriptionPanel;
    public GameObject namePanel;
    public GameObject ItemImage;

    public Text dialogueText;
    public Text descriptionText;
    public Text nameText;

    private List<GameObject> selectButtonList = new List<GameObject>();
    public GameObject selectButtons;
    public Button selectBtn1;
    public Button selectBtn2;
    public Button selectBtn3;
    public Button selectBtn4;

    private List<GameObject> selectTextList = new List<GameObject>();
    public Text selectText1;
    public Text selectText2;
    public Text selectText3;
    public Text selectText4;

    public int lineCount = 0; //대화 카운트
    public int contextCount = 0; //대사 카운트


    private void Awake()
    {
        
        dialogueManager = FindObjectOfType<DialogueManager_Jiyun>();
        npc = FindObjectOfType<NPC>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowDialogue(Dialogue[] _dialogues, string explainNum = null)
    {
        dialogueManager.isDialogue = true;
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
                    Images[explainIndex - 1].SetActive(true);
                    dialogueManager.isPopup = true;
                }
            }

            dialogueManager.isExplain = true;

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
        dialogueManager.isSelect = true;

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

    public void EndMessage()
    {
        dialogueManager.isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
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
        dialogues = null;
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
        dialogueManager.isSelect = false;
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
    public IEnumerator DialogueWriter()
    {
        //Debug.Log("DialogueWriter");
        if (dialogues[lineCount].name != "") //대사에 name 있으면
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(true);
        }
        else //name 없으면
        {
            descriptionPanel.SetActive(true);
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

        dialogueManager.isNext = true;
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
