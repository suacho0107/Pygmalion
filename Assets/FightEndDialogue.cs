using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightEndDialogue : MonoBehaviour
{
    public NPC npc;
    //public MuseumLobbyCSV csv;

    void Start()
    {
        if(PlayerPrefs.GetInt("PlayerLose", 0) == 1)
        {
            PlayerPrefs.SetInt("PlayerLose", 0);
            npc.dialogueFileName = "FightEnd_dialogue";
            npc.explainNum = "1";
            npc.StartDialogue();
        }
        else if (PlayerPrefs.GetInt("PlayerRun", 0) == 1)
        {
            PlayerPrefs.SetInt("PlayerRun", 0);
            npc.dialogueFileName = "FightEnd_dialogue";
            npc.explainNum = "2";
            npc.StartDialogue();
        }
    }
}
