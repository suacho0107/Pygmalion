using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [SerializeField] private DoorState  doorState;
    [SerializeField] private StageNPC   stageNPC;
    [SerializeField] private DialogueUI dialogueUI;

    Animator animator;

    private void Awake()
    {
        if (gameObject != null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void OnEnable()
    {
        // 이벤트 등록
        if (dialogueUI != null)
            dialogueUI.OnDialogueEnded += HandleDialogueEnded;
    }

    private void OnDisable()
    {
        // 이벤트 제거
        if (dialogueUI != null)
            dialogueUI.OnDialogueEnded -= HandleDialogueEnded;
    }

    // DialogueUI에서 이벤트 호출과 함께 NPC 전달
    void HandleDialogueEnded(NPC endedNPC)
    {
        if (!stageNPC.isInteract)
            return;

        animator.SetTrigger("Open");
    }

    public void DoorOff()
    {
        doorState.isDoorDestroyed = true;
        gameObject.SetActive(false);
    }
}
