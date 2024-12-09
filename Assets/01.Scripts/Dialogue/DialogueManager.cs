using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] List<GameObject> Images;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] GameObject namePanel;

    [SerializeField] Text dialogueText;
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
    public PlayerMove playerMove; //플레이어 FSM과 연결, 추가 코드
    StatueScore statueScore;
    MuseumLobbyCSV csv;

    bool isDialogue = false;
    bool isNext = false; //특정 키 입력 대기
    bool isSelect = false;
    bool isExplain = false; //설명대사인지 구분
    bool isPopup = false;

    int lineCount = 0; //대화 카운트
    int contextCount = 0; //대사 카운트

    public void SetNPC(NPC _npc)
    {
        npc = _npc; //NPC 인스턴스 설정

        interactionEvent = npc.interactionEvent; //npc의 interactionEvent 가져오기

        if (npc != null)
        {
            interactionEvent = npc.interactionEvent; //npc의 interactionEvent 가져오기
        }
        else
        {
            Debug.LogError("SetNPC: NPC is null.");
        }
    }
    
    private void Start()
    {
        foreach (var image in Images)
        {
            image.gameObject.SetActive(false);
        }
        
        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);

        selectBtn1.gameObject.SetActive(false);
        selectBtn2.gameObject.SetActive(false);
        selectBtn3.gameObject.SetActive(false);
        selectBtn4.gameObject.SetActive(false);

        playerMove = FindObjectOfType<PlayerMove>(); //플레이어 FSM과 연결, 추가 코드
        statueScore = FindObjectOfType<StatueScore>();

    }

    private void Update()
    {
        if (isDialogue && isNext && !isSelect)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isNext = false;
                dialogueText.text = "";

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

    public void ShowDialogue(Dialogue[] _dialogues, string explainNum = null)
    {
        isDialogue = true;
        dialogueText.text = "";
        nameText.text = "";
        dialogues = _dialogues;

        if (!string.IsNullOrEmpty(explainNum)) //explainNum 있으면
        {
            // imageImage를 보관 중인 자료구조에 explainNum 변수를 인덱스로 사용해 이미지 할당 후 활성화.
            // imageImage.SetActive(true);
            if (!npc.isStatue && npc.gameObject.CompareTag("Artwork") && int.TryParse(explainNum, out int explainIndex))
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

    public void OnSelectButtonClicked(int selectedIndex, int currentIndex) // 판별 매개변수 추가(currentIndex)
    {
        //Debug.Log("누른 버튼:" + currentIndex);
        if (npc.isStatue)
        {
            if (!npc.isChecked && currentIndex == 0) // 첫 번째 상호작용(조사): 선지 2개 출력
            {
                npc.isChecked = true;
                npc.SaveNPCData();
                Debug.Log("statue.isChecked == True");
            }
            else if (!npc.isChecked && currentIndex == 1)
            {
                Debug.Log("1) 그대로 둔다");
            }
            else if (npc.isChecked && currentIndex == 0 ) // 두 번째 상호작용(판별): 선지 4개 출력
            {
                Debug.Log("2) 다시 살펴본다");
                statueScore.checkedCount += 1;
                statueScore.SaveScore();
            }
            else if (npc.isChecked && currentIndex == 1 )
            {
                statueScore.checkedCount += 1;
                statueScore.SaveScore();

                if (npc.isEnemy)
                {// 건드린다 --> 정답
                    npc.isJudged = true;
                    npc.isCorrect = true;
                    npc.currentIndex = 3;
                    npc.explainNum = "1";
                    //statueScore.fightCount += 1;
                    //statueScore.SaveScore();
                    //npc.SaveNPCData();
                }
                else
                {// 건드린다 --> 오답
                    npc.isJudged = true;
                    npc.isCorrect = false;
                    //npc.SaveNPCData();
                }
            }
            else if (npc.isChecked && currentIndex == 2)
            {
                statueScore.checkedCount += 1;
                statueScore.SaveScore();

                if (npc.isEnemy)
                {// 이상 없음 --> 오답
                    npc.isJudged = true;
                    npc.isCorrect = false;
                    npc.explainNum = "2";
                    //statueScore.fightCount += 1;
                    //statueScore.SaveScore();      //NPC 스크립트에서 변경
                    //npc.SaveNPCData();
                }
                else
                {// 이상 없음 --> 정답
                    npc.isJudged = true;
                    npc.isCorrect = true;
                    npc.explainNum = "3";
                    //npc.SaveNPCData();
                }
            }
            else if (npc.isChecked && currentIndex == 3)
            {// 그대로 둔다
                Debug.Log("2) 그대로 둔다");
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
        Debug.Log("endDialogue1");

        if(npc.dialogueFileName == "Tutorial2_dialogue")
        {
            npc.isTutoFin = true;
            Debug.Log("endDialogue2");
        }

        if(npc.isStatue && npc.isChecked && npc.isJudged)
        {
            if(npc.isEnemy)
            {
                npc.result = true;
            }
            else
            {
                if(npc.currentIndex == 3)
                {
                    if (npc.isCorrect == true)
                    {
                        npc.ChangeDialogueExplain(3, "3");
                        npc.result = true;
                    }
                    else
                    {
                        npc.ChangeDialogueExplain(4, "1");
                        Debug.Log("dialogueManager 여기, " + npc.dialogueFileName);
                        npc.result = true;
                    }
                }
                //if (npc.isCorrect)
                //{
                //    if (npc.currentIndex == 3)
                //    {
                //        npc.result = true;
                //        Debug.Log("result True 테스트 완료 시 삭제할 로그");
                //    }
                //}
                //else
                //{
                //    if (npc.currentIndex == 3)
                //    {
                //        npc.result = true;
                //        Debug.Log("result True 테스트 완료 시 삭제할 로그");
                //    }
                //}
            }
        }
        npc.SaveNPCData();

        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        playerMove.ActiveInteract = false; // 추가 코드

        // 결과 UI 출력
        if (npc.dialogueFileName == "Check3_dialogue")
        {
            Invoke("SetUIStateEnd", 1.5f);
        }

        // 모든 이미지 비활성화
        foreach (var image in Images)
        {
            image.SetActive(false);
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
        
        npc.SaveNPCData();
    }

    #region 팝업 이미지 구현
    IEnumerator DialogueWriter()
    {
        if (dialogues[lineCount].name != "") //대사에 name 있으면
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(true);
        }
        else //name 없으면
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(false);
        }

        string replaceText = dialogues[lineCount].contexts[contextCount];
        replaceText = replaceText.Replace("#", ","); //#을 ,로 변환
        replaceText = replaceText.Replace("@", "\n"); //*을 \n으로 변환

        nameText.text = dialogues[lineCount].name; //name 출력

        //context 출력
        for (int i = 0; i < replaceText.Length; i++)
        {
            dialogueText.text += replaceText[i];
            yield return new WaitForSeconds(0.03f);
        }

        isNext = true;
    }
    #endregion

    IEnumerator SelectWriter()
    {
        Button[] buttons = { selectBtn1, selectBtn2, selectBtn3, selectBtn4 };
        Text[] texts = { selectText1, selectText2, selectText3, selectText4 };

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
}
