
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionEvent : MonoBehaviour
{
    [SerializeField] DialogueEvent dialogue;
    [SerializeField] SelectEvent select;

    public Dialogue[] dialogues { get; private set; }
    public int lineCount { get; private set; }


    public SelectEvent Select
    {
        get { return select; }
    }

    public void LoadDialogue(string _csvFileName, string explainNum = null)
    {
        DialogueParser dialogueParser = FindObjectOfType<DialogueParser>();
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        TutorialDialog tutorialDialogParser = GetComponent<TutorialDialog>();

        if (dialogueParser != null)
        {
            if (tutorialDialogParser != null)
            {
                _csvFileName = tutorialDialogParser.tutorial_csvFileName;
            }

            dialogues = dialogueParser.Parse(_csvFileName);
            dialogue.dialogues = dialogues; //파싱된 대화 데이터를 DialogueEvent에 할당
            lineCount = 0;

            if (dialogueManager != null)
            {
                if (!string.IsNullOrEmpty(explainNum))//explainNum 있으면 전달
                {
                    dialogueManager.ShowDialogue(dialogues, explainNum);
                }
                else //explainNum 없으면 그냥
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

    public void AdvanceDialogue()
    {
        if (lineCount <= dialogues.Length)
        {
            lineCount++;
            Debug.Log($"현재 대사 진행 상태: {lineCount} / {dialogues.Length}");
        }
    }
}
