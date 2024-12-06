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
            if (SceneManager.GetActiveScene().name == "Museum_Lobby")
            {
                npcs[0].dialogueFiles = new string[] { "stage1_lobby_dialogue", "Stage1LobbyStatue1_dialogue", "Stage1LobbyStatue1_dialogue", "battle1_dialogue", "Destroyed_dialogue" };
                npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

                npcs[1].dialogueFiles = new string[] { "Tutorial1_dialogue", "Tutorial2_dialogue", "Check1_dialogue", "Check2_dialogue", "Check3_dialogue" };
                npcs[1].selectFiles = new string[] { "Tutorial1_select", "", "", "", "" };
                npcs[1].currentIndex = 0;
                npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
                npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];
            }
            else if (SceneManager.GetActiveScene().name == "Museum_ExhibitionRoom1")
            {
                npcs[0].dialogueFiles = new string[] { "stage1_exhibit1_dialogue", "Stage1Exhibit1Statue2_dialogue", "Stage1Exhibit1Statue2_dialogue", "battle2_dialogue", "Destroyed_dialogue" };
                npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
            }
            else if (SceneManager.GetActiveScene().name == "Museum_ExhibitionRoom2")
            {
                npcs[0].dialogueFiles = new string[] { "stage1_exhibit2_dialogue", "Stage1Exhibit2Statue3_dialogue", "Stage1Exhibit2Statue3_dialogue", "battle3_dialogue", "Destroyed_dialogue" };
                npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

                npcs[1].dialogueFiles = new string[] { "stage1_exhibit2_dialogue", "Stage1Exhibit2Statue4_dialogue", "Stage1Exhibit2Statue4_dialogue", "battle4_dialogue", "Destroyed_dialogue" };
                npcs[1].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                npcs[1].currentIndex = 0;
                npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
                npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];
            }
            else if (SceneManager.GetActiveScene().name == "Museum_ExhibitionRoom3")
            {
                npcs[0].dialogueFiles = new string[] { "stage1_exhibit3_dialogue", "Stage1Exhibit3Statue6_dialogue", "Stage1Exhibit3Statue6_dialogue", "battle6_dialogue", "Destroyed_dialogue" };
                npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
            }
            else if (SceneManager.GetActiveScene().name == "Museum_Garden")
            {
                npcs[0].dialogueFiles = new string[] { "stage1_garden_dialogue", "Stage1GardenStatue5_dialogue", "Stage1GardenStatue5_dialogue", "battle5_dialogue", "Destroyed_dialogue" };
                npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
            }
            else if (SceneManager.GetActiveScene().name == "Library_1F")
            {
                npcs[0].dialogueFiles = new string[] { "stage1_garden_dialogue", "Stage1GardenStatue5_dialogue", "Stage1GardenStatue5_dialogue", "battle5_dialogue", "Destroyed_dialogue" };
                npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                npcs[0].currentIndex = 0;
                npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
            }
        }
    }
}
