using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FieldItemManager : MonoBehaviour
{
    static FieldItemManager fi_instance;
    public static FieldItemManager Instance
    {
        get
        {
            if (fi_instance == null)
            {
                fi_instance = FindObjectOfType<FieldItemManager>();
            }
            return fi_instance;
        }
    }

    //public static FieldItemManager instance => fi_instance;

    string filePath;
    List<FieldItemData> collectedItems = new List<FieldItemData>();

    private void Awake()
    {
        if(fi_instance == null)
        {
            fi_instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        filePath = Path.Combine(Application.persistentDataPath, "collectedItems.json");
        LoadCollectedItems();
    }

    void LoadCollectedItems()
    {
        if(File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            collectedItems = JsonUtility.FromJson<FieldItemDataList>(json).items;
        }
    }

    void SaveCollectedItems()
    {
        FieldItemDataList dataList = new FieldItemDataList { items = collectedItems };
        string json = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(filePath, json);
    }

    public void CollectedItem(int itemID, Vector2 position)
    {
        collectedItems.Add(new FieldItemData { itemID = itemID, position = position, isCollected = true });
        SaveCollectedItems();
    }

    public bool IsCollected(int itemID)
    {
        return collectedItems.Exists(item => item.itemID == itemID && item.isCollected);
    }

    public void ResetFieldItems()
    {
        collectedItems.Clear();
    }
}

[System.Serializable]
public class FieldItemDataList
{
    public List<FieldItemData> items = new List<FieldItemData>();
}