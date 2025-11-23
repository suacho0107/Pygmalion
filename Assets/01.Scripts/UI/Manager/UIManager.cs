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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LoadStartScene();
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