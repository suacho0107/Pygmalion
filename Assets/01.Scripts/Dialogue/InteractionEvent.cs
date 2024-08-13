using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] DialogueEvent dialogue;

    public void LoadDialogue(string _csvFileName)
    {
        DialogueParser dialogueParser = FindObjectOfType<DialogueParser>();
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueParser != null)
        {
            Dialogue[] dialogues = dialogueParser.Parse(_csvFileName);
            dialogue.dialogues = dialogues; //파싱된 대화 데이터를 DialogueEvent에 할당

            if (dialogueManager != null)
            {
                dialogueManager.ShowDialogue(dialogues);
            }
        }
    }
}
