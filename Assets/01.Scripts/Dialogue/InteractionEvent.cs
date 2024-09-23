using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] DialogueEvent dialogue;
    [SerializeField] SelectEvent select;

    public SelectEvent Select
    {
        get { return select; }
    }

    public void LoadDialogue(string _csvFileName, string explainNum = null)
    {
        DialogueParser dialogueParser = FindObjectOfType<DialogueParser>();
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueParser != null)
        {
            Dialogue[] dialogues = dialogueParser.Parse(_csvFileName);
            dialogue.dialogues = dialogues; //�Ľ̵� ��ȭ �����͸� DialogueEvent�� �Ҵ�

            if (dialogueManager != null)
            {
                if (!string.IsNullOrEmpty(explainNum))//explainNum ������ ����
                {
                    dialogueManager.ShowDialogue(dialogues, explainNum);
                }
                else //explainNum ������ �׳�
                {
                    dialogueManager.ShowDialogue(dialogues);
                }
            }
        }
    }

    public void LoadSelect(string _csvFileName)
    {
        SelectParser selectParser = FindObjectOfType<SelectParser>();

        if (selectParser != null)
        {
            Select[] selects = selectParser.Parse(_csvFileName);

            select.selects = selects;
        }
    }
}
