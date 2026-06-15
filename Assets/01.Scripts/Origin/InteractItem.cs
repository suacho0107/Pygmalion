using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractItem : MonoBehaviour
{
    //[SerializeField] Statue statue;
    //DialogueManager dialogueManager;
    //bool currentNPC => dialogueManager != null && dialogueManager.CurrentNPC == this;

    //private void Awake()
    //{
    //    statue = GetComponent<Statue>();
    //    dialogueManager = FindObjectOfType<DialogueManager>();
    //}

    //private void Update()
    //{
    //    if (!statue.isInteract)
    //    {
    //        if (currentNPC && dialogueManager.isEnd)
    //        {
    //            GetInteractItem(10403);
    //            dialogueManager.isEnd = false;
    //        }
    //    }
    //}

    public void GetInteractItem(int _itemID)
    {
        InventoryUI.instance.GetAnItem(_itemID);
    }
}
