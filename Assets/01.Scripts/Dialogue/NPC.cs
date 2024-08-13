using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private string csvFileName;

    public void StartDialogue()
    {
        InteractionEvent interactionEvent = GetComponent<InteractionEvent>();
        if (interactionEvent != null)
        {
            interactionEvent.LoadDialogue(csvFileName);
        }
    }
}
