using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class FieldItems : MonoBehaviour
{
    public int itemID;
    public int _count;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryUI.instance.GetAnItem(itemID);
            FieldItemManager.Instance.CollectedItem(itemID, transform.position);
            Destroy(this.gameObject);

            string message = $"[{gameObject.name}]¿ª(∏¶) »πµÊ«ﬂ¥Ÿ.";

            //DialogueManager dm = FindObjectOfType<DialogueManager>();
            //dm.ShowMessage(message);
            DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
            dialogueUI.ShowMessage(message);
        }
    }

    private void Start()
    {
        if (FieldItemManager.Instance.IsCollected(itemID))
        {
            Destroy(gameObject);
        }
        else
        {
            return;
        }
    }
}
