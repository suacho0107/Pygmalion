using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightEndDialogue : MonoBehaviour
{
    //public NPC npc;

    AudioSource RetrySound;
    //public MuseumLobbyCSV csv;

    void Start()
    {
        RetrySound = GetComponent<AudioSource>();
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        //ㄴ이거 원래if문 안에 dialogueUI.ShowMessage(message); 윗줄에 있었는데 여기로 옮겨도 문제가 없을까요? if문 안에서 돌아가야 하는 이유가 있나...?

        if (PlayerPrefs.GetInt("PlayerLose", 0) == 1)
        {
            StartCoroutine(ResetLoseSignal());
            RetrySound.Play();
            //npc.dialogueFileName = "FightEnd_dialogue";
            //npc.explainNum = "1";
            //npc.StartDialogue();

            string message = "얼른 끝내고 퇴근해야지...";
            
            dialogueUI.ShowMessage(message);
        }
        else if (PlayerPrefs.GetInt("PlayerRun", 0) == 1)
        {
            StartCoroutine(ResetRunSignal());
            //npc.dialogueFileName = "FightEnd_dialogue";
            //npc.explainNum = "2";
            //npc.StartDialogue();

            string message = "잠깐, 숨 좀 돌리고...";

            dialogueUI.ShowMessage(message);
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
