using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LibraryRoom : NPC
{
    public bool unlock = false;
    bool saved;

    protected override void Awake()
    {
        base.Awake();
        LoadData();
        saved = false;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Library_1F") // 회의실
        {
            //Debug.Log("SceneManager name");
            if (explainNum != "11")
            {
                isInteract = false;
                //Debug.Log("explainNum " + explainNum);
            }

            ChangeDialogueFileName("Stage2_1F_dialogue");
            if (InventoryUI.instance.HasItem(20102))
            {
                ChangeExplainNum("11");
                if (isInteract && !unlock)
                {
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                    unlock = true;

                    if (!saved)
                    {
                        SaveData();
                        saved = true;
                    }
                }

                if (unlock)
                {
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            else ChangeExplainNum("10");
        }
        else if (SceneManager.GetActiveScene().name == "Library_B1F") // 열람실
        {
            if (InventoryUI.instance.HasItem(20101))
            {
                ChangeExplainNum("2");

                dialogueManager = FindObjectOfType<DialogueManager>();

                if (dialogueManager.isEnd)
                {
                    unlock = true;

                    if (!saved)
                    {
                        SaveData();
                        saved = true;
                    }
                }
            }
        }
    }

    void SaveData()
    {
        npcData.isInteract = isInteract;
        npcData.unlock = unlock;

        string json = JsonUtility.ToJson(npcData);
        File.WriteAllText(filePath, json);
        Debug.Log(gameObject.name + " / 데이터 저장");
    }

    void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            npcData = JsonUtility.FromJson<NPCData>(json);
            Debug.Log(gameObject.name + " / 데이터 로드");

            isInteract = npcData.isInteract;
            unlock = npcData.unlock;
        }
    }
}
