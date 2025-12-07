using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MuseumLobbyCSV : MonoBehaviour
{
    public NPC[] npcs;  // 여러 NPC 스크립트를 배열로 받음

    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (npcs.Length > 0)
        {
            if (sceneName.StartsWith("Company"))
            {
                if (sceneName.StartsWith("Company_Lobby"))
                {
                    npcs[0].dialogueFiles = new string[] { "Office-1-2_dialogue" };
                    npcs[0].selectFiles = new string[] { "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
                }
                else if (sceneName.StartsWith("Company_Office"))
                {
                    npcs[0].dialogueFiles = new string[] { "Office-1-2_dialogue" };
                    npcs[0].selectFiles = new string[] { "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
                }
                else if (sceneName.StartsWith("Company_TutorialOffice"))
                {
                    npcs[0].dialogueFiles = new string[] { "Office-2-1_dialogue" };
                    npcs[0].selectFiles = new string[] { "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
                }
            }
            else if (sceneName.StartsWith("Museum"))
            {
                if (sceneName.StartsWith("Museum_Lobby"))
                {
                    npcs[0].dialogueFiles = new string[] { "stage1_lobby_dialogue", "Stage1LobbyStatue1_dialogue", "Stage1LobbyStatue1_dialogue", "battle1_dialogue", "Destroyed_dialogue" };
                    npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

                    npcs[1].dialogueFiles = new string[] { "Tutorial1_dialogue", "Tutorial2_dialogue", "Check1_dialogue", "Check2_dialogue", "Check3_dialogue" };
                    npcs[1].selectFiles = new string[] { "Tutorial1_select", "", "", "", "" };
                    ////Test
                    //npcs[1].dialogueFiles = new string[] { "2Select_Test_Dialogue_Jiyun", "Tutorial2_dialogue", "Check1_dialogue", "Check2_dialogue", "Check3_dialogue" };
                    //npcs[1].selectFiles = new string[] { "2Select_Test_Select_Jiyun", "", "", "", "" };
                    npcs[1].currentIndex = 0;
                    npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
                    npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];
                }
                else if (sceneName.StartsWith("Museum_ExhibitionRoom1"))
                {
                    npcs[0].dialogueFiles = new string[] { "stage1_exhibit1_dialogue", "Stage1Exhibit1Statue2_dialogue", "Stage1Exhibit1Statue2_dialogue", "battle2_dialogue", "Destroyed_dialogue" };
                    npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
                }
                else if (sceneName.StartsWith("Museum_ExhibitionRoom2"))
                {
                    npcs[0].dialogueFiles = new string[] { "stage1_exhibit2_dialogue", "Stage1Exhibit2Statue3_dialogue", "Stage1Exhibit2Statue3_dialogue", "battle3_dialogue", "Destroyed_dialogue", "stage1_exhibit2_Item" };
                    npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "", "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

                    npcs[1].dialogueFiles = new string[] { "stage1_exhibit2_dialogue", "Stage1Exhibit2Statue4_dialogue", "Stage1Exhibit2Statue4_dialogue", "battle4_dialogue", "Destroyed_dialogue" };
                    npcs[1].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                    npcs[1].currentIndex = 0;
                    npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
                    npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];
                }
                else if (sceneName.StartsWith("Museum_ExhibitionRoom3"))
                {
                    npcs[0].dialogueFiles = new string[] { "stage1_exhibit3_dialogue", "Stage1Exhibit3Statue6_dialogue", "Stage1Exhibit3Statue6_dialogue", "battle6_dialogue", "Destroyed_dialogue" };
                    npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
                }
                else if (sceneName.StartsWith("Museum_Garden"))
                {
                    npcs[0].dialogueFiles = new string[] { "Stage1GardenStatue5_dialogue", "Stage1GardenStatue5_dialogue", "Stage1GardenStatue5_dialogue", "battle5_dialogue", "Destroyed_dialogue" };
                    npcs[0].selectFiles = new string[] { "", "judge1_select", "judge2_select", "", "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

                    npcs[1].dialogueFiles = new string[] { "Museum-Guard0_dialogue", "Museum-Guard1a_dialogue", "Museum-Guard1b_dialogue", "Museum-Guard1c_dialogue", "Museum-Guard1d_dialogue", "Museum-Guard2_dialogue" };
                    npcs[1].selectFiles = new string[] { "Museum-Guard0_select", "", "", "", "Museum-Guard1d_select", "" };
                    npcs[1].currentIndex = 0;
                    npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
                    npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];
                }
            }
            else if (sceneName.StartsWith("Library"))
            {
                if (sceneName.StartsWith("Library_1F"))
                {
                    npcs[0].dialogueFiles = new string[] { "Library_Statue1_dialogue", "Library_Statue1_dialogue", "Stage2_battle1_dialogue", "Destroyed_dialogue" };
                    npcs[0].selectFiles = new string[] { "judge1_select", "judge2_select", "", "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

                    npcs[1].dialogueFiles = new string[] { "Guard1_dialogue", "Guard_Check0_dialogue", "Guard_Check1_dialogue", "Guard_Check2_dialogue", "Guard_Check3_dialogue" };
                    npcs[1].selectFiles = new string[] { "Guard1_select", "", "", "", "" };
                    npcs[1].currentIndex = 0;
                    npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
                    npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];
                }
                else if (sceneName.StartsWith("Library_2F"))
                {
                    npcs[0].dialogueFiles = new string[] { "Stage2_Library2F_Statue2_dialogue", "Stage2_Library2F_Statue2_dialogue", "Stage2_Library2F_Statue2_dialogue", "Stage2_battle2_dialogue", "Destroyed_dialogue" };
                    npcs[0].selectFiles = new string[] { "judge1_select", "judge2_select", "judge2_select", "", "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

                    npcs[1].dialogueFiles = new string[] { "Stage2_Library2F_Statue3_dialogue", "Stage2_Library2F_Statue3_dialogue", "Stage2_Library2F_Statue3_dialogue", "Stage2_battle3_dialogue", "Destroyed_dialogue" };
                    npcs[1].selectFiles = new string[] { "judge1_select", "judge2_select", "judge2_select", "", "" };
                    npcs[1].currentIndex = 0;
                    npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
                    npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];

                    npcs[2].dialogueFiles = new string[] { "Stage2_Library2F_Statue4_dialogue", "Stage2_Library2F_Statue4_dialogue", "Stage2_Library2F_Statue4_dialogue", "Stage2_battle4_dialogue", "Destroyed_dialogue" };
                    npcs[2].selectFiles = new string[] { "judge1_select", "judge2_select", "judge2_select", "", "" };
                    npcs[2].currentIndex = 0;
                    npcs[2].dialogueFileName = npcs[2].dialogueFiles[npcs[2].currentIndex];
                    npcs[2].selectFileName = npcs[2].selectFiles[npcs[2].currentIndex];

                    npcs[3].dialogueFiles = new string[] { "Library-Librarian0_dialogue", "Library-Librarian1_dialogue" };
                    npcs[3].selectFiles = new string[] { "Library-Librarian0_select", "" };
                    npcs[3].currentIndex = 0;
                    npcs[3].dialogueFileName = npcs[3].dialogueFiles[npcs[3].currentIndex];
                    npcs[3].selectFileName = npcs[3].selectFiles[npcs[3].currentIndex];
                }
                else if (sceneName.StartsWith("Library_B1F"))
                {
                    npcs[0].dialogueFiles = new string[] { "Stage2_LibraryB1F_Statue5_dialogue", "Stage2_LibraryB1F_Statue5_dialogue", "Stage2_LibraryB1F_Statue5_dialogue", "Stage2_battle5_dialogue", "Destroyed_dialogue" };
                    npcs[0].selectFiles = new string[] { "judge1_select", "judge2_select", "judge2_select", "", "" };
                    npcs[0].currentIndex = 0;
                    npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
                    npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];

                    //npcs[1].dialogueFiles = new string[] { "kiosk_dialogue" };
                    //npcs[1].selectFiles = new string[] { "kiosk_select" };
                    //npcs[1].currentIndex = 0;
                    //npcs[1].dialogueFileName = npcs[1].dialogueFiles[npcs[1].currentIndex];
                    //npcs[1].selectFileName = npcs[1].selectFiles[npcs[1].currentIndex];
                }
            }
            else if (sceneName.StartsWith(""))
            {

            }
        }
    }
}
