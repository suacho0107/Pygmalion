using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveUI : MonoBehaviour
{
    [SerializeField] GameObject SavePanel;
    [SerializeField] private GameObject buttons;
    [SerializeField] GameObject SavePopup;
    [SerializeField] private GameObject popupButtons;

    private List<GameObject> buttonList = new List<GameObject>();
    private List<GameObject> popupButtonList = new List<GameObject>();
    private int selectedButtonIndex = 0;
    private int selectedPopupButtonIndex = 0;

    private Scene currentScene;

    bool isPanelOn = false;
    bool isPopupOn = false;

    void Start()
    {
        SavePanel.SetActive(false);
        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            GameObject button = buttons.transform.GetChild(i).gameObject;
            buttonList.Add(button);

            button.SetActive(false);
        }

        SavePopup.SetActive(false);
        for (int i = 0; i < popupButtons.transform.childCount; i++)
        {
            GameObject button = popupButtons.transform.GetChild(i).gameObject;
            popupButtonList.Add(button);

            button.SetActive(false);
        }

        selectedButtonIndex = selectedPopupButtonIndex = 0;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPopupOn)
            {
                ClosePopup();
            }
            else if (isPanelOn)
            {
                ClosePanel();
            }
            else
            {
                OpenPanel();
            }

            return;
        }

        if (isPopupOn)
        {
            PopupButtonInputHandler();
        }
        else if (isPanelOn)
        {
            ButtonInputHandler();
        }
    }

    void ButtonInputHandler()
    {
        //상하 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (selectedButtonIndex > 0)
            {
                selectedButtonIndex--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (selectedButtonIndex < buttons.transform.childCount - 1)
            {
                selectedButtonIndex++;
            }
        }

        //선택
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedButtonIndex == 0) // 계속하기
            {
                ClosePanel();
                ClosePopup();
            }
            else if (selectedButtonIndex == 1) // 설정
            {

            }
            else if (selectedButtonIndex == 2) // 시작화면으로 돌아가기
            {
                OpenPopup();
            }
        }
        HighlightButton();
    }

    void PopupButtonInputHandler()
    {
        //상하 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (selectedPopupButtonIndex > 0)
            {
                selectedPopupButtonIndex--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (selectedPopupButtonIndex < popupButtons.transform.childCount - 1)
            {
                selectedPopupButtonIndex++;
            }
        }

        //선택
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedPopupButtonIndex == 0) // 저장하고 나가기
            {
                OnSaveData();
                UIManager.u_instance.RequestSaveMessage();
                SceneManager.LoadScene("Start");
            }
            else if (selectedPopupButtonIndex == 1) // 저장하지않고 나가기
            {
                SceneManager.LoadScene("Start");
            }
        }
        HighlightButton();
    }

    private void HighlightButton()
    {
        if (isPanelOn && !isPopupOn)
        {
            if (selectedButtonIndex < 0 || selectedButtonIndex >= buttonList.Count)
            {
                return;
            }

            for (int i = 0; i < buttonList.Count; i++)
            {
                GameObject selectedButton = buttonList[i];

                //Text Color 변경
                Text text = selectedButton.GetComponent<Text>();
                text.color = (i == selectedButtonIndex) ? new Color32(206, 178, 120, 255) : new Color32(156, 156, 156, 255);

                //Text 하단 Image
                GameObject selectedImage = selectedButton.transform.GetChild(0).gameObject;
                selectedImage.SetActive(i == selectedButtonIndex ? true : false);
            }
        }
        else if (isPanelOn && isPopupOn)
        {
            if (selectedPopupButtonIndex < 0 || selectedPopupButtonIndex >= popupButtonList.Count)
            {
                return;
            }

            for (int i = 0; i < popupButtonList.Count; i++)
            {
                GameObject selectedButton = popupButtonList[i];

                Text text = selectedButton.GetComponent<Text>();
                text.color = (i == selectedPopupButtonIndex) ? new Color32(206, 178, 120, 255) : new Color32(156, 156, 156, 255);

                GameObject selectedImage = selectedButton.transform.GetChild(0).gameObject;
                selectedImage.SetActive(i == selectedPopupButtonIndex ? true : false);
            }
        }
    }

    void OpenPanel()
    {
        isPanelOn = true;
        isPopupOn = false;

        SavePanel.SetActive(isPanelOn);
        SavePopup.SetActive(isPopupOn);

        for (int i = 0; i < buttonList.Count; i++)
        {
            buttonList[i].SetActive(isPanelOn);
        }

        for (int i = 0; i < popupButtonList.Count; i++)
        {
            popupButtonList[i].SetActive(isPopupOn);
        }

        HighlightButton();
    }

    void ClosePanel()
    {
        isPanelOn = false;
        isPopupOn = false;

        SavePanel.SetActive(isPanelOn);
        SavePopup.SetActive(isPopupOn);

        for (int i = 0; i < buttonList.Count; i++)
        {
            buttonList[i].SetActive(isPanelOn);
        }

        for (int i = 0; i < popupButtonList.Count; i++)
        {
            popupButtonList[i].SetActive(isPopupOn);
        }
    }

    void OpenPopup()
    {
        isPopupOn = true;
        selectedPopupButtonIndex = 0;

        SavePopup.SetActive(isPopupOn);

        for (int i = 0; i < popupButtonList.Count; i++)
        {
            popupButtonList[i].SetActive(isPopupOn);
        }
    }

    void ClosePopup()
    {
        isPopupOn = false;
        SavePopup.SetActive(false);

        for (int i = 0; i < popupButtonList.Count; i++)
        {
            popupButtonList[i].SetActive(false);
        }

        HighlightButton();
    }

    public void OnSaveData()
    {
        SaveManager.s_instance.SaveData();
    }
}
