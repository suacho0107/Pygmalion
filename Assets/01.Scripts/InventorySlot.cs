using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Text itemName_Text;
    public Text itemCount_Text;
    public GameObject selected_Item;

    public void AddItem(Item _item)
    {
        itemName_Text.text = _item.itemName;
        icon.sprite = _item.itemIcon;

        // 소모품인 경우
        if(Item.ItemType.Use == _item.itemType)
        {
            if (_item.itemCount > 0)            
                itemCount_Text.text = "x " + _item.itemCount.ToString();       
            else
                itemCount_Text.text = "";
        }
    }

    public void RemoveItem()
    {
        itemName_Text.text = "";
        itemName_Text.text = "";
        icon.sprite = null;
    }
}
