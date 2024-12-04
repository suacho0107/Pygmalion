using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightCountTest : MonoBehaviour
{
    //public FightDataTest fightData;
    public NPCData npcData = new NPCData();

    //public string sceneName;

    public bool isWin;
    string filePath;

    private void Awake()
    {
        // 이러면 전투 씬에서도 조각상 이름이 같아야 함
        filePath = Application.persistentDataPath + "/" + gameObject.name + "_data.json";
        LoadFightData();
    }

    public void WinTest()
    {
        isWin = true;
        //fightData.isWin = true;
        Debug.Log("전투 승리");
        SaveFightData();
        SceneManager.LoadScene("Museum_ExhibitionRoom2");
    }

    public void LoseTest()
    {
        isWin = false;
        SaveFightData();
        SceneManager.LoadScene("Museum_Lobby");
    }

    public void SaveFightData()
    {
        npcData.isFin = isWin;

        string json = JsonUtility.ToJson(npcData);
        File.WriteAllText(filePath, json);
        Debug.Log("데이터 저장");
    }

    public void LoadFightData()
    {
        if(File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            npcData = JsonUtility.FromJson<NPCData>(json);
            Debug.Log("데이터 로드");
        }
        
        isWin = npcData.isFin;
    }
}
