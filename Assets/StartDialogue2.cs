using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue2 : MonoBehaviour
{
    public NPC npc;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(PlayerPrefs.GetInt("Start2", 0) == 1)
        {
            npc.StartDialogue();
            PlayerPrefs.SetInt("Start2", 0);
            PlayerPrefs.Save();
        }
    }
}
