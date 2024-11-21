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
        // �̱��� �ʱ�ȭ
        if (u_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        u_instance = this;
        DontDestroyOnLoad(gameObject);

        // �ʱ� ���� ����
        currentState = UI.UIState.Ready;
        UpdateUI();

    }

    public void SetUIState(UI.UIState newState)
    {
        // ���� ���� �� UI ������Ʈ
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

        // ���� ���¿� ���� UI Ȱ��ȭ
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

        Debug.Log($"UI ���°� {currentState}�� ��ȯ�Ǿ����ϴ�.");
    }

    public void UpdateUIText(Text newText)
    {
        if (startUIText != null)
        {
            startUIText.text = newText.text;
            Debug.Log("UIManager�� ����");
        }
        else
        {
            return;
        }
    }

}
