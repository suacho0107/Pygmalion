using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using static PlayerMove;

public class NPC : MonoBehaviour
{
    [Header("대화 시스템")]
    public DialogueManager dialogueManager;
    public InteractionEvent interactionEvent; // 이 NPC와 연결된 InteractionEvent
    public MuseumLobbyCSV csv;
    public StatueScore statueScore;
    public NPCData npcData = new NPCData();

    public SpriteRenderer spriteRenderer;
    public Sprite destroyedSprite; // 무너진 조각상 스프라이트
    //public StatueController statueController;

    public bool isStatue = false;
    public bool isChecked = false;
    public bool isJudged = false;
    public bool isEnemy = false;
    public bool isCorrect = false;
    public bool tutorial = false;
    public bool isInteract = false;
    public bool isEnd = false;

    public bool isTutoDialogueChanged = false;
    public bool isTutoFin = false;
    bool isDialogueChanged = false;
    public bool isFin = false;
    public bool result = false;

    public bool isSpriteChanged = false;

    string filePath;
    string currentName;

    [SerializeField] public string dialogueFileName;
    [SerializeField] public string selectFileName;
    [SerializeField] public string explainNum;
    [SerializeField] public string[] dialogueFiles; // 파일 변경 배열 추가
    [SerializeField] public string[] selectFiles;
    public int currentIndex = 0;

    private void Awake()
    {
        filePath = Application.persistentDataPath + "/" + gameObject.name + "_data.json";
        LoadNPCData();
    }
    private void Start()
    {
        // ResetNPCData();
        LoadNPCData();

        //if (isStatue)
        //{
        //    isChecked = false;
        //}
    }
    private void Update()
    {
        //Debug.Log(lastCount);
        if(tutorial && csv != null)// 미술관장 tutorial V
        {
            // 미술관장과의 첫 대화가 끝나면 isInteract == true;
            if(isInteract)
            {
                if (!isTutoDialogueChanged)
                {
                    csv.npcs[0].ChangeDialogueFile(); // 조각상(npcs[0])의 대화 파일 변경
                    isTutoDialogueChanged = true;
                    SaveNPCData();
                }

                if (isTutoDialogueChanged)
                {
                    if (statueScore.statueCount == 1 && !isTutoFin)
                    {
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

        if (isStatue && isChecked)
        {
            isChecked = true;
            if (!isDialogueChanged)
            {
                ChangeDialogueFile();
                isDialogueChanged = true;
            }
            else
            {
                // 튜토 후 전시실1에서, 판별로 넘어는 갔는데 선택지 4개가 안 나옴, 그대로 2개
                if (isEnemy && isJudged)
                {
                    if (isCorrect)
                    {// 건드린다 --> 정답 --> battleDialogue.csv --> 전투 진입(플레이어 선공)
                        Debug.Log("건드린다 > 정답");
                        isCorrect = true;
                        ChangeDialogueFile("1");
                        StartCoroutine(DelayLoadScene(1.5f, "Demo_minjoo"));
                    }
                    else
                    {// 이상 없음 --> 오답 --> 기록 효과~ --> 전투 진입(적 선공)
                        Debug.Log("이상 없음 > 오답");
                        isCorrect = false;
                        SceneManager.LoadScene("Demo_minjoo");
                    }
                }
                else if (!isEnemy && isJudged && !isFin)
                {
                    if (isCorrect)
                    {// 이상 없음 --> 정답 --> 기록 효과~ --> count++
                        Debug.Log("이상 없음 > 정답");
                        //StatueScore statueScore = FindObjectOfType<StatueScore>();
                        statueScore.statueCount += 1;
                        statueScore.SaveScore();
                        isCorrect = true;
                        isFin = true;
                        Debug.Log("currentIdnex " +currentIndex);
                        SaveNPCData();
                    }
                    else
                    {// 건드린다 --> 오답 --> 조각상이 힘없이 무너져내린다... --> statueState.Destroyed
                        Debug.Log("건드린다 > 오답");
                        ChangeDialogueFile("2");
                        ChangeSprite();
                        statueScore.statueCount += 1;
                        statueScore.destroyedCount += 1;
                        statueScore.SaveScore();
                        isCorrect = false;
                        isSpriteChanged = true;
                        isFin = true;
                        Debug.Log("currentIdnex " + currentIndex);
                        SaveNPCData();
                        //statueController.sState = statueController.StatueState.Destroyed;
                    }
                }
                
                if (isDialogueChanged && isFin && result && !isEnemy)
                {
                    Debug.Log("result 출력");
                    //string currentName;
                    //dialogueFileName = currentName;
                    if (isCorrect == true) // statueDialogue: BattleDialogue.csv ID 3
                    {
                        Debug.Log("statueDialogue 출력");
                        currentIndex = 2;
                        explainNum = "3";
                        ChangeDialogueFile();
                    }
                    else // 무너져 있다: 공통 Destroyed.csv
                    {
                        currentIndex = 3;
                        explainNum = "1";
                        ChangeDialogueFile();
                        //dialogueFileName = "Destroyed_dialogue";
                        //SaveNPCData();
                    }
                    //PlayerMove playerMove = FindObjectOfType<PlayerMove>();
                    //if(playerMove.pState == PlayerState.Interaction)
                    //{
                    //    if (isCorrect == true) // 무너져 있다: 공통 Destroyed.csv
                    //    {
                    //        Debug.Log("statueDialogue 출력");
                    //        //currentIndex = 2;
                    //        ChangeDialogueFileName("battle1_dialogue");
                    //    }
                    //    else // statueDialogue: BattleDialogue.csv ID 3
                    //    {
                    //        ChangeDialogueFileName("Destroyed_dialogue");
                    //        //dialogueFileName = "Destroyed_dialogue";
                    //        //SaveNPCData();
                    //    }
                    //}
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
        else //null 처리
        {
            Debug.LogError("DialogueManager is null.");
        }

        InteractionEvent interactionEvent = GetComponent<InteractionEvent>();
        if (interactionEvent != null)
        {

            if (!string.IsNullOrEmpty(explainNum)) //explainNum 있으면 전달
            {
                interactionEvent.LoadDialogue(dialogueFileName, explainNum);
            }
            else //explainNum 없으면 그냥
            {
                interactionEvent.LoadDialogue(dialogueFileName);
            }
        }
    }

    void ChangeDialogueFileName(string _dialogueFileName)
    {
        dialogueFileName = _dialogueFileName;
        currentName = dialogueFileName;
        Debug.Log(dialogueFileName);
        //SaveNPCData();
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
                Debug.Log("대화: " + dialogueFileName + ", 선지: " + selectFileName);
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
                currentName = dialogueFileName;
                Debug.Log("대화: " + dialogueFileName + ", 선지: " + selectFileName);
                if(isJudged && ((!isCorrect && !isEnemy) || (isCorrect && isEnemy)))
                {
                    StartCoroutine(TriggerDialogue());
                }
            }
        }
        //SaveNPCData();
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

    public void SaveNPCData()
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

        string json = JsonUtility.ToJson(npcData);
        File.WriteAllText(filePath, json);
        Debug.Log("데이터 저장");
    }

    public void LoadNPCData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            npcData = JsonUtility.FromJson<NPCData>(json);
            Debug.Log("데이터 로드");

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
        }
    }

    public void ResetNPCData()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("NPC 데이터 초기화 : " + filePath);
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
