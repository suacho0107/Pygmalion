using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LibraryReport : MonoBehaviour
{
    NPC report;
    NPCData melData;
    string transPath;

    void Awake()
    {
        report = GetComponent<NPC>();
        transPath = Application.persistentDataPath + "/stage2_statue 2_data.json"; // 멜포메네
        if (File.Exists(transPath))
        {
            string json = File.ReadAllText(transPath);
            melData = JsonUtility.FromJson<NPCData>(json);
        }
    }

    void Update()
    {
        // 멜포메네 처치 전
        if (report.isInteract && (melData == null || !melData.result))
        {
            report.dialogueFileName = "Stage2_1F_BigRoom-1_dialogue";
        }
        // 멜포메네 처치 후
        else if (melData != null && melData.result)
        {
            report.dialogueFileName = "Stage2_1F_BigRoom-2_dialogue";
        }
    }
}
