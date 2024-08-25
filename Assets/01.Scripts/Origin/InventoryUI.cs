using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    private ItemDatabase ItemDB;
    public GameObject inventoryPanel;
    public bool activeInventory = false;

    private InventorySlot[] slots;          // �κ��丮 ���� ����Ʈ

    private List<Item> InventoryItemList;   // �÷��̾� ������ ����Ʈ

    public Text Description_Text;           // �� ����(���� ����X)

    public Transform tf;

    public int selectedItem;

    private bool activeItem;

    public void RemoveSlot()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    }

    public void ShowItem()
    {
        RemoveSlot();
        selectedItem = 0;
        for(int i = 0; i < InventoryItemList.Count; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].AddItem(InventoryItemList[i]);
        }
    }

    public void GetAnItem(int _itemID)
    {
        for(int i = 0; i< ItemDB.itemList.Count;i++)
        {
            if(_itemID == ItemDB.itemList[i].itemID)
            {
                for(int j = 0; j < InventoryItemList.Count; j++)
                {
                    if (InventoryItemList[j].itemID == _itemID)
                    {
                        InventoryItemList[j].itemCount ++;
                        return;
                    }
                }
                InventoryItemList.Add(ItemDB.itemList[i]);
                return;
            }
        }
        Debug.LogError("�����ͺ��̽��� ���� ������");
    }

    void Start()
    {
        instance = this;
        ItemDB = FindObjectOfType<ItemDatabase>();
        inventoryPanel.SetActive(activeInventory);

        InventoryItemList = new List<Item>();
        // GridSlot�� �ڽİ�ü ����
        slots = tf.GetComponentsInChildren<InventorySlot>();

        //InventoryItemList.Add(new Item(10001, "Items_10", "A����", "Itmes_10", Item.ItemType.Use));
    }

    void Update()
    {        
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab");
            activeInventory = !activeInventory;
            if (activeInventory == true)
            {
                inventoryPanel.SetActive(activeInventory);
                activeItem = !activeItem;
                ShowItem();
                selectedItem = 0;
                if (activeItem)
                {
                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        if (selectedItem < InventoryItemList.Count - 2)
                            selectedItem += 2;
                        else
                            selectedItem %= 2;
                    }
                    else if (Input.GetKeyDown(KeyCode.W))
                    {
                        if (selectedItem > 1)
                            selectedItem -= 2;
                        else
                            // ���� �������� �ֻ�ܿ� ���� ��� ���ϴ����� �̵�
                            selectedItem = InventoryItemList.Count - 1 - selectedItem;
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        if (selectedItem < InventoryItemList.Count - 1)
                            selectedItem++;
                        else
                            selectedItem = 0;
                    }
                    else if (Input.GetKeyDown(KeyCode.A))
                    {
                        if (selectedItem > 0)
                            selectedItem--;
                        else
                            selectedItem = InventoryItemList.Count - 1;
                    }
                    else if (Input.GetKeyDown(KeyCode.F))
                    {
                        // ������ ��� ����
                    }
                }
            }
            else
            {
                inventoryPanel.SetActive(false);
                activeItem = false;
            }
        }
    }
}
