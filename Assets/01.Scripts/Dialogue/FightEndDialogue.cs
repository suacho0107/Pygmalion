using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightEndDialogue : MonoBehaviour
{
    //public NPC npc;

    AudioSource RetrySound;
    //public MuseumLobbyCSV csv;
    string sceneName;

    void Start()
    {
        RetrySound = GetComponent<AudioSource>();
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        sceneName = SceneManager.GetActiveScene().name;

        if (PlayerPrefs.GetInt("PlayerLose", 0) == 1)
        {
            StartCoroutine(ResetLoseSignal());
            RetrySound.Play();
            //npc.dialogueFileName = "FightEnd_dialogue";
            //npc.explainNum = "1";
            //npc.StartDialogue();

            if (sceneName.StartsWith("Museum"))
            {
                string message = "얼른 끝내고 퇴근해야지...";

                dialogueUI.ShowMessage(message);
            }
        }
        else if (PlayerPrefs.GetInt("PlayerRun", 0) == 1)
        {
            StartCoroutine(ResetRunSignal());
            //npc.dialogueFileName = "FightEnd_dialogue";
            //npc.explainNum = "2";
            //npc.StartDialogue();

            if (sceneName.StartsWith("Museum"))
            {
                string message = "잠깐, 숨 좀 돌리고...";

                dialogueUI.ShowMessage(message);
            }
        }
    }

    IEnumerator ResetLoseSignal()
    {
        yield return new WaitForEndOfFrame();
        PlayerPrefs.SetInt("PlayerLose", 0);
        PlayerPrefs.Save();
    }

    IEnumerator ResetRunSignal()
    {
        yield return new WaitForEndOfFrame();
        PlayerPrefs.SetInt("PlayerRun", 0);
        PlayerPrefs.Save();
    }
}
