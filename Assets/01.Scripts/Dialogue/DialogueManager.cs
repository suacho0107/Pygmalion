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
    bool isNext = false; //특정 키 입력 대기

    int lineCount = 0; //대화 카운트
    int contextCount = 0; //대사 카운트

    public PlayerMove playerMove; //플레이어 FSM과 연결

    // 최초 플레이 시 패널 비활성화가 안 돼서 추가
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
                Debug.Log("Space 입력");

                isNext = false;
                dialogueText.text = "";

                if (++contextCount < dialogues[lineCount].contexts.Length)
                {
                    StartCoroutine(TypeWriter()); //같은 name 밑의 context만 변경
                }
                else
                {
                    contextCount = 0;
                    if (++lineCount < dialogues.Length)
                    {
                        StartCoroutine(TypeWriter()); //name과 context 모두 변경
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
        //초기화
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
        t_ReplaceText = t_ReplaceText.Replace("#", ","); //#을 ,로 변환

        nameText.text = dialogues[lineCount].name;

        for (int i = 0; i < t_ReplaceText.Length; i++)
        {
            dialogueText.text += t_ReplaceText[i];
            yield return new WaitForSeconds(0.03f);
        }

        isNext = true;
    }
}
