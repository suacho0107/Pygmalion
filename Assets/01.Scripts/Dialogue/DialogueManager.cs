using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public PlayerMove_origin playerMove; //�÷��̾� FSM�� ����, �߰� �ڵ�

    bool isDialogue = false;
    bool isNext = false; //Ư�� Ű �Է� ���
    bool isSelect = false;

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
        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        selectBtn1.gameObject.SetActive(false);
        selectBtn2.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isDialogue && isNext && !isSelect)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                isNext = false;
                dialogueText.text = "";

                if (!string.IsNullOrEmpty(dialogues[lineCount].skipNum[contextCount]))
                {
                    int skipLine;
                    if (int.TryParse(dialogues[lineCount].skipNum[contextCount], out skipLine))
                    {
                        lineCount = skipLine - 2; //�� -2������ �𸣰ڴµ�.... ·�� �̷��� �ϸ� ����� ���ư�
                        contextCount = 0;
                    }
                }

                // ���� ���� ���θ� Ȯ���ϰ� �ε�
                if (!string.IsNullOrEmpty(dialogues[lineCount].eventNum[contextCount])) //eventNum�� Null�� �ƴϸ�
                {
                    // selectFileName�� ��� ���� ���� ���� ȣ��
                    if (!string.IsNullOrEmpty(npc.selectFileName))
                    {
                        interactionEvent.LoadSelect(npc.selectFileName);

                        if (interactionEvent.Select != null && interactionEvent.Select.selects.Length > 0)
                        {
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
                else
                {
                    if (++contextCount < dialogues[lineCount].contexts.Length)
                    {
                        StartCoroutine(DialogueWriter()); //���� name ���� context�� ����
                    }
                    else
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
                }


            }
        }
    }

    public void ShowDialogue(Dialogue[] _dialogues)
    {
        isDialogue = true;

        dialogueText.text = "";
        nameText.text = "";
        dialogues = _dialogues;

        StartCoroutine(DialogueWriter());
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

        if (targetLineCount >= 0 && targetLineCount < dialogues.Length) //targetLineCount�� 0 �̻��̰�, dialogues �ȿ� ������
        {
            lineCount = targetLineCount; //lineCount ���� ����
            contextCount = 0; //contextCount �ʱ�ȭ
            EndSelect(); //Select End�ϱ�
            StartCoroutine(DialogueWriter()); //������ lineCount, contextCount�� DialogueWriter ����
        }
        else
        {
            Debug.LogError("Selected dialogue index is out of bounds. Ending dialogue.");
            EndDialogue();
        }
    }


    void EndDialogue()
    {
        //�ʱ�ȭ
        isDialogue = false;
        contextCount = 0;
        lineCount = 0;
        dialogues = null;
        isNext = false;

        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
        playerMove.ActiveInteract = false; // �߰� �ڵ�
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
        //Debug.Log(dialogues[lineCount].name);
        if (dialogues[lineCount].name != "")
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(true);
        }
        else
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(false);
        }

        string replaceText = dialogues[lineCount].contexts[contextCount];
        replaceText = replaceText.Replace("#", ","); //#�� ,�� ��ȯ

        nameText.text = dialogues[lineCount].name;

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

                        buttons[j].onClick.RemoveAllListeners();
                        buttons[j].onClick.AddListener(() => OnSelectButtonClicked(selectedMoveNumInt));
                    }
                }
            }
        }
    }
}
