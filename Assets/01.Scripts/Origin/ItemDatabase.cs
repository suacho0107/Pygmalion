using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<Item> itemList = new List<Item>();

    public void ItemEffect(int _itemID)
    {
        switch (_itemID)
        {
            case 10402:
                Debug.Log("비타5000 효과");
                break;
            case 10403:
                Debug.Log("포도주 효과");
                break;
        }
    }
    
    void Start()
    {
        //itemList.Add(new Item(10001, "Items_10", "A설명", "Itmes_10", Item.ItemType.Use));
        //itemList.Add(new Item(10002, "B이름", "B설명", "B이름", Item.ItemType.Use));
        //itemList.Add(new Item(20001, "C이름", "C설명", "C이름", Item.ItemType.Equip));
        itemList.Add(new Item(10401, "열쇠꾸러미", "정원의 경비원에게 보여주자.", "열쇠꾸러미", Item.ItemType.Use));
        itemList.Add(new Item(10402, "비타5000", "요즘 인기 최고인 에너지드링크.\n피로와 상처를 순식간에 회복시켜준다.", "비타5000", Item.ItemType.Battle));
        itemList.Add(new Item(10403, "포도주", "술과 축제의 신 디오니소스가 특별히 만든 포도주.\n생명력이 깃들어 죽어가던 사람도 살아난다고 한다.", "포도주", Item.ItemType.Battle));
        itemList.Add(new Item(10301, "의문의 파편", "이게 뭐지?", "1-1손톱들", Item.ItemType.Quest));
        itemList.Add(new Item(20101, "열람실 열쇠", "도서관 B1 열람실의 잠금을 해제할 수 있다.", "열람실 열쇠", Item.ItemType.Use));
        itemList.Add(new Item(20102, "회의실A 열쇠", "도서관 1F 회의실A의 잠금을 해제할 수 있다.", "회의실A 열쇠", Item.ItemType.Use));
        itemList.Add(new Item(20103, "수상한 액체가 든 병", "정체를 모르는 수상한 액체가 들어 있다. 실수로\n조각상에 뿌리면 부식될 수 있으니 주의하자.", "수상한 액체가 든 병", Item.ItemType.Battle));
        itemList.Add(new Item(20104, "의문의 파편", "이게 뭐지?", "2-2발", Item.ItemType.Quest));
        itemList.Add(new Item(20201, "의문의 파편", "이게 뭐지?", "2-1손가락1개", Item.ItemType.Quest));
    }    
}
