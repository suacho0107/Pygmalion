using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager u_instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject UIReady;
    [SerializeField] private GameObject UIStart;
    [SerializeField] private GameObject UIWork;
    [SerializeField] private GameObject UIEnd;

    private UI.UIState          currentState = UI.UIState.None;
    private Stage.StageState    currentStage = Stage.StageState.None;

    public List<string> locationList = new List<string>
    {
        "미술관",
        "도서관",
        "공원",
        "시청",
        "방송국",
        "병원",
    };

    public  int     stageIndex = 0;
    public  bool    isRespawn = false;
    private bool needResetAllData = false;

    void Awake()
    {
        // 싱글톤 초기화
        if (u_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        u_instance = this;
        DontDestroyOnLoad(gameObject);

        // 씬 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

        UpdateUI();
    }

    private void Start()
    {
        Assign_UIObject();

        // GameManager
        string GetCurrentScene = SceneManager.GetActiveScene().name;

        if ("Monologue_success" == GetCurrentScene)
        {
            stageIndex++;
        }
    }

    private void Update()
    {
        #region Test
        if (Input.GetKeyDown(KeyCode.O))
        {
            LoadStartScene();
            needResetAllData = true;        // 초기화 요청
        }
        #endregion
    }

    #region Test
    private void LoadStartScene()
    {
        Set_UIState(UI.UIState.Ready);

        string sceneName = "Start";

        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("씬 이름이 설정되지 않았습니다!");
        }
    }
    #endregion

    private void OnDestroy()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DeleteAllData delete = FindObjectOfType<DeleteAllData>();
        if (scene.name == "Start" && needResetAllData)
        {
            if (delete != null)
            {
                delete.DeleteAllJsonFiles();
            }
            else
            {
                Debug.LogWarning("Start 씬에서 DeleteAllData를 찾을 수 없습니다.");
            }

            if (FieldItemManager.Instance != null)
            {
                FieldItemManager.Instance.ResetFieldItems();
            }

            needResetAllData = false; // 초기화 한 번 하고 플래그 해제
        }
        else if(scene.name == "Monologue_success")
        {
            PlayerPrefs.SetInt("StatueCount", 0);
            PlayerPrefs.SetInt("destroyedCount", 0);
            PlayerPrefs.SetInt("checkedCount", 0);
            PlayerPrefs.SetInt("fightCount", 0);
            PlayerPrefs.SetInt("checkCount", 0);

            PlayerPrefs.Save();
            Debug.Log("StatueScore Reset");
        }

        Assign_UIObject();

        UpdateUI();
    }

    void Assign_UIObject()
    {
        GameObject uiCanvas = GameObject.FindWithTag("UICanvas");

        UIReady = UIReady   != null ? UIReady   : uiCanvas.transform.Find("UIReady")?.gameObject;
        UIStart = UIStart   != null ? UIStart   : uiCanvas.transform.Find("UIStart")?.gameObject;
        UIWork  = UIWork    != null ? UIWork    : uiCanvas.transform.Find("UIWork")?.gameObject;
        UIEnd   = UIEnd     != null ? UIEnd     : uiCanvas.transform.Find("UIEnd")?.gameObject;

        if (UIReady != null) UIReady.SetActive(false);
        if (UIStart != null) UIStart.SetActive(false);
        if (UIWork  != null) UIWork.SetActive(false);
        if (UIEnd   != null) UIEnd.SetActive(false);
    }

    public void Set_UIState(UI.UIState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            UpdateUI();
        }
    }

    public void Set_StageState(Stage.StageState newStage)
    {
        if (currentStage != newStage)
        {
            currentStage = newStage;
        }
    }

    public UI.UIState Get_CurrentState()
    {
        return currentState;
    }

    public Stage.StageState Get_StageState()
    {
        return currentStage;
    }

    public int Get_StageIndex()
    {
        return stageIndex;
    }

    void UpdateUI()
    {
        if (UIReady != null) UIReady.SetActive(false);
        if (UIStart != null) UIStart.SetActive(false);
        if (UIWork != null) UIWork.SetActive(false);
        if (UIEnd != null) UIEnd.SetActive(false);

        // 현재 상태에 따라 UI 활성화
        switch (currentState)
        {
            case UI.UIState.Ready:
                if (UIReady != null) UIReady.SetActive(true);
                break;

            case UI.UIState.Start:
                if (UIStart != null) UIStart.SetActive(true);
                break;

            case UI.UIState.Work:
                if (UIWork != null) UIWork.SetActive(true);
                break;

            case UI.UIState.End:
                if (UIEnd != null) UIEnd.SetActive(true);
                break;
        }

        //Debug.Log($"UI 상태가 {currentState}로 전환되었습니다.");
    }

}