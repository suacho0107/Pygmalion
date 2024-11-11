using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestUI : MonoBehaviour
{
    private NPC npc;

    void Awake()
    {
        npc = GetComponent<NPC>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && npc != null)
        {
            npc.StartDialogue();
        }
    }
}
