using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCSaveEntry
{
    public string npcID;
    public string sceneID;
    public NPCData data;
}

[System.Serializable]
public class SaveData
{
    public string sceneName;

    public Vector3 playerPosition;

    public string uiState;
    public string stageState;
    public int stageIndex;

    public List<FieldItemData> collectedItems = new List<FieldItemData>();
    public List<NPCSaveEntry> npcDatas = new List<NPCSaveEntry>();
}
