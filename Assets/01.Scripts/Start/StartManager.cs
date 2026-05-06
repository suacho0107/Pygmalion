using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    #region Variables
    [SerializeField] PlayerPosition playerPos;

    [SerializeField] private GameObject buttons;

    [SerializeField] private GameObject blackBoard;
    [SerializeField] private GameObject controls;

    private List<GameObject> buttonList = new List<GameObject>();

    private int selectedButtonIndex = 0;

    private bool isFadeInOut = false;
    private bool isStoryMode = false;

    #endregion

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
        if (!isFadeInOut && !isStoryMode)
        {
            ButtonInputHandler();
        }

        if (!isFadeInOut && isStoryMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Company_LobbyTuto-1");
            }
        }
        
    }

    private void ButtonInputHandler()
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
            if (selectedButtonIndex == 0) //시작하기
            {
                StartCoroutine(FadeInOut(true, 1f));
            }
            else if (selectedButtonIndex == 1) // 이어하기
            {
                SaveManager.s_instance.LoadData();
            }
            else if (selectedButtonIndex == 2) //종료하기
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

    //private void ButtonInputHandler()
    //{
    //    //상하 이동
    //    if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
    //    {
    //        if (selectedButtonIndex == 1 || selectedButtonIndex == 2)
    //        {
    //            selectedButtonIndex--;
    //        }
    //    }
    //    else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
    //    {
    //        if (selectedButtonIndex == 0 || selectedButtonIndex == 1)
    //        {
    //            selectedButtonIndex++;
    //        }
    //    }

    //    //선택
    //    else if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        if (selectedButtonIndex == 0) //startButton
    //        {
    //            //SceneManager.LoadScene("Company_LobbyTuto-1");
    //            StartCoroutine(FadeInOut(true, 1f));
    //        }
    //        else if (selectedButtonIndex == 1) //battleButton
    //        {
    //            SceneTransport.previousScene = SceneManager.GetActiveScene().name;
    //            Debug.Log($"Statue: previousScene = {SceneTransport.previousScene}");
    //            SceneManager.LoadScene("Battle");
    //        }
    //        else if (selectedButtonIndex == 2) //endButton
    //        {
    //            #if UNITY_EDITOR
    //            UnityEditor.EditorApplication.isPlaying = false;
    //            #else
    //            Application.Quit();
    //            #endif
    //        }
    //    }
    //    HighlightButton();
    //}

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

    public IEnumerator FadeInOut(bool _isFadeIn, float _duration)
    {
        isFadeInOut = true;

        blackBoard.SetActive(true);
        Image image = blackBoard.GetComponent<Image>();

        //Fade In/Out 설정
        float startAlpha = _isFadeIn ? 0f : 1f;
        float endAlpha = _isFadeIn ? 1f : 0f;

        //초기화
        float time = 0f;
        Color color = image.color;

        while (time < _duration)
        {
            float t = time / _duration;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            image.color = color;

            time += Time.deltaTime;
            yield return null;
        }

        // 마지막 값 보정
        color.a = endAlpha;
        image.color = color;

        isFadeInOut = false;
        isStoryMode = true;

        yield return new WaitForSeconds(0.03f);
        controls.SetActive(true);
    }
}
