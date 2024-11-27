using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
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

    bool isTutoDialogueChanged = false;
    bool isTutoFin = false;
    bool isDialogueChanged = false;
    bool isFin = false;
    int lastCount = -1;
    
    [SerializeField] public string dialogueFileName;
    [SerializeField] public string selectFileName;
    [SerializeField] public string explainNum;
    [SerializeField] public string[] dialogueFiles; // ���� ���� �迭 �߰�
    [SerializeField] public string[] selectFiles;
    public int currentIndex = 0;

    private void Start()
    {
        if(isStatue)
        {
            isChecked = false;
        }
        //PlayerPrefs.DeleteAll();
        isInteract = PlayerPrefs.GetInt("isInteract", 0) == 1;
        isTutoDialogueChanged = PlayerPrefs.GetInt("isTutoDialogueChanged", 0) == 1;
        isTutoFin = PlayerPrefs.GetInt("isTutoFin", 0) == 1;
        lastCount = PlayerPrefs.GetInt("lastCount", lastCount);
    }
    private void Update()
    {
        if(tutorial && csv != null)// �̼����� tutorial V
        {
            // �̼�������� ù ��ȭ�� ������ isInteract == true;
            if(isInteract)
            {
                if (!isTutoDialogueChanged)
                {
                    csv.npcs[0].ChangeDialogueFile(); // ������(npcs[0])�� ��ȭ ���� ����
                    isTutoDialogueChanged = true;
                    SaveTuto();
                }

                // ������ �Ǻ� ������ ���� ��ȭ ���� ����
                if (isTutoDialogueChanged && !isTutoFin && statueScore.statueCount != lastCount)
                {
                    // �ƿ� ����� �� �Ѿ���� ��
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
                        isTutoFin = true;
                        SaveTuto();
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
                else if (!isEnemy && isJudged && !isFin)
                {
                    if (isCorrect)
                    {// �̻� ���� --> ���� --> ��� ȿ��~ --> count++
                        Debug.Log("�̻� ���� > ����");
                        //StatueScore statueScore = FindObjectOfType<StatueScore>();
                        statueScore.statueCount += 1;
                        statueScore.SaveScore();
                        isFin = true;
                    }
                    else
                    {// �ǵ帰�� --> ���� --> �������� ������ ������������... --> statueState.Destroyed
                        Debug.Log("�ǵ帰�� > ����");
                        ChangeDialogueFile("2");
                        statueScore.statueCount += 1;
                        statueScore.destroyedCount += 1;
                        statueScore.SaveScore();
                        isFin = true;
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
                //Debug.Log("��ȭ: " + dialogueFileName + ", ����: " + selectFileName);

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

    public void SaveTuto()
    {
        PlayerPrefs.SetInt("isInteract", isInteract ? 1 : 0);
        PlayerPrefs.SetInt("isDialogueChanged", isTutoDialogueChanged ? 1 : 0);
        PlayerPrefs.SetInt("isTutoFin", isTutoFin ? 1 : 0);
        PlayerPrefs.Save();
    }
}
