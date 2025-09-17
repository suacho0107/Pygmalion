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

    [Header("Text")]
    [SerializeField] public GameObject location_UIStart;

    private UI.UIState currentState;

    private int     checkCount;
    private int     fightCount;
    private int     destroyedCount;
    private string  efficiency;
    private float   workEfficiency;

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
        AssignUIObjects();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LoadStartScene();
        }
    }

    private void LoadStartScene()
    {
        SetUIState(UI.UIState.Ready);

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

    private void OnDestroy()
    {
        // 씬 로드 이벤트 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignUIObjects();

        UpdateUI();
    }

    void AssignUIObjects()
    {
        // UI Canvas 하위의 UI 오브젝트 할당
        GameObject uiCanvas = GameObject.FindWithTag("UICanvas");

        UIReady = UIReady   != null ? UIReady   : uiCanvas.transform.Find("UIReady")?.gameObject;
        UIStart = UIStart   != null ? UIStart   : uiCanvas.transform.Find("UIStart")?.gameObject;
        UIWork  = UIWork    != null ? UIWork    : uiCanvas.transform.Find("UIWork")?.gameObject;
        UIEnd   = UIEnd     != null ? UIEnd     : uiCanvas.transform.Find("UIEnd")?.gameObject;

        location_UIStart = UIStart.transform.GetChild(0).GetChild(0).gameObject;

        if (UIReady != null) UIReady.SetActive(false);
        if (UIStart != null) UIStart.SetActive(false);
        if (UIWork  != null) UIWork.SetActive(false);
        if (UIEnd   != null) UIEnd.SetActive(false);
    }

    public void SetUIState(UI.UIState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            //Debug.Log($"UI 상태가 변경되었습니다: {currentState}");
            UpdateUI();
        }
    }

    public UI.UIState GetCurrentState()
    {
        return currentState;
    }

    void UpdateUI()
    {
        // 모든 UI 비활성화
        if (UIReady != null) UIReady.SetActive(false);
        if (UIStart != null) UIStart.SetActive(false);
        if (UIWork  != null) UIWork.SetActive(false);
        if (UIEnd   != null) UIEnd.SetActive(false);

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

    #region UI 업데이트
    /// <summary>
    /// 리스트 정의: 업무 장소(1~6)
    /// SceneTransition에서 키 값으로 lobby 씬 이름 값을 넘겨주면
    /// </summary>
    public List<string> locationList = new List<string>
    {
        "미술관",
        "도서관",
        "공원",
        "시청",
        "방송국",
        "병원",
    };

    #endregion
}