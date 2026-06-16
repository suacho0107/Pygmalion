using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactOff : MonoBehaviour
{
    NPC npc;

    private void Awake()
    {
        npc = GetComponent<NPC>();
    }

    private void Update()
    {
        if (npc.isInteract) gameObject.layer = 0;
    }
}
