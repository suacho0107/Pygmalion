using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [SerializeField] PlayerPosition playerPos;

    [SerializeField] private GameObject buttons;
    private List<GameObject> buttonList = new List<GameObject>();

    private int selectedButtonIndex = 0;

    void Start()
    {
        playerPos.currentPosition = new Vector3(3, 0.2f, 0);

        for (int i = 0; i < buttons.transform.childCount; i++)
        {
            GameObject button = buttons.transform.GetChild(i).gameObject;
            buttonList.Add(button);
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            ButtonInputHandler();
        }
        
    }

    private void ButtonInputHandler()
    {
        //상하 이동
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (selectedButtonIndex == 1 || selectedButtonIndex == 2)
            {
                selectedButtonIndex--;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (selectedButtonIndex == 0 || selectedButtonIndex == 1)
            {
                selectedButtonIndex++;
            }
        }

        //선택
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (selectedButtonIndex == 0) //startButton
            {
                SceneManager.LoadScene("Company_LobbyTuto-1");
            }
            else if (selectedButtonIndex == 1) //battleButton
            {
                SceneTransport.previousScene = SceneManager.GetActiveScene().name;
                Debug.Log($"Statue: previousScene = {SceneTransport.previousScene}");
                SceneManager.LoadScene("Battle");
            }
            else if (selectedButtonIndex == 2) //endButton
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
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
}
