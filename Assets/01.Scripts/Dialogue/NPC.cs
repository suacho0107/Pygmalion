using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public InteractionEvent interactionEvent; // �� NPC�� ����� InteractionEvent
    //public StatueController statueController;

    public bool isStatue = false;
    public bool isChecked;
    public bool isJudged = false;
    public bool isEnemy = false;
    public bool isCorrect;

    bool isDialogueChanged = false;

    [SerializeField] public string dialogueFileName;
    [SerializeField] public string selectFileName;
    [SerializeField] public string explainNum;
    [SerializeField] public string[] dialogueFiles; // ���� ���� �迭 �߰�
    [SerializeField] public string[] selectFiles;
    public int currentIndex = 0;

    //public bool isChecked = false;

    //private void OnMouseDown()
    //{
    //    StartDialogue();
    //}
    //private void Start()
    //{
    //    dialogueFiles = new string[] { "Tutorial1_dialogue", "Tutorial2_dialogue", "Check1_dialogue", "Check2_dialogue", "Check3_dialogue" };
    //    selectFiles = new string[] { "Tutorial1_select", "", "", "", ""};
    //    dialogueFileName = dialogueFiles[currentIndex];
    //    selectFileName = selectFiles[currentIndex];
    //}

    private void Start()
    {
        if(isStatue)
        {
            isChecked = false;
        }
    }
    private void Update()
    {
        //if (isChecked)
        //{
        //    ChangeDialogueFile();
        //}
        if (isStatue && isChecked)
        {
            if (!isDialogueChanged)
            {
                ChangeDialogueFile();
                isDialogueChanged = true;
            }
            else
            {
                if (isEnemy && isJudged)
                {
                    if (isCorrect)
                    {// �ǵ帰�� --> ���� --> battleDialogue.csv --> ���� ����(�÷��̾� ����)
                        Debug.Log("�ǵ帰�� > ����");
                        ChangeDialogueFile("1");
                        StartCoroutine(DelayLoadScene(1.5f, "Demo_minjoo"));
                    }
                    else
                    {// �̻� ���� --> ���� --> ��� ȿ��~ --> ���� ����(�� ����)
                        Debug.Log("�̻� ���� > ����");
                        SceneManager.LoadScene("Demo_minjoo");
                    }
                }
                else if (!isEnemy && isJudged)
                {
                    if (isCorrect)
                    {// �̻� ���� --> ���� --> ��� ȿ��~ --> count++
                        Debug.Log("�̻� ���� > ����");
                    }
                    else
                    {// �ǵ帰�� --> ���� --> �������� ������ ������������... --> statueState.Destroyed
                        Debug.Log("�ǵ帰�� > ����");
                        ChangeDialogueFile("2");
                        //statueController.sState = statueController.StatueState.Destroyed;
                    }
                }
            }
        }
    }

    public void StartDialogue()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.SetNPC(this);
        }
        else //null ó��
        {
            Debug.LogError("DialogueManager is null.");
        }

        InteractionEvent interactionEvent = GetComponent<InteractionEvent>();
        if (interactionEvent != null)
        {

            if (!string.IsNullOrEmpty(explainNum)) //explainNum ������ ����
            {
                interactionEvent.LoadDialogue(dialogueFileName, explainNum);
            }
            else //explainNum ������ �׳�
            {
                interactionEvent.LoadDialogue(dialogueFileName);
            }
        }
    }

    void ChangeDialogueFile(string _explainNum = null)
    {
        if (string.IsNullOrEmpty(_explainNum))
        {
            if (currentIndex < dialogueFiles.Length - 1)
            {
                currentIndex++;
                dialogueFileName = dialogueFiles[currentIndex];
                selectFileName = selectFiles[currentIndex];
                Debug.Log("��ȭ: " + dialogueFileName + ", ����: " + selectFileName);
            }
        }
        else
        {
            explainNum = _explainNum;
            if (currentIndex < dialogueFiles.Length - 1)
            {
                currentIndex++;
                dialogueFileName = dialogueFiles[currentIndex];
                selectFileName = selectFiles[currentIndex];
                Debug.Log("��ȭ: " + dialogueFileName + ", ����: " + selectFileName);

                StartCoroutine(TriggerDialogue());
            }
        }
    }

    IEnumerator TriggerDialogue()
    {
        yield return new WaitForSeconds(0.1f);
        StartDialogue();
    }

    IEnumerator DelayLoadScene(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
