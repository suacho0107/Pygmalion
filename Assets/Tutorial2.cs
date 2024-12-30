using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial2 : MonoBehaviour
{
    public NPC npc;

    bool isEndTutorial2 = false;

    private void Start()
    {
        Invoke("StartTutorial2", 1f);
    }

    void StartTutorial2()
    {
        if (!isEndTutorial2)
        {
            if (PlayerPrefs.GetInt("Start2", 0) == 1)
            {
                npc.StartDialogue();
                PlayerPrefs.SetInt("Start2", 0);
                PlayerPrefs.Save();
            }
        }

        isEndTutorial2 = true;
    }
}
