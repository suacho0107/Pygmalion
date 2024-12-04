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
        // �̱��� �ʱ�ȭ
        if (u_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        u_instance = this;
        DontDestroyOnLoad(gameObject);

        // �� �ε� �̺�Ʈ ���
        SceneManager.sceneLoaded += OnSceneLoaded;

        // �ʱ� ���� ����
        UpdateUI();

        // �ʱ� ���� ���� - Enum: Ready
        // awake���� Ready�� ����ϴ°� ȸ��������� o
        // Ʃ�丮�� �Ѿ�� �ܰ迡�� ��������� ��(���� ��ȭ ��)
    }

    private void Start()
    {
        // ù ������ UI ������Ʈ �Ҵ�
        AssignUIObjects();
    }

    private void OnDestroy()
    {
        // �� �ε� �̺�Ʈ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���ο� ���� �ε�� ������ UI ������Ʈ ���Ҵ�
        AssignUIObjects();

        UpdateUI();
    }

    void AssignUIObjects()
    {
        // UI Canvas ������ UI ������Ʈ �Ҵ�
        GameObject uiCanvas = GameObject.FindWithTag("UICanvas");

        UIReady = UIReady != null ? UIReady : uiCanvas.transform.Find("UIReady")?.gameObject;
        UIStart = UIStart != null ? UIStart : uiCanvas.transform.Find("UIStart")?.gameObject;
        UIWork = UIWork != null ? UIWork : uiCanvas.transform.Find("UIWork")?.gameObject;
        UIEnd = UIEnd != null ? UIEnd : uiCanvas.transform.Find("UIEnd")?.gameObject;

        // �׷� location�� �ƴ϶� �� ���ǰ� �Ҵ��
        textUIStart = UIStart.transform.GetChild(0).GetChild(0).gameObject;

        // �Ҵ� �� ��� UI ��Ȱ��ȭ
        if (UIReady != null) UIReady.SetActive(false);
        if (UIStart != null) UIStart.SetActive(false);
        if (UIWork != null) UIWork.SetActive(false);
        if (UIEnd != null) UIEnd.SetActive(false);

        Debug.Log("��� UI ������Ʈ�� ��Ȱ��ȭ�Ǿ����ϴ�.");
    }

    public void SetUIState(UI.UIState newState)
    {
        // ���� ���� �� UI ������Ʈ
        if (currentState != newState)
        {
            currentState = newState;
            Debug.Log($"UI ���°� ����Ǿ����ϴ�: {currentState}");
            UpdateUI();
        }
    }

    public UI.UIState GetCurrentState()
    {
        return currentState;
    }

    void UpdateUI()
    {
        // ��� UI ��Ȱ��ȭ
        if (UIReady != null) UIReady.SetActive(false);
        if (UIStart != null) UIStart.SetActive(false);
        if (UIWork != null) UIWork.SetActive(false);
        if (UIEnd != null) UIEnd.SetActive(false);

        // ���� ���¿� ���� UI Ȱ��ȭ
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

        Debug.Log($"UI ���°� {currentState}�� ��ȯ�Ǿ����ϴ�.");
    }

    /// <summary>
    /// ����Ʈ ����: ���� ���(1~6)
    /// SceneTransition���� Ű ������ lobby �� �̸� ���� �Ѱ��ָ�
    /// </summary>
    private List<string> locations = new List<string>
    {
        "�̼���",
        "������",
        "����",
        "��û",
        "��۱�",
        "����",
    };

    public void UpdateStartUI()
    {
        // �ڷᱸ��(��ųʸ�orEnum) ���� �� ���� �� ������ ���� 6���� ��� �� �ϳ��� ���
        // UI �� �ƴ϶� �������ü��� "���"���� ���� ������ ǥ��Ǿ�� ��

        // 1�� Start���� ��� �� EndUI ��µǴ� ������ �ε��� 1 �������� ���� ��ҷ� ������Ʈ

        // Ȥ�� �� �Լ��� �Ű������� �ε����� ����

        Text _textUIStart = textUIStart.GetComponent<Text>();

        if (_textUIStart != null)
        {
            _textUIStart.text = locations[0];
        }
    }

}
