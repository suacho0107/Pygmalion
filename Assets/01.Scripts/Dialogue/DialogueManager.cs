using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] GameObject namePanel;

    [SerializeField] Text dialogueText;
    [SerializeField] Text nameText;

    [SerializeField] Button selectBtn1;
    [SerializeField] Button selectBtn2;

    [SerializeField] Text selectText1;
    [SerializeField] Text selectText2;

    Dialogue[] dialogues;
    Select[] selects;

    InteractionEvent interactionEvent;
    NPC npc; //= currentNPC
    public PlayerMove playerMove; //플레이어 FSM과 연결, 추가 코드

    bool isDialogue = false;
    bool isNext = false; //특정 키 입력 대기
    bool isSelect = false;
    bool isExplain = false; //설명대사인지 구분

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
        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        selectBtn1.gameObject.SetActive(false);
        selectBtn2.gameObject.SetActive(false);
        playerMove = FindObjectOfType<PlayerMove>(); //플레이어 FSM과 연결, 추가 코드
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
        selects = _selects;

        StartCoroutine(SelectWriter());
    }

    public void OnSelectButtonClicked(int selectedIndex)
    {
        int targetLineCount = (int)selectedIndex - 1;

        if (targetLineCount >= 0 && targetLineCount < dialogues.Length) //targetLineCount가 0 이상이고, dialogues 안에 있으면
        {
            lineCount = targetLineCount; //lineCount 강제 변경
            contextCount = 0; //contextCount 초기화
            EndSelect(); //Select End하기
            StartCoroutine(DialogueWriter()); //변경한 lineCount, contextCount로 DialogueWriter 실행
        }
        else
        {
            Debug.LogError("Selected dialogue index is out of bounds. Ending dialogue.");
            EndDialogue();
        }
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

        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        playerMove.ActiveInteract = false; // 추가 코드
    }

    void EndSelect()
    {
        isSelect = false;
        selects = null;

        selectBtn1.gameObject.SetActive(false);
        selectBtn2.gameObject.SetActive(false);
        selectText1.gameObject.SetActive(false);
        selectText2.gameObject.SetActive(false);
    }

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



    IEnumerator SelectWriter()
    {
        Button[] buttons = { selectBtn1, selectBtn2 };
        Text[] texts = { selectText1, selectText2 };

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

                        buttons[j].onClick.RemoveAllListeners();
                        buttons[j].onClick.AddListener(() => OnSelectButtonClicked(selectedMoveNumInt));
                    }
                }
            }
        }
    }
}
