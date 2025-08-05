using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class StageNPC : NPC
{
    bool isNPC = true;
    public bool tutorial = false;

    public bool isTutoDialogueChanged = false;
    public bool isTutoFin = false;

    public bool questStart = false;
    public bool questEnd = false;
    //bool isDialogueChanged = false;

    //bool once;

    protected override void Awake()
    {
        base.Awake();
        LoadStageNPCData();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ResetNPCData();
            FieldItemManager.Instance.ResetFieldItems();
            Debug.Log("reset");
        }
        #region Tutorial NPC
        if (SceneManager.GetActiveScene().name == "Museum_Lobby")
        {
            if (tutorial && csv != null)// 미술관장 tutorial V
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
                                Debug.LogError("튜토 미완료");
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
        #region Museum Guard
        else if (SceneManager.GetActiveScene().name == "Museum_Garden")
        {
            if (dialogueManager == null)
            {
                dialogueManager = FindObjectOfType<DialogueManager>();
            }

            if (!questEnd)
            {
                if (!questStart)
                {   // 최초 상호작용
                    if (!isInteract)
                    {
                        if (InventoryUI.instance.HasItem(10401)) // 최초 상호작용, 퀘스트 시작 전 아이템 소지
                        {
                            ChangeDialogueFile(3);
                            if (!dialogueManager.MessageTrue() && dialogueManager.DialogueTrue() && dialogueManager.isEnd) // 열쇠 습득 시 작동하지 않도록
                            {// 여기가 작동을 안 함
                                questEnd = true;
                                InventoryUI.instance.GetAnItem(10402);
                                InventoryUI.instance.RemoveInventoryItem(10401);
                                SaveNPCData();
                                dialogueManager.isEnd = false;
                            }
                        }
                        else
                        {
                            if (dialogueManager.selectedNum == 1) // 퀘스트 수락
                            {
                                if (dialogueManager.isEnd)
                                {
                                    questStart = true;
                                    if (InventoryUI.instance.HasItem(10401)) ChangeDialogueFile(1);
                                    else ChangeDialogueFile(2);
                                    SaveNPCData();
                                    dialogueManager.isEnd = false;
                                }
                            }
                            else
                            {
                                if (dialogueManager.isEnd)
                                {
                                    questStart = false;
                                    ChangeDialogueFile(4);
                                    SaveNPCData();
                                    dialogueManager.isEnd = false;
                                }
                            }
                        }
                        
                        //else                                  // 퀘스트 거절
                        //{
                        //    if (dialogueManager.isEnd)
                        //    {
                        //        questStart = false;
                        //        if (InventoryUI.instance.HasItem(10401))
                        //        {
                        //            ChangeDialogueFile(3);
                        //            questEnd = true;
                        //            InventoryUI.instance.GetAnItem(10402);
                        //        }
                        //        else ChangeDialogueFile(4);
                        //        dialogueManager.isEnd = false;
                        //    }
                        //}
                    }
                    else if (InventoryUI.instance.HasItem(10401)) // 최초 상호작용 X, 퀘스트 시작 전 템 소지
                    {
                        ChangeDialogueFile(3);
                        if (!dialogueManager.MessageTrue() && dialogueManager.DialogueTrue() && dialogueManager.isEnd) // 열쇠 습득 시 작동하지 않도록
                        {
                            questEnd = true;
                            InventoryUI.instance.GetAnItem(10402);
                            InventoryUI.instance.RemoveInventoryItem(10401);
                            SaveNPCData();
                            dialogueManager.isEnd = false;
                        }
                    }
                    else if (!InventoryUI.instance.HasItem(10401))
                    {
                        ChangeDialogueFile(4);

                        if (dialogueManager.isEnd)
                        {
                            SaveNPCData();
                            dialogueManager.isEnd = false;
                        }

                        if (dialogueManager.selectedNum == 1) // 퀘스트 수락
                        {
                            if (dialogueManager.isEnd)
                            {
                                questStart = true;
                                SaveNPCData();
                                dialogueManager.isEnd = false;
                            }
                        }
                    }
                    //// 퀘스트 거절, 최초 상호작용 이후
                    //else if (dialogueFileName == "Museum-Guard1c_dialogue") // 퀘스트 거절, 템 소지
                    //{
                    //    if (dialogueManager.isEnd)
                    //    {
                    //        questEnd = true;
                    //        InventoryUI.instance.GetAnItem(10402);
                    //        InventoryUI.instance.RemoveInventoryItem(10401);
                    //        dialogueManager.isEnd = false;
                    //    }
                    //}
                    //else if(dialogueFileName == "Museum-Guard1d_dialogue") // 퀘스트 거절, 템 미소지
                    //{
                    //    if(dialogueManager.selectedNum == 1) // 퀘스트 수락
                    //    {
                    //        if (dialogueManager.isEnd)
                    //        {
                    //            questStart = true;
                    //            dialogueManager.isEnd = false;
                    //        }
                    //    }
                    //}
                }
                else // questStart
                {
                    if (InventoryUI.instance.HasItem(10401))
                    {
                        ChangeDialogueFile(1);
                        if (!dialogueManager.MessageTrue() && dialogueManager.DialogueTrue() && dialogueManager.isEnd) // 열쇠 습득 시 작동하지 않도록
                        {
                            questEnd = true;
                            InventoryUI.instance.GetAnItem(10402);
                            InventoryUI.instance.RemoveInventoryItem(10401);
                            SaveNPCData();
                            dialogueManager.isEnd = false;
                        }
                    }
                    else
                    {
                        ChangeDialogueFile(2);
                        if (dialogueManager.isEnd)
                        {
                            SaveNPCData();
                            dialogueManager.isEnd = false;
                        }
                    }
                }
            }

            //if (InventoryUI.instance.HasItem(10401))
            //{
            //    ChangeDialogueFile(1);
            //    //InventoryUI.instance.RemoveInventoryItem(10401);
            //    if(dialogueManager == null)
            //    {
            //        dialogueManager = FindObjectOfType<DialogueManager>();
            //    }
            //    if (!dialogueManager.MessageTrue() && dialogueManager.DialogueTrue())
            //    {
            //        InventoryUI.instance.RemoveInventoryItem(10401);
            //    }
            //}

            if (questEnd)
            {
                ChangeDialogueFileName("Museum-Guard2_dialogue");
            }
        }
        #endregion
        #region Library Guard
        else if (SceneManager.GetActiveScene().name == "Library_1F" && isNPC) // 도서관 1층 경비원
        {
            if (isInteract)
            {
                ChangeDialogueFileName("Guard_Check0_dialogue");

                if (statueScore.statueCount == 1)
                {
                    ChangeDialogueFileName("Guard_Check1_dialogue");
                }
                else if (statueScore.statueCount > 1 && statueScore.statueCount < 5)
                {
                    ChangeDialogueFileName("Guard_Check2_dialogue");
                }
                else if (statueScore.statueCount == 5)
                {
                    ChangeDialogueFileName("Guard_Check3_dialogue");
                }
            }
        }
        #endregion

        #region Library Librarian

        #endregion
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
        npcData.questEnd = questEnd;

        npcData.isDialogueChanged = isDialogueChanged;
        npcData.currentIndex = currentIndex;
        npcData.dialogueFileName = dialogueFileName;
        npcData.selectFileName = selectFileName;
        npcData.isInteract = isInteract;

        string json = JsonUtility.ToJson(npcData);
        File.WriteAllText(filePath, json);

        Debug.Log(gameObject.name + " 데이터 저장");
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
            questEnd = npcData.questEnd;
        }

        Debug.Log(gameObject.name + " 데이터 로드");
    }
}
