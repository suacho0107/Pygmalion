using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LibraryStatue5 : MonoBehaviour
{
    Statue statue;
    NPCData melData;
    string transPath;
    
    void Awake()
    {
        statue = GetComponent<Statue>();
        transPath = Application.persistentDataPath + "/stage2_statue 2_data.json"; // ¸áÆ÷¸Þ³×
        if (File.Exists(transPath))
        {
            string json = File.ReadAllText(transPath);
            melData = JsonUtility.FromJson<NPCData>(json);
        }
    }

    void Update()
    {
        if (melData == null) return;
        if (!statue.isJudged && melData.result)
        {
            statue.dialogueFileName = "Library-B1F_Statue5-2_dialogue";
        }
    }
}
