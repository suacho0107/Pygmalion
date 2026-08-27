using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager s_instance { get; private set; }

    [SerializeField] private PlayerPosition playerPos;

    private string SavePath => Path.Combine(Application.persistentDataPath, "SaveData.json");

    private SaveData currentSaveData = new SaveData();

    private void Awake()
    {
        if (s_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        s_instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void SaveData()
    {
        currentSaveData.sceneName = SceneManager.GetActiveScene().name;

        currentSaveData.playerPosition = playerPos.currentPosition;

        if (UIManager.u_instance != null)
        {
            currentSaveData.uiState = UIManager.u_instance.Get_CurrentState().ToString();
            currentSaveData.stageState = UIManager.u_instance.Get_StageState().ToString();
            currentSaveData.stageIndex = UIManager.u_instance.Get_StageIndex();
        }

        if (FieldItemManager.Instance != null)
        {
            currentSaveData.collectedItems = FieldItemManager.Instance.GetCollectedItems();
        }

        CaptureNPCStates();

        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(SavePath, json);

        Debug.Log("통합 데이터 저장 완료");
    }

    public void LoadData()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("저장 파일이 없습니다.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        currentSaveData = JsonUtility.FromJson<SaveData>(json);

        StartCoroutine(LoadRoutine(currentSaveData));
    }

    private IEnumerator LoadRoutine(SaveData data)
    {
        playerPos.currentPosition = data.playerPosition;
        playerPos.nextPosition = data.playerPosition;
        playerPos.isChecked = false;

        yield return SceneManager.LoadSceneAsync(data.sceneName);

        RestoreNPCStates();

        // 씬 로드 완료 후 실행
        if (FieldItemManager.Instance != null) // 보류
        {
            //FieldItemManager.Instance.
        }

        if (UIManager.u_instance != null)
        {
            if (System.Enum.TryParse(data.uiState, out Define.UI.UIState uiState))
            {
                UIManager.u_instance.Set_UIState(uiState);
            }

            if (System.Enum.TryParse(data.stageState, out Define.Stage.StageState stageState))
            {
                UIManager.u_instance.Set_StageState(stageState);
            }

            UIManager.u_instance.stageIndex = data.stageIndex;
        }
    }

    private void CaptureNPCStates()
    {
        NPC[] npcs = FindObjectsOfType<NPC>(true);
        string currentScene = SceneManager.GetActiveScene().name;

        foreach(NPC npc in npcs)
        {
            if (string.IsNullOrEmpty(npc.saveID))
            {
                npc.saveID = npc.name;
                continue;
            }

            NPCSaveEntry entry = currentSaveData.npcDatas.Find(
                x => x.npcID == npc.saveID);

            if (entry == null)
            {
                entry = new NPCSaveEntry();
                entry.npcID = npc.saveID;
                currentSaveData.npcDatas.Add(entry);
            }

            entry.sceneID = currentScene;
            entry.data = npc.CaptureData();
        }
    }

    private void RestoreNPCStates()
    {
        NPC[] npcs = FindObjectsOfType<NPC>(true);

        foreach(NPC npc in npcs)
        {
            NPCSaveEntry entry = currentSaveData.npcDatas.Find(
                x => x.npcID == npc.saveID);

            if (entry != null)
            {
                npc.RestoreData(entry.data);
            }
        }
    }
}
