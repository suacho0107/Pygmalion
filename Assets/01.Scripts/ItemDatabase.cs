using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> itemList = new List<Item>();
    
    void Start()
    {
        itemList.Add(new Item(10001, "Items_10", "A����", "Itmes_10", Item.ItemType.Use));
        itemList.Add(new Item(10002, "B�̸�", "B����", "B�̸�", Item.ItemType.Use));
        itemList.Add(new Item(20001, "C�̸�", "C����", "C�̸�", Item.ItemType.Equip));
    }    
}
