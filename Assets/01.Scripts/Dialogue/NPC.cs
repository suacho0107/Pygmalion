using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    [Header("��ȭ �ý���")]
    public DialogueManager dialogueManager;
    public InteractionEvent interactionEvent; // �� NPC�� ����� InteractionEvent
    public MuseumLobbyCSV csv; // A�� ��ȣ�ۿ� ���� �� B ��ȭ ���� ����, csv ���� ��� �ѹ��� �����ϴ� ��ũ��Ʈ�� ������ �� ������?
    public StatueScore statueScore;
    //public StatueController statueController;

    public bool isStatue = false;
    public bool isChecked;
    public bool isJudged = false;
    public bool isEnemy = false;
    public bool isCorrect;
    public bool tutorial = false;
    public bool isInteract = false;
    public bool isEnd = false;

    bool isDialogueChanged = false;
    bool isFin = false;
    int lastCount = -1;

    [SerializeField] public string dialogueFileName;
    [SerializeField] public string selectFileName;
    [SerializeField] public string explainNum;
    [SerializeField] public string[] dialogueFiles; // ���� ���� �迭 �߰�
    [SerializeField] public string[] selectFiles;
    public int currentIndex = 0;

    //private void OnMouseDown()
    //{
    //    StartDialogue();
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
        if(tutorial && csv != null)// �̼����� tutorial V
        {
            // �̼�������� ù ��ȭ�� ������ isInteract == true;
            if(isInteract)
            {
                if (!isDialogueChanged)
                {
                    csv.npcs[1].ChangeDialogueFile(); // ������(npcs[1])�� ��ȭ ���� ����
                    isDialogueChanged = true;
                }

                // ������ �Ǻ� ������ ���� ��ȭ ���� ����
                if (statueScore != null && isDialogueChanged && !isFin && statueScore.statueCount != lastCount)
                {
                    lastCount = statueScore.statueCount;

                    if (lastCount == 1)
                    {
                        ChangeDialogueFile();
                    }
                    else if (lastCount > 1 && lastCount < 6)
                    {
                        ChangeDialogueFile();
                    }
                    else if (lastCount == 6)
                    {
                        ChangeDialogueFile();
                        isFin = true;
                    }
                }
            }
        }

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
                        StatueScore statueScore = FindObjectOfType<StatueScore>();
                        statueScore.statueCount =+ 1;
                    }
                    else
                    {// �ǵ帰�� --> ���� --> �������� ������ ������������... --> statueState.Destroyed
                        Debug.Log("�ǵ帰�� > ����");
                        ChangeDialogueFile("2");
                        statueScore.statueCount = +1;
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

    public void ChangeDialogueFile(string _explainNum = null)
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
