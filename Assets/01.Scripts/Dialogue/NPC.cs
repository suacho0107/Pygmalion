using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public InteractionEvent interactionEvent; // �� NPC�� ����� InteractionEvent

    [SerializeField] private string dialogueFileName;
    [SerializeField] public string selectFileName;
    [SerializeField] private string[] dialogueFiles; // ���� ���� �迭 �߰�
    [SerializeField] private string[] selectFiles;
    int currentIndex = 0;

    public bool isChecked = false;

    //private void OnMouseDown()
    //{
    //    StartDialogue();
    //}

    private void Start()
    {
        dialogueFiles = new string[] { "Tutorial1_dialogue", "Tutorial2_dialogue", "Check1_dialogue", "Check2_dialogue", "Check3_dialogue" };
        selectFiles = new string[] { "Tutorial1_select", "", "", "", ""};
        dialogueFileName = dialogueFiles[currentIndex];
        selectFileName = selectFiles[currentIndex];
    }

    private void Update()
    {
        if (isChecked)
        {
            ChangeDialogueFile();
        }
    }

    public void StartDialogue()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.SetNPC(this);
        }
        else
        {
            Debug.LogError("DialogueManager is null.");
        }

        InteractionEvent interactionEvent = GetComponent<InteractionEvent>();
        if (interactionEvent != null)
        {
            interactionEvent.LoadDialogue(dialogueFileName);
        }
    }

    void ChangeDialogueFile()
    {
        if(currentIndex < dialogueFiles.Length - 1)
        {
            currentIndex++;
            dialogueFileName = dialogueFiles[currentIndex];
            selectFileName = selectFiles[currentIndex];
            isChecked = false;
            Debug.Log("��ȭ ���� ����: " + dialogueFileName);
        }
    }
}
