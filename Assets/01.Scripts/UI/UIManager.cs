using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Define;

public class UIManager : MonoBehaviour
{
    public static UIManager u_instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject readyUI;
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject workUI;
    [SerializeField] private GameObject endUI;

    [Header("Text")]
    // [SerializeField] private GameObject readyUI;
    [SerializeField] private Text startUIText;
    // [SerializeField] private GameObject workUI;
    // [SerializeField] private GameObject endUI;

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

        // 초기 상태 설정
        currentState = UI.UIState.Ready;
        UpdateUI();

    }

    public void SetUIState(UI.UIState newState)
    {
        // 상태 변경 및 UI 업데이트
        if (currentState != newState)
        {
            currentState = newState;
            UpdateUI();
        }
    }

    public UI.UIState GetCurrentState()
    {
        return currentState;
    }

    void UpdateUI()
    {
        readyUI.SetActive(false);
        startUI.SetActive(false);
        workUI.SetActive(false);
        endUI.SetActive(false);

        // 현재 상태에 따라 UI 활성화
        switch (currentState)
        {
            case UI.UIState.Ready:
                readyUI.SetActive(true);
                break;

            case UI.UIState.Start:
                startUI.SetActive(true);
                break;

            case UI.UIState.Work:
                workUI.SetActive(true);
                break;

            case UI.UIState.End:
                endUI.SetActive(true);
                break;
        }

        Debug.Log($"UI 상태가 {currentState}로 전환되었습니다.");
    }

    public void UpdateUIText(Text newText)
    {
        if (startUIText != null)
        {
            startUIText.text = newText.text;
            Debug.Log("UIManager는 정상");
        }
        else
        {
            return;
        }
    }

}
