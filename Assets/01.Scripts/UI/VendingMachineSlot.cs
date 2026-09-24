using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VendingMachineSlot : MonoBehaviour
{
    [SerializeField] GameObject selectedImage;
    [SerializeField] Image      icon;
    [SerializeField] Text       itemNameText;

    public void SetItem(Item item)
    {
        icon.sprite = item.itemIcon;
        itemNameText.text = item.itemName;
    }

    public void SetSelected(bool bSelected)
    {
        selectedImage.SetActive(bSelected);
    }
}
