
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
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
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

            if (dialogueUI != null)
            {
                if (!string.IsNullOrEmpty(explainNum))//explainNum 있으면 전달
                {
                    dialogueUI.ShowDialogue(dialogues, explainNum);
                }
                else //explainNum 없으면 그냥
                {
                    dialogueUI.ShowDialogue(dialogues);
                }
            }
        }
    }

    public void LoadSelect(string _csvFileName)
    {
        SelectParser selectParser = FindObjectOfType<SelectParser>();

        //Guard
        if (selectParser == null)
        {
            Debug.LogError("SelectParser is Null.");
            return;
        }

        Select[] parsedSelects = selectParser.Parse(_csvFileName);

        //Guard
        if (parsedSelects == null)
        {
            Debug.LogError($"SelectParser.Parse({_csvFileName}) returned NULL.");
            return;
        }
        if (parsedSelects.Length == 0)
        {
            Debug.LogError($"SelectParser.Parse({_csvFileName}) returned an EMPTY array.");
            return;
        }
        if (select == null)
        {
            Debug.LogError("interactionEvent.Select is Null.");
            return;
        }

        select.selects = parsedSelects;

        //Debug.Log($"LoadSelect 성공: {_csvFileName}, 선택지 개수 = {parsedSelects.Length}");
    }

    public void AdvanceDialogue()
    {
        if (lineCount <= dialogues.Length)
        {
            lineCount++;
            //Debug.Log($"현재 대사 진행 상태: {lineCount} / {dialogues.Length}");
        }
    }
}
