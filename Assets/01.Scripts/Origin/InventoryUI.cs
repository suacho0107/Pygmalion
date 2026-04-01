using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using Unity.Mathematics;

// End 시점에 결과보고서 등급에 따라 성과급 부여

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI instance;
    private ItemDatabase ItemDB;
    public GameObject inventoryPanel;
    public GameObject currencyPanel;
    public GameObject infoPanel;
    public GameObject inventoryBackground;

    public bool activeInventory = false;
    public bool battleInventory;

    private InventorySlot[] slots;          // 인벤토리 슬롯 리스트

    private List<Item> inventoryItemList;   // 플레이어 소지템 리스트
    private List<Item> battleItemList;      // 전투 씬 아이템 리스트

    private List<Item> CurrentItemList()
    {
        return battleInventory ? battleItemList : inventoryItemList;
    }

    public Text DescriptionName_text;       // 설명창 템 이름
    public Text Description_Text;           // 템 설명
    public Text Currency_Text;

    public Image Description_Icon;          // 템 설명창 아이콘

    public Transform tf;

    public int selectedItem;

    public bool activeItem;                 // 아이템 선택 강조 코루틴 타이밍

    public bool activeSelect;               // 템 선택 가능 타이밍

    public bool keyUp = false;              // 스페이스바 입력 조절(activeSelect)

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);

    string filePath = "/inventoryItemList.json";

    void LoadBattleItemList()
    {
        if (battleItemList == null)
            battleItemList = new List<Item>();
        else
            battleItemList.Clear();

        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (inventoryItemList[i].itemType == Item.ItemType.Battle)
            {
                battleItemList.Add(inventoryItemList[i]);
            }
        }
    }

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

        if (battleInventory)
        {
            LoadBattleItemList();
        }

        List<Item> currentList = CurrentItemList();

        for (int i = 0; i < currentList.Count; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].AddItem(currentList[i]);
        }

        //for(int i = 0; i < inventoryItemList.Count; i++)
        //{
        //    slots[i].gameObject.SetActive(true);
        //    slots[i].AddItem(inventoryItemList[i]);
        //    //if (!battleInventory) // 일반 인벤토리
        //    //{
        //    //    slots[i].gameObject.SetActive(true);
        //    //    slots[i].AddItem(inventoryItemList[i]);
        //    //}
        //    //else // 전투 인벤토리에는 전투 아이템만
        //    //{
        //    //    if (inventoryItemList[i].itemType == Item.ItemType.Battle)
        //    //    {
        //    //        for(int j = 0; j < 5; j++)
        //    //        {
        //    //            slots[j].gameObject.SetActive(true);
        //    //            slots[j].AddItem(inventoryItemList[i]);
        //    //        }
        //    //    }
        //    //}
        //}

        /* count 0 일 때 아래 함수 호출되면 에러나길래 막아뒀습니다! */
        if (currentList.Count > 0)
            SelectedItem();
        else
        {
            //RemoveSlot();
            if (!battleInventory)
            {
                Description_Icon.sprite = null;
                DescriptionName_text.text = "";
                DescriptionName_text.text = "";
            }
        }
    }

    public void SelectedItem()
    {
        StopAllCoroutines();

        List<Item> currentList = CurrentItemList();
        if(currentList == null || currentList.Count == 0)
        {
            Description_Icon.sprite = null;
            DescriptionName_text.text = "";
            Description_Text.text = "";
            return;
        }

        Color color = slots[0].selected_Item.GetComponent<Image>().color;
        color.a = 0f;

        for (int i = 0; i < inventoryItemList.Count; i++)
            slots[i].selected_Item.GetComponent<Image>().color = color;

        Description_Text.text = currentList[selectedItem].itemDescription;
        if (!battleInventory)
        {
            DescriptionName_text.text = currentList[selectedItem].itemName;
            Description_Icon.sprite = currentList[selectedItem].itemIcon;
        }

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

    public void UseAnItem(int _itemID)
    {
        for (int i = 0; i < inventoryItemList.Count; i++)
        {
            if (inventoryItemList[i].itemID == _itemID)
            {
                ItemDB.ItemEffect(_itemID);
                if (inventoryItemList[i].itemCount > 1) inventoryItemList[i].itemCount--;
                else inventoryItemList.RemoveAt(i);
                SaveInventory();

                if (battleInventory)
                {
                    LoadBattleItemList();

                    if (selectedItem >= battleItemList.Count)
                        selectedItem = battleItemList.Count - 1;
                }
                else
                {
                    if (selectedItem >= inventoryItemList.Count)
                        selectedItem = inventoryItemList.Count - 1;
                }

                if (selectedItem < 0) selectedItem = 0;

                ShowItem();
                //Debug.Log("아이템 사용");
                //return;
                //break;
            }
        }

        activeInventory = false;
        activeSelect = false;
        //StopAllCoroutines();
        keyUp = false;
        inventoryPanel.SetActive(false);
        activeItem = false;
        SaveInventory();
        Debug.Log("아이템 사용");
        return;
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
                //ItemImagePopup(ItemDB.itemList[i].itemName);
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

        //DialogueManager dm = FindObjectOfType<DialogueManager>();
        //dm.ShowMessage(message);
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        dialogueUI.ShowMessage(message);
    }

    public void ItemImagePopup(string _itemName)
    {
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
        for(int i = 0; i < dialogueUI.Images.Count; i++)
        {
            if (dialogueUI.Images[i].name == _itemName)
            {
                Debug.Log("아이템 이름: " + dialogueUI.Images[i].name);
                dialogueUI.Images[i].SetActive(true);
            }
        }
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
        //inventoryItemList.Add(new Item(10002, "B이름", "B설명", "B이름", Item.ItemType.Use));
        //inventoryItemList.Add(new Item(20102, "회의실 열쇠", "회의실 열쇠 설명", "회의실 열쇠", Item.ItemType.Use));
        inventoryItemList.Add(new Item(20101, "열람실 열쇠", "열람실 열쇠 설명", "열람실 열쇠", Item.ItemType.Use));
        inventoryItemList.Add(new Item(10402, "비타5000", "요즘 인기 최고인 에너지드링크.\n피로와 상처를 순식간에 회복시켜준다.", "비타5000", Item.ItemType.Battle));
        inventoryItemList.Add(new Item(10403, "포도주", "술과 축제의 신 디오니소스가 특별히 만든 포도주.\n생명력이 깃들어 죽어가던 사람도 살아난다고 한다.", "포도주", Item.ItemType.Battle));
        //SaveInventory();
        //Debug.Log("DefualtItem");
    }

    void Start()
    {
        instance = this;
        ItemDB = FindObjectOfType<ItemDatabase>();
        
        inventoryPanel.SetActive(activeInventory);
        if (!battleInventory)
        {
            currencyPanel.SetActive(activeInventory);
            infoPanel.SetActive(activeInventory);
            inventoryBackground.SetActive(activeInventory);
        }

        inventoryItemList = new List<Item>();
        battleItemList = new List<Item>();
        // GridSlot의 자식객체 저장
        slots = tf.GetComponentsInChildren<InventorySlot>();

        //DefaultItmes();
        LoadInventory();
    }

    void Update()
    {
        if (!battleInventory) // 일반 씬 인벤토리 *전투 씬 인벤토리는 BattleUI에
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                //Debug.Log("Tab");
                activeInventory = !activeInventory;

                if (activeInventory == true)
                {
                    inventoryPanel.SetActive(true);
                    currencyPanel.SetActive(true);
                    infoPanel.SetActive(true);
                    inventoryBackground.SetActive(true);
                    activeItem = true;
                    ShowItem();
                    selectedItem = 0;

                    Currency_Text.text = DataManager.Instance.currency.ToString();
                }
                else
                {
                    StopAllCoroutines();
                    inventoryPanel.SetActive(false);
                    currencyPanel.SetActive(false);
                    infoPanel.SetActive(false);
                    inventoryBackground.SetActive(false);
                    activeItem = false;
                    SaveInventory();
                }
            }
        }

        if(activeInventory && activeItem && battleInventory && keyUp)
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                keyUp = false;
                activeSelect = true;
            }

            return;
        }

        if (activeInventory && activeItem && activeSelect)
        {
            List<Item> currentList = CurrentItemList();

            if (battleInventory)
            {
                LoadBattleItemList();
                currentList = CurrentItemList();
            }

            if (currentList.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (selectedItem < currentList.Count - 2)
                        selectedItem += 2;
                    else
                        selectedItem %= 2;

                    if (selectedItem >= currentList.Count)
                        selectedItem = currentList.Count - 1;

                    SelectedItem();
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    if (selectedItem > 1)
                        selectedItem -= 2;
                    else
                        selectedItem = currentList.Count - 1 - selectedItem;

                    if (selectedItem < 0)
                        selectedItem = 0;

                    SelectedItem();
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    if (selectedItem < currentList.Count - 1)
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
                        selectedItem = currentList.Count - 1;

                    SelectedItem();
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    UseAnItem(currentList[selectedItem].itemID);

                    BattleManager battleManager = FindObjectOfType<BattleManager>();
                    battleManager.state = BattleManager.State.ENEMYTURN;
                }
            }
            #region legacy
            //if (activeItem)
            //{
            //    if (inventoryItemList.Count > 0)
            //    {
            //        if (Input.GetKeyDown(KeyCode.S))
            //        {

            //            if (selectedItem < inventoryItemList.Count - 2)
            //                selectedItem += 2;
            //            else
            //                selectedItem %= 2;
            //            SelectedItem();
            //        }
            //        else if (Input.GetKeyDown(KeyCode.W))
            //        {
            //            if (selectedItem > 1)
            //                selectedItem -= 2;
            //            else
            //                // 현재 선택템이 최상단에 있을 경우 최하단으로 이동
            //                selectedItem = inventoryItemList.Count - 1 - selectedItem;
            //            SelectedItem();
            //        }
            //        else if (Input.GetKeyDown(KeyCode.D))
            //        {
            //            if (selectedItem < inventoryItemList.Count - 1)
            //                selectedItem++;
            //            else
            //                selectedItem = 0;
            //            SelectedItem();
            //        }
            //        else if (Input.GetKeyDown(KeyCode.A))
            //        {
            //            if (selectedItem > 0)
            //                selectedItem--;
            //            else
            //                selectedItem = inventoryItemList.Count - 1;
            //            SelectedItem();
            //        }
            //        else if (Input.GetKeyDown(KeyCode.F))
            //        {
            //            UseAnItem(inventoryItemList[selectedItem].itemID);
            //        }
            //    }
            //}
            #endregion
        }
    }
}
