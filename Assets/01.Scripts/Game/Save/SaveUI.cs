using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveUI : MonoBehaviour
{
    [SerializeField] GameObject SavePanel;
    [SerializeField] private GameObject buttons;

    private List<GameObject> buttonList = new List<GameObject>();
    private int selectedButtonIndex = 0;

    private Scene currentScene;

    bool isPanelOn = false;

    void Start()
    {
        SavePanel.SetActive(false);

        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            GameObject button = buttons.transform.GetChild(i).gameObject;
            buttonList.Add(button);

            button.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPanelOn = !isPanelOn;
            SavePanel.SetActive(isPanelOn);

            for (int i = 0; i < buttons.transform.childCount; i++)
            {
                GameObject button = buttons.transform.GetChild(i).gameObject;

                if (i == 1)
                    button.transform.GetChild(0).gameObject.SetActive(isPanelOn);

                button.SetActive(isPanelOn);
            }
        }

        if (isPanelOn)
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
            if (selectedButtonIndex == 0) // 이어하기
            {
                SaveManager.s_instance.LoadData();
            }
            else if (selectedButtonIndex == 1) // 설정
            {

            }
            else if (selectedButtonIndex == 2) // 시작화면으로 돌아가기
            {
                OnSaveData();
                SceneManager.LoadScene("Start");
            }
        }
        HighlightButton();
    }

    private void HighlightButton()
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

    public void OnSaveData()
    {
        SaveManager.s_instance.SaveData();
    }
}
