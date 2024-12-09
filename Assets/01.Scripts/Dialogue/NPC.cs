using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using static PlayerMove;
using UnityEditor.SearchService;
using Define;

public class NPC : MonoBehaviour
{
    #region Set Values
    public DialogueManager dialogueManager;
    public InteractionEvent interactionEvent; // �� NPC�� ����� InteractionEvent
    public MuseumLobbyCSV csv;
    public StatueScore statueScore;
    public NPCData npcData = new NPCData();
    //public FightDataTest fightData;

    public SpriteRenderer spriteRenderer;
    public Sprite destroyedSprite; // ������ ������ ��������Ʈ

    StatueAudio statueAudio;

    public bool isStatue = false;
    public bool isNPC;
    public bool isChecked = false;
    public bool isJudged = false;
    public bool isEnemy = false;
    public bool isCorrect = false;
    public bool tutorial = false;
    public bool isInteract = false;

    public bool isTutoDialogueChanged = false;
    public bool isTutoFin = false;
    bool isDialogueChanged = false;
    public bool isFin = false;
    public bool result = false;

    bool test1;
    bool test2;
    bool test3;
    bool test4;

    public bool isSpriteChanged = false;

    string filePath;
    string currentName;
    #endregion

    [SerializeField] public string dialogueFileName;
    [SerializeField] public string selectFileName;
    [SerializeField] public string explainNum;
    [SerializeField] public string[] dialogueFiles; // ���� ���� �迭 �߰�
    [SerializeField] public string[] selectFiles;
    public int currentIndex = 0;

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/" + gameObject.name + "_data.json";
        LoadNPCData();
    }
    private void Start()
    {
        //ResetNPCData();
        LoadNPCData();

        statueAudio = GetComponent<StatueAudio>();
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
                    csv.npcs[0].ChangeDialogueFile(1); // ������(npcs[0])�� ��ȭ ���� ����
                    isTutoDialogueChanged = true;
                    SaveNPCData();
                }

                if (isTutoDialogueChanged)
                {
                    if (!isTutoFin && statueScore.statueCount == 1)
                    {
                        Debug.Log("!isTutoFin, Ʃ��2�� ����");
                        ChangeDialogueFileName("Tutorial2_dialogue");
                    }
                    if (isTutoFin)
                    {
                        if (statueScore.statueCount == 1)
                        {
                            ChangeDialogueFileName("Check1_dialogue");
                        }
                        else if (statueScore.statueCount > 1 && statueScore.statueCount < 6)
                        {
                            ChangeDialogueFileName("Check2_dialogue");
                        }
                        else if (statueScore.statueCount == 6)
                        {
                            ChangeDialogueFileName("Check3_dialogue");
                        }
                    }
                }
            }
        }

        //if (SceneManager.GetActiveScene().name == "Library_1F" && isNPC) // ������ 1�� ����
        //{
        //    if (isInteract)
        //    {
        //        ChangeDialogueFileName("Guard_Check0_dialogue");

        //        if (statueScore.statueCount == 1)
        //        {
        //            ChangeDialogueFileName("Guard_Check1_dialogue");
        //        }
        //        else if (statueScore.statueCount > 1 && statueScore.statueCount < 5)
        //        {
        //            ChangeDialogueFileName("Guard_Check2_dialogue");
        //        }
        //        else if (statueScore.statueCount == 5)
        //        {
        //            ChangeDialogueFileName("Guard_Check3_dialogue");
        //        }
        //    }
        //}

        if(SceneManager.GetActiveScene().name == "Museum_Lobby")
        {
            Judge();
        }
        else
        {
            if (statueScore != null)
            {
                Debug.Log("statueScore != null");
                //string sceneName = SceneManager.GetActiveScene().name;
                if (statueScore.statueCount >= 1 && !isChecked && !isJudged && !isFin)
                {
                    Debug.Log("�⺻��� -> �Ǻ�");
                    ChangeDialogueFile(1);
                    Judge();
                }
                else
                {
                    Debug.Log("�Ǻ�");
                    Judge();
                }
                //if (sceneName.StartsWith("Museum"))
                //{
                //    if (statueScore.statueCount >= 1 && !isChecked && !isJudged && !isFin)
                //    {
                //        Debug.Log("�⺻��� -> �Ǻ�");
                //        ChangeDialogueFile(1);
                //        Judge();
                //    }
                //    else
                //    {
                //        Debug.Log("�Ǻ�");
                //        Judge();
                //    }
                //}
                //else if (sceneName.StartsWith("Library"))
                //{
                //    Judge();
                //}
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

    public void Judge()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.StartsWith("Museum"))
        {
            if (currentIndex == 1 || currentIndex == 2)
            {
                explainNum = null;
            }

            if (isStatue && isChecked)
            {
                isChecked = true;

                if (!isJudged)
                {
                    ChangeDialogueFile(2);
                }

                if (isEnemy && isJudged)
                {
                    if (isCorrect && !isFin && !test2)
                    {// �ǵ帰�� --> ���� --> battleDialogue.csv --> ���� ����(�÷��̾� ����)
                        Debug.Log("�ǵ帰�� > ����");
                        isCorrect = true;
                        statueAudio.PlayEnterFight();
                        statueScore.fightCount += 1;
                        statueScore.SaveScore();
                        ChangeDialogueExplain(3, "1");
                        test2 = true;
                        StartCoroutine(DelayLoadScene(2.2f, "Battle"));
                    }
                    else if (!isCorrect && !isFin)
                    {// �̻� ���� --> ���� --> ��� ȿ��~ --> ���� ����(�� ����)
                        Debug.Log("�̻� ���� > ����");
                        if (!test2)
                        {
                            statueAudio.PlayPencil();
                            statueScore.fightCount += 1;
                            statueScore.SaveScore();
                            test2 = true;
                        }
                        isCorrect = false;
                        ChangeDialogueExplain(3, "1");
                        StartCoroutine(PlaySound());
                    }
                    else if (isFin) // ���� �¸� �� ������ ������ ��ȭ��?
                    {
                        ChangeDialogueFileName("Destroyed_dialogue");
                        ChangeSprite();
                        if (!test4)
                        {
                            statueAudio.PlayDestroyed();
                            statueScore.statueCount += 1;
                            statueScore.SaveScore();
                            test4 = true;
                        }
                    }
                }
                else if (!isEnemy && isJudged && !isFin)
                {
                    if (isCorrect)
                    {// �̻� ���� --> ���� --> ��� ȿ��~ --> count++
                        Debug.Log("�̻� ���� > ����");
                        ChangeDialogueExplain(3, "3");
                        statueAudio.PlayPencil();
                        statueScore.statueCount += 1;
                        statueScore.SaveScore();
                        isCorrect = true;
                        isFin = true;
                        SaveNPCData();
                    }
                    else
                    {// �ǵ帰�� --> ���� --> �������� ������ ������������... --> statueState.Destroyed
                        Debug.Log("�ǵ帰�� > ����");
                        ChangeDialogueExplain(3, "2");
                        ChangeSprite();
                        statueAudio.PlayDestroyed();
                        statueScore.statueCount += 1;
                        statueScore.destroyedCount += 1;
                        statueScore.SaveScore();
                        isCorrect = false;
                        isSpriteChanged = true;
                        isFin = true;
                        SaveNPCData();
                        //statueController.sState = statueController.StatueState.Destroyed;
                    }
                }

                if (isFin && result && !isEnemy)
                {
                    Debug.Log("result ���");
                    if (isCorrect == true) // statueDialogue: BattleDialogue.csv ID 3
                    {
                        Debug.Log("statueDialogue ���");
                        //currentIndex = 3;
                        ChangeDialogueExplain(3, "3");
                    }
                    else // ������ �ִ�: ���� Destroyed.csv
                    {
                        ChangeDialogueFileName("Destroyed_dialogue");
                    }
                }
            }
        }
        else
        {
            if (currentIndex == 0 || currentIndex == 1)
            {
                explainNum = null;
            }

            if (isStatue && isChecked)
            {
                isChecked = true;

                if (!isJudged)
                {
                    ChangeDialogueFile(1);
                }

                if (isEnemy && isJudged)
                {
                    if (isCorrect && !isFin && !test2)
                    {// �ǵ帰�� --> ���� --> battleDialogue.csv --> ���� ����(�÷��̾� ����)
                        Debug.Log("�ǵ帰�� > ����");
                        isCorrect = true;
                        statueAudio.PlayEnterFight();
                        statueScore.fightCount += 1;
                        statueScore.SaveScore();
                        ChangeDialogueExplain(2, "1");
                        test2 = true;
                        StartCoroutine(DelayLoadScene(2.2f, "Battle"));
                    }
                    else if (!isCorrect && !isFin)
                    {// �̻� ���� --> ���� --> ��� ȿ��~ --> ���� ����(�� ����)
                        Debug.Log("�̻� ���� > ����");
                        if (!test2)
                        {
                            statueAudio.PlayPencil();
                            statueScore.fightCount += 1;
                            statueScore.SaveScore();
                            test2 = true;
                        }
                        isCorrect = false;
                        ChangeDialogueExplain(3, "1");
                        StartCoroutine(PlaySound());
                    }
                    else if (isFin) // ���� �¸� �� ������ ������ ��ȭ��?
                    {
                        ChangeDialogueFileName("Destroyed_dialogue");
                        ChangeSprite();
                        if (!test4)
                        {
                            statueAudio.PlayDestroyed();
                            statueScore.statueCount += 1;
                            statueScore.SaveScore();
                            test4 = true;
                        }
                    }
                }
                else if (!isEnemy && isJudged && !isFin)
                {
                    if (isCorrect)
                    {// �̻� ���� --> ���� --> ��� ȿ��~ --> count++
                        Debug.Log("�̻� ���� > ����");
                        ChangeDialogueExplain(2, "3");
                        statueAudio.PlayPencil();
                        statueScore.statueCount += 1;
                        statueScore.SaveScore();
                        isCorrect = true;
                        isFin = true;
                        SaveNPCData();
                    }
                    else
                    {// �ǵ帰�� --> ���� --> �������� ������ ������������... --> statueState.Destroyed
                        Debug.Log("�ǵ帰�� > ����");
                        ChangeDialogueExplain(2, "2");
                        ChangeSprite();
                        statueAudio.PlayDestroyed();
                        statueScore.statueCount += 1;
                        statueScore.destroyedCount += 1;
                        statueScore.SaveScore();
                        isCorrect = false;
                        isSpriteChanged = true;
                        isFin = true;
                        SaveNPCData();
                        //statueController.sState = statueController.StatueState.Destroyed;
                    }
                }

                if (isFin && result && !isEnemy)
                {
                    Debug.Log("result ���");
                    if (isCorrect == true) // statueDialogue: BattleDialogue.csv ID 3
                    {
                        ChangeDialogueExplain(2, "3");
                    }
                    else // ������ �ִ�: ���� Destroyed.csv
                    {
                        ChangeDialogueFileName("Destroyed_dialogue");
                    }
                }
            }
        }
            
    }

    public void ChangeDialogueFileName(string _dialogueFileName)
    {
        dialogueFileName = _dialogueFileName;
        currentName = dialogueFileName;
        //Debug.Log(dialogueFileName);
    }

    public void ChangeDialogueExplain(int _currentIndex, string _explainNum)
    {
        currentIndex = _currentIndex;
        explainNum = _explainNum;
        if (currentIndex < dialogueFiles.Length - 1)
        {
            dialogueFileName = dialogueFiles[currentIndex];
            selectFileName = selectFiles[currentIndex];
            currentName = dialogueFileName;
            Debug.Log("��ȭ: " + dialogueFileName + ", ����: " + selectFileName);
            if (isJudged && ((!isCorrect && !isEnemy) || (isCorrect && isEnemy)))
            {
                StartCoroutine(TriggerDialogue());
            }
        }
    }

    public void ChangeDialogueFile(int _currentIndex)
    {
        if (currentIndex < dialogueFiles.Length - 1)
        {
            currentIndex = _currentIndex;
            dialogueFileName = dialogueFiles[currentIndex];
            selectFileName = selectFiles[currentIndex];
            Debug.Log("��ȭ: " + dialogueFileName + ", ����: " + selectFileName);
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

    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(1f);
        statueAudio.PlayEnterFight();

        yield return new WaitForSeconds(2f);
        StartCoroutine(DelayLoadScene(2.2f, "Battle"));
    }

    public void SaveNPCData()
    {
        if(isStatue || isNPC)
        {
            npcData.isChecked = isChecked;
            npcData.isJudged = isJudged;
            npcData.isCorrect = isCorrect;
            npcData.isDialogueChanged = isDialogueChanged;
            npcData.currentIndex = currentIndex;
            npcData.dialogueFileName = dialogueFileName;
            npcData.selectFileName = selectFileName;
            npcData.isInteract = isInteract;
            npcData.isTutoDialogueChanged = isTutoDialogueChanged;
            npcData.isTutoFin = isTutoFin;
            npcData.isFin = isFin;
            npcData.result = result;
            npcData.isSpriteChanged = isSpriteChanged;
            npcData.test1 = test1;
            npcData.test2 = test2;
            npcData.test3 = test3;
            npcData.test4 = test4;

            string json = JsonUtility.ToJson(npcData);
            File.WriteAllText(filePath, json);
            Debug.Log("������ ����");
        }
    }

    public void LoadNPCData()
    {
        if(isStatue || isNPC)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                npcData = JsonUtility.FromJson<NPCData>(json);
                Debug.Log("������ �ε�");

                isChecked = npcData.isChecked;
                isJudged = npcData.isJudged;
                isCorrect = npcData.isCorrect;
                isDialogueChanged = npcData.isDialogueChanged;
                currentIndex = npcData.currentIndex;
                dialogueFileName = npcData.dialogueFileName;
                selectFileName = npcData.selectFileName;
                isInteract = npcData.isInteract;
                isTutoDialogueChanged = npcData.isTutoDialogueChanged;
                isTutoFin = npcData.isTutoFin;
                isFin = npcData.isFin;
                result = npcData.result;
                isSpriteChanged = npcData.isSpriteChanged;
                if (isSpriteChanged)
                {
                    ChangeSprite();
                }
                test1 = npcData.test1;
                test2 = npcData.test2;
                test3 = npcData.test3;
                test4 = npcData.test4;
            }
        }
    }

    public void ResetNPCData()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("NPC ������ �ʱ�ȭ : " + filePath);
        }
        npcData = new NPCData();
    }

    void ChangeSprite()
    {
        if (spriteRenderer != null && destroyedSprite != null)
        {
            spriteRenderer.sprite = destroyedSprite;
        }
    }
}
