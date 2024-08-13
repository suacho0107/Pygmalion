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

    Dialogue[] dialogues;

    bool isDialogue = false;
    bool isNext = false; //Ư�� Ű �Է� ���

    int lineCount = 0; //��ȭ ī��Ʈ
    int contextCount = 0; //��� ī��Ʈ

    public PlayerMove playerMove; //�÷��̾� FSM�� ����

    // ���� �÷��� �� �г� ��Ȱ��ȭ�� �� �ż� �߰�
    private void Start()
    {
        dialoguePanel.SetActive(false);
        namePanel.SetActive(false);
    }

    private void Update()
    {
        if (isDialogue && isNext)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space �Է�");

                isNext = false;
                dialogueText.text = "";

                if (++contextCount < dialogues[lineCount].contexts.Length)
                {
                    StartCoroutine(TypeWriter()); //���� name ���� context�� ����
                }
                else
                {
                    contextCount = 0;
                    if (++lineCount < dialogues.Length)
                    {
                        StartCoroutine(TypeWriter()); //name�� context ��� ����
                    }
                    else
                    {
                        EndDialogue();
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

        StartCoroutine(TypeWriter());
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
        playerMove.ActiveInteract = false;
    }

    IEnumerator TypeWriter()
    {
        Debug.Log(dialogues[lineCount].name);
        if(dialogues[lineCount].name != "")
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(true);
        }
        else
        {
            dialoguePanel.SetActive(true);
            namePanel.SetActive(false);
        }

        string t_ReplaceText = dialogues[lineCount].contexts[contextCount];
        Debug.Log("TypeWriter(): " + t_ReplaceText);
        t_ReplaceText = t_ReplaceText.Replace("#", ","); //#�� ,�� ��ȯ

        nameText.text = dialogues[lineCount].name;

        for (int i = 0; i < t_ReplaceText.Length; i++)
        {
            dialogueText.text += t_ReplaceText[i];
            yield return new WaitForSeconds(0.03f);
        }

        isNext = true;
    }
}
