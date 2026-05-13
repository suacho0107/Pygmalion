using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string sceneName;

    public Vector3 playerPosition;

    public string uiState;
    public string stageState;
    public int stageIndex;

    public List<FieldItemData> collectedItems = new List<FieldItemData>();
}
