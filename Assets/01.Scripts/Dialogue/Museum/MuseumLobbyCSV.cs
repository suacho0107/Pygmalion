using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumLobbyCSV : MonoBehaviour
{
    public NPC[] npcs;  // 여러 NPC 스크립트를 배열로 받음

    private void Start()
    {
        if (npcs.Length > 0)
        {
            // 미술관장
            npcs[0].dialogueFiles = new string[] { "Tutorial1_dialogue", "Tutorial2_dialogue", "Check1_dialogue", "Check2_dialogue", "Check3_dialogue" };
            npcs[0].selectFiles = new string[] { "Tutorial1_select", "", "", "", "" };
            npcs[0].currentIndex = 0;
            npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
            npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

            // 조각상
            npcs[1].dialogueFiles = new string[] { "stage1_lobby_dialogue" };
            npcs[1].selectFiles = new string[] { "" };
            npcs[1].currentIndex = 0;
            npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
            npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];
        }
    }
}
