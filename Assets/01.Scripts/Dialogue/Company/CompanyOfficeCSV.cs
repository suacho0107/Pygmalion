using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompanyOfficeCSV : MonoBehaviour
{
    public NPC[] npcs;  // 여러 NPC 스크립트를 배열로 받음

    private void Start()
    {
        if (npcs.Length > 0)
        {
            if (SceneManager.GetActiveScene().name == "Company_Lobby-1")
            {
                npcs[0].dialogueFiles = new string[] { "Office-2-2_dialogue" };
                npcs[0].selectFiles = new string[] { "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
            }
            else if (SceneManager.GetActiveScene().name == "Company_Office-1")
            {
                npcs[0].dialogueFiles = new string[] { "request1_dialogue" };
                npcs[0].selectFiles = new string[] { "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
            }
            else if(SceneManager.GetActiveScene().name == "Company_Lobby-")
            {

            }
        }
    }
}
