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
    public PlayerMove playerMove; //�÷��̾� FSM�� ����, �߰� �ڵ�
    StatueScore statueScore;
    MuseumLobbyCSV csv;

    bool isDialogue = false;
    bool isNext = false; //Ư�� Ű �Է� ���
    bool isSelect = false;
    bool isExplain = false; //���������� ����
    bool isPopup = false;

    int lineCount = 0; //��ȭ ī��Ʈ
    int contextCount = 0; //��� ī��Ʈ

    public void SetNPC(NPC _npc)
    {
        npc = _npc; //NPC �ν��Ͻ� ����

        interactionEvent = npc.interactionEvent; //npc�� interactionEvent ��������

        if (npc != null)
        {
            interactionEvent = npc.interactionEvent; //npc�� interactionEvent ��������
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

        playerMove = FindObjectOfType<PlayerMove>(); //�÷��̾� FSM�� ����, �߰� �ڵ�
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

                //skipNum�� ������
                if (!string.IsNullOrEmpty(dialogues[lineCount].skipNum[contextCount]))
                {
                    int skipLine;
                    if (int.TryParse(dialogues[lineCount].skipNum[contextCount], out skipLine))
                    {
                        lineCount = skipLine - 2; //�� -2������ �𸣰ڴµ�.... ·�� �̷��� �ϸ� ����� ���ư�
                        contextCount = 0;
                    }
                }

                //eventNum�� ������: ������ȭ
                if (!string.IsNullOrEmpty(dialogues[lineCount].eventNum[contextCount]))
                {
                    if (!string.IsNullOrEmpty(npc.selectFileName)) //selectFileName ���� Ȯ��
                    {
                        interactionEvent.LoadSelect(npc.selectFileName);

                        if (interactionEvent.Select != null && interactionEvent.Select.selects.Length > 0)
                        {
                            //�� ��ȭ�� ������� 2���� �� ���ĺ����� �ϴ� ������
                            //int eNum =  int.Parse(dialogues[lineCount].eventNum[contextCount]);
                            //Debug.Log("ShowSelect ��: " + interactionEvent.Select.selects);
                            ShowSelect(interactionEvent.Select.selects);
                        }
                        else
                        {
                            Debug.LogWarning("Selects could not be loaded or are empty.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("npc.selectFileName ����");
                    }
                }
                else //eventNum�� ������: ���� ���� �׳� ��ȭ
                {
                    if (++contextCount < dialogues[lineCount].contexts.Length) //line�� contexts.Length �̸��̸�
                    {
                        StartCoroutine(DialogueWriter()); //���� name ���� context�� ����
                    }
                    else //line�� �Ѱܾ� �ϸ�
                    {
                        if (!isExplain) //������ �ƴϸ� line�� �ѱ�
                        {
                            contextCount = 0;
                            if (++lineCount < dialogues.Length)
                            {
                                StartCoroutine(DialogueWriter()); //name�� context ��� ����
                            }
                            else
                            {
                                EndDialogue();
                            }
                        }
                        else //�����̸� �� ����
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

        if (!string.IsNullOrEmpty(explainNum)) //explainNum ������
        {
            // imageImage�� ���� ���� �ڷᱸ���� explainNum ������ �ε����� ����� �̹��� �Ҵ� �� Ȱ��ȭ.
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
                    lineCount = explainLine - 1; //explainLine��° line���� �̵�; �ٵ� �� -1���� �𸣰���... �׳� �� ���ư�
                    contextCount = 0;
                }
                else //����ó��
                {
                    Debug.LogError("Invalid explainNum. Starting from the first dialogue.");
                    lineCount = 0; // explainNum�� �߸��� ��� ù ��° ��ȭ�� ����
                }
            }
            else //����ó��
            {
                Debug.LogError("Failed to parse explainNum. Starting from the first dialogue.");
                lineCount = 0; // explainNum �Ľ� ���� �� ù ��° ��ȭ�� ����
            }


        }
        else //explainNum ������ �׳� ó������
        {
            lineCount = 0;
        }

        StartCoroutine(DialogueWriter()); //��ȭ ����
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

    public void OnSelectButtonClicked(int selectedIndex, int currentIndex) // �Ǻ� �Ű����� �߰�(currentIndex)
    {
        //Debug.Log("���� ��ư:" + currentIndex);
        if (npc.isStatue)
        {
            if (!npc.isChecked && currentIndex == 0) // ù ��° ��ȣ�ۿ�(����): ���� 2�� ���
            {
                npc.isChecked = true;
                npc.SaveNPCData();
                Debug.Log("statue.isChecked == True");
            }
            else if (!npc.isChecked && currentIndex == 1)
            {
                Debug.Log("1) �״�� �д�");
            }
            else if (npc.isChecked && currentIndex == 0 ) // �� ��° ��ȣ�ۿ�(�Ǻ�): ���� 4�� ���
            {
                Debug.Log("2) �ٽ� ���캻��");
                statueScore.checkedCount += 1;
                statueScore.SaveScore();
            }
            else if (npc.isChecked && currentIndex == 1 )
            {
                statueScore.checkedCount += 1;
                statueScore.SaveScore();

                if (npc.isEnemy)
                {// �ǵ帰�� --> ����
                    npc.isJudged = true;
                    npc.isCorrect = true;
                    npc.currentIndex = 3;
                    npc.explainNum = "1";
                    //statueScore.fightCount += 1;
                    //statueScore.SaveScore();
                    //npc.SaveNPCData();
                }
                else
                {// �ǵ帰�� --> ����
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
                {// �̻� ���� --> ����
                    npc.isJudged = true;
                    npc.isCorrect = false;
                    npc.explainNum = "2";
                    //statueScore.fightCount += 1;
                    //statueScore.SaveScore();      //NPC ��ũ��Ʈ���� ����
                    //npc.SaveNPCData();
                }
                else
                {// �̻� ���� --> ����
                    npc.isJudged = true;
                    npc.isCorrect = true;
                    npc.explainNum = "3";
                    //npc.SaveNPCData();
                }
            }
            else if (npc.isChecked && currentIndex == 3)
            {// �״�� �д�
                Debug.Log("2) �״�� �д�");
            }
        }

        int targetLineCount = (int)selectedIndex - 1;

        if (targetLineCount >= 0 && targetLineCount < dialogues.Length) //targetLineCount�� 0 �̻��̰�, dialogues �ȿ� ������
        {
            lineCount = targetLineCount; //lineCount ���� ����
            contextCount = 0; //contextCount �ʱ�ȭ
            EndSelect(); //Select End�ϱ�
            StartCoroutine(DialogueWriter()); //������ lineCount, contextCount�� DialogueWriter ����
        }
        else if (selectedIndex == 0) // ���� ���� ���� ��ȭ ����
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
        //�ʱ�ȭ
        isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;
        isExplain = false;
        npc.isInteract = true; // �̼�����
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
                        Debug.Log("dialogueManager ����, " + npc.dialogueFileName);
                        npc.result = true;
                    }
                }
                //if (npc.isCorrect)
                //{
                //    if (npc.currentIndex == 3)
                //    {
                //        npc.result = true;
                //        Debug.Log("result True �׽�Ʈ �Ϸ� �� ������ �α�");
                //    }
                //}
                //else
                //{
                //    if (npc.currentIndex == 3)
                //    {
                //        npc.result = true;
                //        Debug.Log("result True �׽�Ʈ �Ϸ� �� ������ �α�");
                //    }
                //}
            }
        }
        npc.SaveNPCData();

        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        playerMove.ActiveInteract = false; // �߰� �ڵ�

        // ��� UI ���
        if (npc.dialogueFileName == "Check3_dialogue")
        {
            Invoke("SetUIStateEnd", 1.5f);
        }

        // ��� �̹��� ��Ȱ��ȭ
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

    #region �˾� �̹��� ����
    IEnumerator DialogueWriter()
    {
        if (dialogues[lineCount].name != "") //��翡 name ������
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(true);
        }
        else //name ������
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(false);
        }

        string replaceText = dialogues[lineCount].contexts[contextCount];
        replaceText = replaceText.Replace("#", ","); //#�� ,�� ��ȯ
        replaceText = replaceText.Replace("@", "\n"); //*�� \n���� ��ȯ

        nameText.text = dialogues[lineCount].name; //name ���

        //context ���
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
            if (selects[i].contexts.Length > 1) //������ 2�� �̻� �����ϸ�
            {
                for (int j = 0; j < selects[i].contexts.Length; j++)
                {
                    if (i < buttons.Length) // �迭 ���� ������ Ȯ��
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

                        int currentSelectNum = j;// �Ǻ� �߰� �ڵ�

                        buttons[j].onClick.RemoveAllListeners();
                        buttons[j].onClick.AddListener(() => OnSelectButtonClicked(selectedMoveNumInt, currentSelectNum)); // �Ǻ� �Ű����� �߰�
                    }
                }
            }
        }
    }
}
