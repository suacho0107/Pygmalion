using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC))]
public class TutorialDialog : TutorialBase
{
    [SerializeField]
    public string tutorial_csvFileName;

    private NPC             npc;
    private PlayerMove      player;
    private DialogueManager dialogManager;
    
    private bool isDialogueStarted = false;
    public bool isMonologue = false;

    public override void Enter()
    {
        //Debug.Log("Enter: TutorialDialog");

        npc             = GetComponent<NPC>();
        player          = FindObjectOfType<PlayerMove>();
        dialogManager   = FindObjectOfType<DialogueManager>();
        
        npc.LoadNPCData();
        if (!isMonologue && npc.isInteract) return;

        if (player != null)
            player.IsMoved = false;
        dialogManager.isEnd = false;
    }

    public override void Execute(TutorialController controller)
    {
        if (!isMonologue && npc.isInteract)
        {
            controller.SetNextTutorial();
            return;
        }

        // 현재 분기에 진행되는 대사 진행
        if (!isDialogueStarted)
        {
            isDialogueStarted = true;
            npc.StartDialogue();
        }
        if (null == dialogManager)
            return;

        if (dialogManager.isEnd)
        {
            if(npc.dialogueFileName == "Guard1_dialogue")
            {
                npc.isInteract = true;
                npc.SaveNPCData();
            }

            if (null != player)
                player.IsAnimation = true;
            controller.SetNextTutorial();
        }
    }

    public override void Exit()
    {
        if(null != player)
        {
            player.IsMoved = true;
        }
    }
}