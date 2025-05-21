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

    [SerializeField] private GameObject check_UIEnd;        // 조사한 조각상
    [SerializeField] private GameObject catch_UIEnd;        // 적발한 조각상
    [SerializeField] private GameObject destroy_UIEnd;      // 파손한 조각상
    [SerializeField] private GameObject efficiency_UIEnd;   // 업무 효율

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

        // 초기 상태 설정
        UpdateUI();
    }

    private void Start()
    {
        // 첫 씬에서 UI 오브젝트 할당
        AssignUIObjects();
    }

    private void Update()
    {
        // 리로드용 테스트 함수
        if (Input.GetKeyDown(KeyCode.Q))
        {
            LoadStartScene();
        }
    }

    private void LoadStartScene()
    {
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
        // 새로운 씬이 로드될 때마다 UI 오브젝트 재할당
        AssignUIObjects();

        UpdateUI();
    }

    void AssignUIObjects()
    {
        // UI Canvas 하위의 UI 오브젝트 할당
        GameObject uiCanvas = GameObject.FindWithTag("UICanvas");

        UIReady = UIReady != null ? UIReady : uiCanvas.transform.Find("UIReady")?.gameObject;
        UIStart = UIStart != null ? UIStart : uiCanvas.transform.Find("UIStart")?.gameObject;
        UIWork = UIWork != null ? UIWork : uiCanvas.transform.Find("UIWork")?.gameObject;
        UIEnd = UIEnd != null ? UIEnd : uiCanvas.transform.Find("UIEnd")?.gameObject;

        // Text 변수 할당
        location_UIStart = UIStart.transform.GetChild(0).GetChild(0).gameObject;
        check_UIEnd = UIEnd.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).gameObject;
        catch_UIEnd = UIEnd.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).gameObject;
        destroy_UIEnd = UIEnd.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(2).gameObject;
        efficiency_UIEnd = UIEnd.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(3).gameObject;

        // 할당 후 모든 UI 비활성화
        if (UIReady != null) UIReady.SetActive(false);
        if (UIStart != null) UIStart.SetActive(false);
        if (UIWork != null) UIWork.SetActive(false);
        if (UIEnd != null) UIEnd.SetActive(false);
    }

    public void SetUIState(UI.UIState newState)
    {
        // 상태 변경 및 UI 업데이트
        if (currentState != newState)
        {
            currentState = newState;
            Debug.Log($"UI 상태가 변경되었습니다: {currentState}");
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
                UpdateStartUI();
                break;

            case UI.UIState.Work:
                if (UIWork != null) UIWork.SetActive(true);
                break;

            case UI.UIState.End:
                if (UIEnd != null) UIEnd.SetActive(true);
                UpdateEndUI();
                break;
        }

        Debug.Log($"UI 상태가 {currentState}로 전환되었습니다.");
    }

    #region UI 업데이트
    /// <summary>
    /// 리스트 정의: 업무 장소(1~6)
    /// SceneTransition에서 키 값으로 lobby 씬 이름 값을 넘겨주면
    /// </summary>
    public List<string> locations = new List<string>
    {
        "미술관",
        "도서관",
        "공원",
        "시청",
        "방송국",
        "병원",
    };

    public void UpdateStartUI()
    {
        Text _textUIStart = location_UIStart.GetComponent<Text>();

        if (_textUIStart != null)
        {
            _textUIStart.text = locations[stageIndex];
        }
    }

    void UpdateEndUI()
    {
        Text _check_UIEnd = check_UIEnd.GetComponent<Text>();
        Text _catch_UIEnd = catch_UIEnd.GetComponent<Text>();
        Text _destroy_UIEnd = destroy_UIEnd.GetComponent<Text>();
        Text _efficiency_UIEnd = efficiency_UIEnd.GetComponent<Text>();
        //Text _grade_UIEnd = grade_UIEnd.GetComponent<Text>();

        StatueScore statueScore = FindObjectOfType<StatueScore>();
        // PlayerPref에서 데이터 불러와 결과보고서 변수에 저장한 뒤 출력


        #region 조각상 조사 횟수(check_UIEnd)
        if (_check_UIEnd != null)
        {
            checkCount = PlayerPrefs.GetInt("StatueCount");
            _check_UIEnd.text = checkCount.ToString();
        }
        else
        {
            Debug.Log("값이 할당되지 않았습니다.");
        }
        #endregion

        #region 적발한 조각상(catch_UIEnd)
        if (_catch_UIEnd != null)
        {
            fightCount = PlayerPrefs.GetInt("fightCount");
            _catch_UIEnd.text = fightCount.ToString();
        }
        else
        {
            Debug.Log("값이 할당되지 않았습니다.");
        }
        #endregion

        #region 파손한 조각상(destroy_UIEnd)
        if (_destroy_UIEnd != null)
        {
            destroyedCount = PlayerPrefs.GetInt("destroyedCount");
            _destroy_UIEnd.text = destroyedCount.ToString();
        }
        else
        {
            Debug.Log("값이 할당되지 않았습니다.");
        }
        #endregion

        #region 업무효율(efficiency_UIEnd)
        if (_efficiency_UIEnd != null)
        {
            // 업무 효율(조사 효율, 전투 효율)
            float investigationEfficiency = checkCount / 12;
            float battleEfficiency = fightCount / 1;
            workEfficiency = investigationEfficiency + battleEfficiency;

            // 등급 산출 및 출력
            if (workEfficiency < 1)
            {
                efficiency = "탁월";
            }
            else if (workEfficiency >= 1 && workEfficiency < 1.5)
            {
                efficiency = "우수";
            }
            else if (workEfficiency >= 1.5 && workEfficiency < 2)
            {
                efficiency = "충족";
            }
            else if (workEfficiency >= 2 && workEfficiency < 2.5)
            {
                efficiency = "개선 필요";
            }
            else
            {
                efficiency = "미흡";
            }

            _efficiency_UIEnd.text = efficiency;
        }
        else
        {
            Debug.Log("값이 할당되지 않았습니다.");
        }
        #endregion

        #region 평가등급(grade_UIEnd)
        if (UIEnd != null)
        {
            // 판별 정확도
            float catchAccurarcy = destroyedCount / 5;

            // 총 평가등급(=업무 효율 + 판별 정확도)
            float totalGrade = workEfficiency + catchAccurarcy;

            if (totalGrade < 1)
            {
                UIEnd.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
            }
            else if (totalGrade >= 1 && totalGrade < 1.3)
            {
                UIEnd.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(true);
            }
            else if (totalGrade >= 1.3 && totalGrade < 1.7)
            {
                UIEnd.transform.GetChild(0).GetChild(1).GetChild(2).gameObject.SetActive(true);
            }
            else if (totalGrade >= 1.7 && totalGrade < 2.2)
            {
                UIEnd.transform.GetChild(0).GetChild(1).GetChild(3).gameObject.SetActive(true);
            }
            else
            {
                UIEnd.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("값이 할당되지 않았습니다.");
        }
        #endregion
    }


    #endregion
}