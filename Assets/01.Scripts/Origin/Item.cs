using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

[System.Serializable]
public class Item
{
    public int itemID;
    public string itemName;
    public string itemDescription;
    public string itemIconName;
    public int itemCount;
    public Sprite itemIcon;
    public ItemType itemType;

    public enum ItemType
    {
        Use,    // 소모품
        Equip,  // 장비
        Quest,  // 퀘스트템
        Etc     // 기타
    }

    // 생성자
    public Item(int _itemID, string _itemName, string _itemDes, string _itemIconName, ItemType _itemType, int _itemCount = 1)
    {
        itemID = _itemID;
        itemName = _itemName;
        itemDescription = _itemDes;
        itemIconName = _itemIconName;
        itemType = _itemType;
        itemCount = _itemCount;
        itemIcon = Resources.Load("ItemIcon/" + _itemName, typeof(Sprite)) as Sprite;
    }
}
