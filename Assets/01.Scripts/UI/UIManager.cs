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
    [SerializeField] private GameObject textUIStart;

    private UI.UIState currentState;

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

        // 초기 상태 설정 - Enum: Ready
        // awake에서 Ready로 출력하는건 회사씬에서만 o
        // 튜토리얼 넘어가는 단계에서 지정해줘야 함(상사와 대화 후)
    }

    private void Start()
    {
        // 첫 씬에서 UI 오브젝트 할당
        AssignUIObjects();
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

        // 그럼 location이 아니라 그 위의게 할당됨
        textUIStart = UIStart.transform.GetChild(0).GetChild(0).gameObject;

        // 할당 후 모든 UI 비활성화
        if (UIReady != null) UIReady.SetActive(false);
        if (UIStart != null) UIStart.SetActive(false);
        if (UIWork != null) UIWork.SetActive(false);
        if (UIEnd != null) UIEnd.SetActive(false);

        Debug.Log("모든 UI 오브젝트가 비활성화되었습니다.");
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
                break;
        }

        Debug.Log($"UI 상태가 {currentState}로 전환되었습니다.");
    }

    /// <summary>
    /// 리스트 정의: 업무 장소(1~6)
    /// SceneTransition에서 키 값으로 lobby 씬 이름 값을 넘겨주면
    /// </summary>
    private List<string> locations = new List<string>
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
        // 자료구조(딕셔너리orEnum) 정의 후 상태 및 순서에 따라 6개의 장소 중 하나를 출력
        // UI 뿐 아니라 업무지시서의 "장소"에도 같은 내용이 표기되어야 함

        // 1번 Start에서 출력 후 EndUI 출력되는 시점에 인덱스 1 증가시켜 다음 장소로 업데이트

        // 혹은 위 함수의 매개변수를 인덱스로 설정

        Text _textUIStart = textUIStart.GetComponent<Text>();

        if (_textUIStart != null)
        {
            _textUIStart.text = locations[0];
        }
    }

}
