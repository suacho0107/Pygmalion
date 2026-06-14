using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.Progress;

public class FieldItems : MonoBehaviour
{
    public int itemID;
    public int _count;
    [SerializeField] NPC npc;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryUI.instance.GetAnItem(itemID);
            FieldItemManager.Instance.CollectedItem(itemID, transform.position);

            if (SceneManager.GetActiveScene().name == "Museum_Garden")
            {
                MuseumKey();
                Destroy(this.gameObject);
                return;
            }
            string message = $"[{gameObject.name}]¿ª(∏¶) »πµÊ«ﬂ¥Ÿ.";

            DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
            dialogueUI.ShowMessage(message);

            Destroy(this.gameObject);
        }
    }

    void MuseumKey()
    {
        MuseumGuard mGuard = FindObjectOfType<MuseumGuard>();
        if (mGuard.questStart)
        {
            npc.ChangeDialogueFileName("Museum-GuardItemT_dialogue");
            npc.StartDialogue();
        }
        else
        {
            npc.ChangeDialogueFileName("Museum-GuardItemF_dialogue");
            npc.StartDialogue();
        }
    }

    private void Start()
    {
        if (null == FieldItemManager.Instance)
            return;

        if (FieldItemManager.Instance.IsCollected(itemID))
        {
            Destroy(gameObject);
        }
        else
        {
            return;
        }

        npc = GetComponent<NPC>();
    }
}
