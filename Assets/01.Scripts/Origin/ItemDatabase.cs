using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> itemList = new List<Item>();
    
    void Start()
    {
        itemList.Add(new Item(10001, "Items_10", "A설명", "Itmes_10", Item.ItemType.Use));
        itemList.Add(new Item(10002, "B이름", "B설명", "B이름", Item.ItemType.Use));
        itemList.Add(new Item(20001, "C이름", "C설명", "C이름", Item.ItemType.Equip));
        itemList.Add(new Item(10401, "열쇠꾸러미", "경비원에게 보여주자.", "열쇠꾸러미", Item.ItemType.Use));
        itemList.Add(new Item(10301, "손가락", "이게 뭐지?", "손가락", Item.ItemType.Quest));
    }    
}
