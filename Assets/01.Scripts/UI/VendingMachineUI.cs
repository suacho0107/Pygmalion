using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachineUI : MonoBehaviour
{
    [System.Serializable]
    public class Product
    {
        public int itemID;
        [Min(0)]
        public int price;
    }

    [SerializeField] private VendingMachine VMObject;
    [SerializeField] private GameObject     VMPanel;
    [SerializeField] private ItemDatabase   itemDB;

    [SerializeField] private VendingMachineSlot[] Slots;
    [SerializeField] private Product[] saleProducts;

    [SerializeField] private Image      descriptionImage;
    [SerializeField] private Text       descriptionName;
    [SerializeField] private Text       descriptionText;
    [SerializeField] private Text       priceText;
    [SerializeField] private Text       currencyText;

    private List<Item>  products = new List<Item>();
    private List<int>   prices = new List<int>();

    [SerializeField] private PlayerMove playerMove;
    private bool previousMoveState;

    private bool IsOpen = false;
    private int selectedIndex;

    private const int Columns = 2;

    void Start()
    {
        if (playerMove == null)
            playerMove = FindObjectOfType<PlayerMove>();

        VMPanel.SetActive(false);
    }

    void Update()
    {
        UpdateMachine();
    }

    void UpdateMachine()
    {
        if (!VMObject.IsPlayerRange)
        {
            if (IsOpen)
                Close();

            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (IsOpen)
                Close();
            else
                Open();

            return;
        }

        if (IsOpen)
        {
            if (DataManager.Instance != null)
                currencyText.text = DataManager.Instance.currency.ToString();

            SelectInput();

            if (Input.GetKeyDown(KeyCode.Space))
                BuySelectedItem();
        }
    }

    void Open()
    {
        products.Clear();
        prices.Clear();

        foreach (VendingMachineSlot slot in Slots)
        {
            slot.SetSelected(false);
            slot.gameObject.SetActive(false);
        }

        foreach (Product product in saleProducts)
        {
            if (products.Count >= Slots.Length)
                break;

            if (product == null || product.price < 0)
                continue;

            Item item = itemDB.itemList.Find(x => x.itemID == product.itemID);

            if (item == null)
            {
                Debug.LogWarning("등록되지 않은 상품 ID: " + product.itemID);
                continue;
            }

            int index = products.Count;

            products.Add(item);
            prices.Add(product.price);

            Slots[index].SetItem(item);
            Slots[index].gameObject.SetActive(true);
        }

        selectedIndex = 0;
        IsOpen = true;

        if (playerMove != null)
        {
            previousMoveState = playerMove.IsMoved;
            playerMove.IsMoved = false;
        }

        VMPanel.SetActive(true);

        UpdateSelection();
    }

    void Close()
    {
        if (!IsOpen)
            return;

        IsOpen = false;

        if (playerMove != null)
            playerMove.IsMoved = previousMoveState;

        if (VMPanel != null)
            VMPanel.SetActive(false);
    }

    void OnDisable()
    {
        Close();
    }

    void UpdateSelection()
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            bool bSelected = products.Count > 0 && i == selectedIndex;
            Slots[i].SetSelected(bSelected);
        }

        if (products.Count == 0)
        {
            descriptionImage.enabled = false;
            descriptionName.text = "";
            descriptionText.text = "";
            priceText.text = "";
            return;
        }

        Item item = products[selectedIndex];

        descriptionImage.sprite = item.itemIcon;
        descriptionImage.enabled = item.itemIcon != null;
        descriptionName.text = item.itemName;
        descriptionText.text = item.itemDescription;
        priceText.text = prices[selectedIndex].ToString();
    }

    void SelectInput()
    {
        if (products.Count == 0)
            return;

        int nextIndex = selectedIndex;
        int column = selectedIndex % Columns;

        if (Input.GetKeyDown(KeyCode.A) && column > 0)
            nextIndex--;

        else if (Input.GetKeyDown(KeyCode.D) && column < Columns - 1)
            nextIndex++;

        else if (Input.GetKeyDown(KeyCode.W))
            nextIndex -= Columns;

        else if (Input.GetKeyDown(KeyCode.S))
            nextIndex += Columns;

        if (nextIndex < 0 || nextIndex >= products.Count)
            return;

        if (nextIndex == selectedIndex)
            return;

        selectedIndex = nextIndex;
        UpdateSelection();
    }

    void BuySelectedItem()
    {
        if (products.Count == 0)
            return;

        InventoryUI inventory = InventoryUI.instance;
        DataManager wallet = DataManager.Instance;

        if (inventory == null || wallet == null)
            return;

        Item item = products[selectedIndex];
        int price = prices[selectedIndex];

        if (wallet.currency < price)
        {
            descriptionText.text = "너무 비쌉니다.";
            return;
        }

        bool added = inventory.TryAddPurchasedItem(item.itemID);

        if (!added)
        {
            descriptionText.text = "아이템을 추가할 수 없습니다. 인벤토리를 확인하세요.";
            return;
        }

        wallet.AddCurrency(-price);
        PlayerPrefs.Save();

        descriptionText.text = item.itemName + "구매 완료.";
    }
}
