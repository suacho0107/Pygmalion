using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Unity.Mathematics;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    private ItemDatabase ItemDB;
    public GameObject inventoryPanel;
    public bool activeInventory = false;

    private InventorySlot[] slots;          // 인벤토리 슬롯 리스트

    private List<Item> inventoryItemList;   // 플레이어 소지템 리스트

    public Text Description_Text;           // 템 설명

    public Image Description_Icon;          // 템 설명창 아이콘

    public Transform tf;

    public int selectedItem;

    private bool activeItem;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    string filePath = "/inventoryItemList.json";

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
        for(int i = 0; i < inventoryItemList.Count; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].AddItem(inventoryItemList[i]);
        }

        SelectedItem();
    }

    public void SelectedItem()
    {
        StopAllCoroutines();
        Color color = slots[0].selected_Item.GetComponent<Image>().color;
        color.a = 0f;
        for (int i = 0; i < inventoryItemList.Count; i++)
            slots[i].selected_Item.GetComponent<Image>().color = color;
        Description_Text.text = inventoryItemList[selectedItem].itemDescription;
        Description_Icon.sprite = inventoryItemList[selectedItem].itemIcon;
        StartCoroutine(SelectedItemEffectCoroutine());
    }

    IEnumerator SelectedItemEffectCoroutine()
    {
        while (activeItem)
        {
            Color color = slots[0].GetComponent<Image>().color;
            while (color.a < 0.5f)
            {
                color.a += 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                slots[selectedItem].selected_Item.GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    public void GetAnItem(int _itemID)
    {
        for(int i = 0; i< ItemDB.itemList.Count;i++)
        {
            if(_itemID == ItemDB.itemList[i].itemID)
            {
                for(int j = 0; j < inventoryItemList.Count; j++)
                {
                    if (inventoryItemList[j].itemID == _itemID)
                    {
                        inventoryItemList[j].itemCount ++;
                        SaveInventory();
                        return;
                    }
                }
                inventoryItemList.Add(ItemDB.itemList[i]);
                SaveInventory();
                Debug.Log("아이템 추가");
                return;
            }
        }
        Debug.LogError("데이터베이스에 없는 아이템");
    }
    public void GetQuestItem(int _itemID)
    {
        GetAnItem(_itemID);

        string message = $"[{gameObject.name}]을(를) 획득했다.";

        DialogueManager dm = FindObjectOfType<DialogueManager>();
        dm.ShowMessage(message);
    }

    public bool HasItem(int _itemID)
    {
        return inventoryItemList.Exists(item => item.itemID == _itemID);
    }

    public void RemoveInventoryItem(int _itemID)
    {
        Item itemToRemove = inventoryItemList.Find(item => item.itemID == _itemID);

        if(itemToRemove != null)
        {
            inventoryItemList.Remove(itemToRemove);
        }
    }

    public void SaveInventory()
    {
        ListJSON.SaveList(inventoryItemList, filePath);
    }

    public void LoadInventory()
    {
        inventoryItemList = ListJSON.LoadList<Item>(filePath);
        if (inventoryItemList == null || inventoryItemList.Count == 0)
        {
            inventoryItemList = new List<Item>();
            DefaultItmes();
            //SaveInventory(); DefaultItems로 이동
        }
        else
        {
            foreach (var item in inventoryItemList)
            {
                item.itemIcon = Resources.Load("ItemIcon/" + item.itemIconName, typeof(Sprite)) as Sprite;
            }
        }
    }

    void DefaultItmes()
    {
        //inventoryItemList.Add(new Item(10001, "Items_10", "A설명", "Itmes_10", Item.ItemType.Use));
        inventoryItemList.Add(new Item(20001, "C이름", "C설명", "C이름", Item.ItemType.Equip));
        inventoryItemList.Add(new Item(10002, "B이름", "B설명", "B이름", Item.ItemType.Use));
        inventoryItemList.Add(new Item(20102, "회의실 열쇠", "회의실 열쇠 설명", "회의실 열쇠", Item.ItemType.Use));
        //inventoryItemList.Add(new Item(20101, "열람실 열쇠", "열람실 열쇠 설명", "열람실 열쇠", Item.ItemType.Use));
        SaveInventory();
        //Debug.Log("DefualtItem");
    }

    void Start()
    {
        instance = this;
        ItemDB = FindObjectOfType<ItemDatabase>();
        inventoryPanel.SetActive(activeInventory);

        inventoryItemList = new List<Item>();
        // GridSlot의 자식객체 저장
        slots = tf.GetComponentsInChildren<InventorySlot>();

        DefaultItmes();
        LoadInventory();
    }

    void Update()
    {        
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab");
            activeInventory = !activeInventory;

            if (activeInventory == true)
            {
                inventoryPanel.SetActive(true);
                activeItem = true;
                ShowItem();
                selectedItem = 0;
            }
            else
            {
                StopAllCoroutines();
                inventoryPanel.SetActive(false);
                activeItem = false;
                SaveInventory();
            }
        }

        if (activeInventory)
        {
            if (activeItem)
            {
                if (inventoryItemList.Count > 0)
                {
                    if (Input.GetKeyDown(KeyCode.S))
                    {

                        if (selectedItem < inventoryItemList.Count - 2)
                            selectedItem += 2;
                        else
                            selectedItem %= 2;
                        SelectedItem();
                    }
                    else if (Input.GetKeyDown(KeyCode.W))
                    {
                        if (selectedItem > 1)
                            selectedItem -= 2;
                        else
                            // 현재 선택템이 최상단에 있을 경우 최하단으로 이동
                            selectedItem = inventoryItemList.Count - 1 - selectedItem;
                        SelectedItem();
                    }
                    else if (Input.GetKeyDown(KeyCode.D))
                    {
                        if (selectedItem < inventoryItemList.Count - 1)
                            selectedItem++;
                        else
                            selectedItem = 0;
                        SelectedItem();
                    }
                    else if (Input.GetKeyDown(KeyCode.A))
                    {
                        if (selectedItem > 0)
                            selectedItem--;
                        else
                            selectedItem = inventoryItemList.Count - 1;
                        SelectedItem();
                    }
                    else if (Input.GetKeyDown(KeyCode.F))
                    {
                        // 아이템 사용 여부
                    }
                }
            }
        }
    }
}
