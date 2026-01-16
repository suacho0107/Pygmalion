using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.Progress;

public class StageNPC : NPC
{
    public bool isNPC = true;
    public bool tutorial = false;

    public bool isTutoDialogueChanged = false;
    public bool isTutoFin = false;

    public bool questStart = false;
    public bool questEnd = false;

    string sceneName;

    //bool isDialogueChanged = false;

    //bool once;

    protected override void Awake()
    {
        base.Awake();
        LoadStageNPCData();
    }

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;// Library는 아직 따로 빼는 씬이 없어서 안 해도 될 듯
    }

    private void Update()
    {
        NPCInteraction();
    }

    void NPCInteraction()
    {
        #region Tutorial NPC
        if (sceneName.StartsWith("Museum_Lobby"))
        {
            if (tutorial && csv && statueScore != null)// 미술관장 tutorial V
            {
                // 미술관장과의 첫 대화가 끝나면 isInteract == true;
                if (isInteract)
                {
                    if (!isTutoDialogueChanged)
                    {
                        csv.npcs[0].ChangeDialogueFile(1); // 조각상(npcs[0])의 대화 파일 변경
                        isTutoDialogueChanged = true;
                        SaveStageNPCData();
                    }

                    if (isTutoDialogueChanged)
                    {
                        if (!isTutoFin)
                        {
                            if (statueScore.statueCount == 0)
                            {
                                ChangeDialogueFileName("Check0_dialogue");
                            }
                            else if (statueScore.statueCount == 1)
                            {
                                ChangeDialogueFileName("Tutorial2_dialogue");
                            }
                            else
                            {
                                //Debug.LogError("튜토 미완료");
                            }
                        }
                        else
                        {
                            if (statueScore.statueCount == 1)
                            {
                                ChangeDialogueFileName("Check1_dialogue");
                            }
                            else if (statueScore.statueCount > 1 && statueScore.statueCount < 6)
                            {
                                ChangeDialogueFileName("Check2_dialogue");
                            }
                            else if (statueScore.statueCount >= 6)
                            {
                                ChangeDialogueFileName("Check3_dialogue");
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Library Guard
        else if (SceneManager.GetActiveScene().name == "Library_1F" && isNPC) // 도서관 1층 경비원
        {
            LibraryGuard();

            if (!questEnd) // 열람실 열쇠 상호작용
            {
                string transPath = Application.persistentDataPath + "/interactObj_S_data.json";

                if (File.Exists(transPath))
                {
                    string json = File.ReadAllText(transPath);
                    NPCData transData = JsonUtility.FromJson<NPCData>(json);

                    if (transData.isInteract)
                    {
                        ChangeDialogueFileName("Guard_key_dialogue");
                        DialogueManager dm = FindObjectOfType<DialogueManager>();
                        if (dm.CurrentNPC == this)
                        {
                            InventoryUI.instance.GetAnItem(20101);
                            //if (dm.isEnd) { InventoryUI.instance.GetAnItem(20101); Debug.Log("dm.isEnd"); }
                            questEnd = true;
                        }
                    }
                }
            }
        }
        #endregion
    }

    void LibraryGuard() // 조각상 개수에 따른 대사
    {
        if (statueScore != null)
        {
            ChangeDialogueFileName("Guard_Check0_dialogue");
            if (statueScore.statueCount == 1) ChangeDialogueFileName("Guard_Check1_dialogue");
            else if (statueScore.statueCount > 1 && statueScore.statueCount < 5) ChangeDialogueFileName("Guard_Check2_dialogue");
            else if (statueScore.statueCount == 5) ChangeDialogueFileName("Guard_Check3_dialogue");
        }
    }

    public void TutorialFin()
    {
        isTutoFin = true;
        //Debug.Log("TutorialFin 실행");
    }
    
    public void SaveStageNPCData()
    {
        npcData.isTutoDialogueChanged = isTutoDialogueChanged;
        npcData.isTutoFin = isTutoFin;
        npcData.questStart = questStart;
        npcData.questEnd = questEnd;

        npcData.isDialogueChanged = isDialogueChanged;
        npcData.currentIndex = currentIndex;
        npcData.dialogueFileName = dialogueFileName;
        npcData.selectFileName = selectFileName;
        npcData.isInteract = isInteract;

        string json = JsonUtility.ToJson(npcData);
        File.WriteAllText(filePath, json);

        //Debug.Log(gameObject.name + " 데이터 저장");
        //Debug.Log("미술관장 isTutoFin: " + isTutoFin);
    }

    public void LoadStageNPCData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            npcData = JsonUtility.FromJson<NPCData>(json);

            isDialogueChanged = npcData.isDialogueChanged;
            currentIndex = npcData.currentIndex;
            dialogueFileName = npcData.dialogueFileName;
            selectFileName = npcData.selectFileName;
            isInteract = npcData.isInteract;
            isTutoDialogueChanged = npcData.isTutoDialogueChanged;
            isTutoFin = npcData.isTutoFin;
            questStart = npcData.questStart;
            questEnd = npcData.questEnd;
        }

        Debug.Log(gameObject.name + " 데이터 로드");
    }
}
