using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MuseumLobbyCSV : MonoBehaviour
{
    public NPC[] npcs;  // 여러 NPC 스크립트를 배열로 받음

    private void Start()
    {
        if (npcs.Length > 0)
        {
            if(SceneManager.GetActiveScene().name == "Museum_Lobby")
            {
                // 조각상 판별 csv(테스트)
                npcs[0].dialogueFiles = new string[] { "stage1_lobby_dialogue", "Stage1LobbyStatue1_dialogue", "Stage1LobbyStatue1_dialogue", "battle1_dialogue", "Destroyed_dialogue" };
                npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

                // 미술관장
                npcs[1].dialogueFiles = new string[] { "Tutorial1_dialogue", "Tutorial2_dialogue", "Check1_dialogue", "Check2_dialogue", "Check3_dialogue" };
                npcs[1].selectFiles = new string[] { "Tutorial1_select", "", "", "", "" };
                npcs[1].currentIndex = 0;
                npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
                npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];
            }
            else if(SceneManager.GetActiveScene().name == "Museum_ExhibitionRoom1")
            {
                // 조각상 판별 csv(테스트)
                npcs[0].dialogueFiles = new string[] { "stage1_exhibit1_dialogue", "Stage1Exhibit1Statue2_dialogue", "Stage1Exhibit1Statue2_dialogue", "battleDialogue1" , "Destroyed_dialogue"};
                npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
            }
            else if(SceneManager.GetActiveScene().name == "Museum_ExhibitionRoom2")
            {
                // 조각상 판별 csv(테스트)
                npcs[0].dialogueFiles = new string[] { "Stage1Exhibit2Statue3_dialogue", "Stage1Exhibit2Statue3_dialogue", "battleDialogue1", "Destroyed_dialogue" };
                npcs[0].selectFiles = new string[] { "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

                // 조각상 판별 csv(테스트)
                npcs[1].dialogueFiles = new string[] { "Stage1Exhibit2Statue4_dialogue", "Stage1Exhibit2Statue4_dialogue", "battleDialogue1", "Destroyed_dialogue" };
                npcs[1].selectFiles = new string[] { "judge1_select", "judge2_select", "", "" };
                npcs[1].currentIndex = 0;
                npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
                npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];

                npcs[2].dialogueFiles = new string[] { "Stage1Exhibit2Statue3_dialogue", "Stage1Exhibit2Statue3_dialogue", "battleDialogue1", "Destroyed_dialogue" };
                npcs[2].selectFiles = new string[] { "judge1_select", "judge2_select", "", "" };
                npcs[2].currentIndex = 0;
                npcs[2].dialogueFileName = npcs[2].dialogueFiles[npcs[2].currentIndex];
                npcs[2].selectFileName = npcs[2].selectFiles[npcs[2].currentIndex];

                npcs[3].dialogueFiles = new string[] { "judge1_dialogue", "judge1_dialogue", "battleDialogue1", "Destroyed_dialogue" };
                npcs[3].selectFiles = new string[] { "judge1_select", "judge2_select", "", "" };
                npcs[3].currentIndex = 0;
                npcs[3].dialogueFileName = npcs[3].dialogueFiles[npcs[3].currentIndex];
                npcs[3].selectFileName = npcs[3].selectFiles[npcs[3].currentIndex];

                npcs[4].dialogueFiles = new string[] { "judge1_dialogue", "judge1_dialogue", "battleDialogue1" , "Destroyed_dialogue" };
                npcs[4].selectFiles = new string[] { "judge1_select", "judge2_select", "", "" };
                npcs[4].currentIndex = 0;
                npcs[4].dialogueFileName = npcs[4].dialogueFiles[npcs[4].currentIndex];
                npcs[4].selectFileName = npcs[4].selectFiles[npcs[4].currentIndex];

                npcs[5].dialogueFiles = new string[] { "judge1_dialogue", "judge1_dialogue", "battleDialogue1" , "Destroyed_dialogue" };
                npcs[5].selectFiles = new string[] { "judge1_select", "judge2_select", "", "" };
                npcs[5].currentIndex = 0;
                npcs[5].dialogueFileName = npcs[5].dialogueFiles[npcs[5].currentIndex];
                npcs[5].selectFileName = npcs[5].selectFiles[npcs[5].currentIndex];
            }
            else if (SceneManager.GetActiveScene().name == "Museum_ExhibitionRoom3")
            {
                // 조각상 판별 csv(테스트)
                npcs[0].dialogueFiles = new string[] { "Stage1Exhibit3Statue6_dialogue", "Stage1Exhibit3Statue6_dialogue", "battleDialogue1" , "Destroyed_dialogue" };
                npcs[0].selectFiles = new string[] { "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
            }
            else if (SceneManager.GetActiveScene().name == "Museum_Garden")
            {
                // 조각상 판별 csv(테스트)
                npcs[0].dialogueFiles = new string[] { "Stage1GardenStatue5_dialogue", "Stage1GardenStatue5_dialogue", "battleDialogue1" , "Destroyed_dialogue" };
                npcs[0].selectFiles = new string[] { "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
            }
        }
    }
}
