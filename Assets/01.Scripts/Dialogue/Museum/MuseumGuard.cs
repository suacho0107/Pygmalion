using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MuseumGuard : StageNPC
{
    public bool uncontacted = false;

    protected override void Awake()
    {
        base.Awake();
        LoadMGuardData();
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Museum_Garden"))
        {
            if (dialogueManager == null)
            {
                dialogueManager = FindObjectOfType<DialogueManager>();
            }

            if(dialogueUI == null)
            {
                dialogueUI = FindObjectOfType<DialogueUI>();
            }

            //Debug.Log("linecount: " + dialogueManager.LineCount());

            // 주인공 알아차리는 대사 전 뒤돌아있기
            if (InventoryUI.instance.HasItem(10401))
            {
                if (!isInteract && !dialogueManager.MessageTrue() && dialogueManager.DialogueTrue())
                {
                    uncontacted = false;
                }
            }
            else
            {
                if (!isInteract && dialogueUI.LineCount() == 0)
                {
                    uncontacted = true;
                }
                else
                {
                    uncontacted = false;
                }
            }

            if (!questEnd)
            {
                #region 퀘스트 시작 전
                if (!questStart)
                {
                    #region 최초 상호작용시
                    if (!isInteract) // 최초 상호작용
                    {
                        if (InventoryUI.instance.HasItem(10401)) // 최초 상호작용, 퀘스트 시작 전 아이템 소지
                        {
                            ChangeDialogueFile(3);

                            if (dialogueManager.isEnd) // isEnd는 Message와 관련 없음 --> DialogueTrue, MessageTrue 확인 필요 없음
                            {
                                questEnd = true;
                                InventoryUI.instance.GetAnItem(10402);
                                InventoryUI.instance.RemoveInventoryItem(10401);
                                SaveMGuardData();
                                dialogueManager.isEnd = false;
                            }
                        }
                        else // 최초 상호작용, 퀘스트 시작 전 아이템 미소지
                        {
                            if (dialogueUI.buttonIndexNPC == 2) // 퀘스트 수락
                            {
                                //Debug.Log("최초 상호작용, 퀘스트 시작 전 아이템 미소지, 퀘스트 수락");
                                if (dialogueManager.isEnd)
                                {
                                    //Debug.Log("퀘스트 수락, isEnd");
                                    questStart = true;
                                    if (InventoryUI.instance.HasItem(10401)) ChangeDialogueFile(1);
                                    else ChangeDialogueFile(2);
                                    SaveMGuardData();
                                    dialogueManager.isEnd = false;
                                }
                            }
                            else // 퀘스트 거절
                            {
                                if (dialogueManager.isEnd)
                                {
                                    questStart = false;
                                    ChangeDialogueFile(4);
                                    SaveMGuardData();
                                    dialogueManager.isEnd = false;
                                }
                            }
                        }
                    }
                    #endregion
                    #region 최초 상호작용 후
                    else if (InventoryUI.instance.HasItem(10401)) // 최초 상호작용 후, 퀘스트 시작 전 템 소지
                    {
                        ChangeDialogueFile(3);
                        if (dialogueManager.isEnd)
                        {
                            questEnd = true;
                            InventoryUI.instance.GetAnItem(10402);
                            InventoryUI.instance.RemoveInventoryItem(10401);
                            SaveMGuardData();
                            dialogueManager.isEnd = false;
                        }
                    }
                    else if (!InventoryUI.instance.HasItem(10401)) // 최초 상호작용 후, 퀘스트 시작 전 템 미소지
                    {
                        ChangeDialogueFile(4);

                        if (dialogueManager.isEnd) // 이거 왜 넣은 거지: 아마 다른 맵 넘어가기 전에 변수 상태 저장하려고
                        {
                            SaveMGuardData();
                            dialogueManager.isEnd = false;
                        }

                        if (dialogueUI.buttonIndexNPC == 2) // 퀘스트 수락
                        {
                            questStart = true;
                            SaveMGuardData();
                        }
                    }
                    #endregion
                }
                #endregion
                #region 퀘스트 시작 후
                else // questStart
                {
                    if (InventoryUI.instance.HasItem(10401)) // 퀘스트 시작, 아이템 소지
                    {
                        ChangeDialogueFile(1);
                        if (dialogueManager.isEnd)
                        {
                            questEnd = true;
                            InventoryUI.instance.GetAnItem(10402);
                            InventoryUI.instance.RemoveInventoryItem(10401);
                            SaveMGuardData();
                            dialogueManager.isEnd = false;
                        }
                    }
                    else // 퀘스트 시작, 아이템 미소지
                    {
                        ChangeDialogueFile(2);
                        if (dialogueManager.isEnd)
                        {
                            SaveMGuardData();
                            dialogueManager.isEnd = false;
                        }
                    }
                }
                #endregion
            }
            else // questEnd
            {
                ChangeDialogueFileName("Museum-Guard2_dialogue");
            }
        }
    }

    public void SaveMGuardData()
    {
        npcData.questStart = questStart;
        npcData.questEnd = questEnd;

        npcData.isDialogueChanged = isDialogueChanged;
        npcData.currentIndex = currentIndex;
        npcData.dialogueFileName = dialogueFileName;
        npcData.selectFileName = selectFileName;
        npcData.isInteract = isInteract;

        string json = JsonUtility.ToJson(npcData);
        File.WriteAllText(filePath, json);

        Debug.Log(gameObject.name + " 데이터 저장");
    }

    public void LoadMGuardData()
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
            questStart = npcData.questStart;
            questEnd = npcData.questEnd;
        }

        Debug.Log(gameObject.name + " 데이터 로드");
    }
}
