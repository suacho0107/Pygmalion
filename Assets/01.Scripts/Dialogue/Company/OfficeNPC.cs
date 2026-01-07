using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class OfficeNPC : NPC
{
    protected override void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        base.Awake();
        LoadData();
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name.StartsWith("Company_Office-"))
        {
            if (isInteract) ChangeDialogueFile(1);
            else ChangeDialogueFile(0);
        }
    }

    void SaveData()
    {
        npcData.isInteract = isInteract;

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
        }
    }
}
