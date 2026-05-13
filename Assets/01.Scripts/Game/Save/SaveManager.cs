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
        SaveData data = new SaveData();

        data.sceneName = SceneManager.GetActiveScene().name;

        data.playerPosition = playerPos.currentPosition;

        if (UIManager.u_instance != null)
        {
            data.uiState = UIManager.u_instance.Get_CurrentState().ToString();
            data.stageState = UIManager.u_instance.Get_StageState().ToString();
            data.stageIndex = UIManager.u_instance.Get_StageIndex();
        }

        if (FieldItemManager.Instance != null)
        {
            data.collectedItems = FieldItemManager.Instance.GetCollectedItems();
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public void LoadData()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("저장 파일이 없습니다.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        StartCoroutine(LoadRoutine(data));
    }

    private IEnumerator LoadRoutine(SaveData data)
    {
        playerPos.currentPosition = data.playerPosition;
        playerPos.nextPosition = data.playerPosition;
        playerPos.isChecked = false;

        yield return SceneManager.LoadSceneAsync(data.sceneName);

        // 씬 로드 완료 후 실행
        if (FieldItemManager.Instance != null) // 보류
        {
            //FieldItemManager.Instance.
        }

        if (UIManager.u_instance != null)
        {
            if (System.Enum.TryParse(data.uiState, out Define.UI.UIState uIState))
            {
                UIManager.u_instance.Set_UIState(uIState);
            }

            if (System.Enum.TryParse(data.stageState, out Define.Stage.StageState stageState))
            {
                UIManager.u_instance.Set_StageState(stageState);
            }

            UIManager.u_instance.stageIndex = data.stageIndex;
        }
    }

}
