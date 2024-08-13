using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItems : MonoBehaviour
{
    public int itemID;
    public int _count;

    private void OnTriggerStay2D(Collider2D collision)
    {
        InventoryUI.instance.GetAnItem(itemID);
        Destroy(this.gameObject);
    }
}
