using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject SavePopup;
    [SerializeField] private GameObject popupButtons;
    [SerializeField] private GameObject OptionTab;
    [SerializeField] private GameObject ContinueTab;

    private readonly List<GameObject> buttonList = new List<GameObject>();
    private readonly List<GameObject> popupButtonList = new List<GameObject>();
    private int selectedButtonIndex;
    private int selectedPopupButtonIndex;
    private bool isPanelOn;
    private bool isPopupOn;
    private bool isPopupFocused;

    void Awake()
    {
        CollectButtons(buttons, buttonList);
        CollectButtons(popupButtons, popupButtonList);
        ClosePanel();
    }

    private static void CollectButtons(GameObject root, List<GameObject> list)
    {
        if (root == null) return;
        foreach (Transform child in root.transform)
        {
            list.Add(child.gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPopupFocused)
            {
                isPopupFocused = false;
                HighlightButtons();
            }
            else if (isPanelOn) ClosePanel();
            else OpenPanel();
            return;
        }
        if (!isPanelOn) return;
        if (isPopupFocused) PopupButtonInputHandler();
        else ButtonInputHandler();
    }

    void ButtonInputHandler()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            selectedButtonIndex = Mathf.Max(0, selectedButtonIndex - 1);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            selectedButtonIndex = Mathf.Min(buttonList.Count - 1, selectedButtonIndex + 1);
        else if (isPopupOn && popupButtonList.Count > 0 &&
            (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Space)))
            isPopupFocused = true;
        else if (selectedButtonIndex == 0 && Input.GetKeyDown(KeyCode.Space))
        {
            ClosePanel();
            return;
        }
        UpdateSelectedPage();
        HighlightButtons();
    }

    void PopupButtonInputHandler()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            isPopupFocused = false;
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            selectedPopupButtonIndex = Mathf.Max(0, selectedPopupButtonIndex - 1);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            selectedPopupButtonIndex = Mathf.Min(popupButtonList.Count - 1, selectedPopupButtonIndex + 1);
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedPopupButtonIndex == 0)
            {
                OnSaveData();
                UIManager.u_instance.RequestSaveMessage();
                SceneManager.LoadScene("Start");
                return;
            }
            if (selectedPopupButtonIndex == 1)
            {
                SceneManager.LoadScene("Start");
                return;
            }
            // Optional third item: cancel and return to the continue page.
            selectedButtonIndex = 0;
            isPopupFocused = false;
            UpdateSelectedPage();
        }
        HighlightButtons();
    }

    private void HighlightButtons()
    {
        ApplyHighlight(buttonList, selectedButtonIndex);
        ApplyHighlight(popupButtonList, isPopupFocused ? selectedPopupButtonIndex : -1);
    }

    private static void ApplyHighlight(List<GameObject> list, int index)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Text text = list[i].GetComponent<Text>();
            if (text != null)
                text.color = i == index ? new Color32(206, 178, 120, 255) : new Color32(156, 156, 156, 255);
            if (list[i].transform.childCount > 0)
                list[i].transform.GetChild(0).gameObject.SetActive(i == index);
        }
    }

    void OpenPanel()
    {
        isPanelOn = true;
        isPopupFocused = false;
        selectedButtonIndex = 0;
        selectedPopupButtonIndex = 0;
        SetPageActive(PausePanel, true);
        foreach (GameObject button in buttonList) button.SetActive(true);
        UpdateSelectedPage();
        HighlightButtons();
    }

    void ClosePanel()
    {
        isPanelOn = false;
        isPopupFocused = false;
        UpdateSelectedPage();
        SetPageActive(PausePanel, false);
        foreach (GameObject button in buttonList) button.SetActive(false);
    }

    private void UpdateSelectedPage()
    {
        bool showPopup = isPanelOn && selectedButtonIndex == 2;
        if (showPopup != isPopupOn)
        {
            selectedPopupButtonIndex = 0;
            isPopupFocused = false;
        }
        isPopupOn = showPopup;
        SetPageActive(ContinueTab, isPanelOn && selectedButtonIndex == 0);
        SetPageActive(OptionTab, isPanelOn && selectedButtonIndex == 1);
        SetPageActive(SavePopup, showPopup);
        foreach (GameObject button in popupButtonList) button.SetActive(showPopup);
    }

    private static void SetPageActive(GameObject page, bool active)
    {
        if (page != null && page.activeSelf != active) page.SetActive(active);
    }

    public void OnSaveData()
    {
        SaveManager.s_instance.SaveData();
    }

    public bool PanelActive()
    {
        return isPanelOn;
    }
}