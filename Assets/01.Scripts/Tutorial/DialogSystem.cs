using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Speaker { You = 0, Bella, Sophia, Ryan }

public class DialogSystem : MonoBehaviour
{
    [SerializeField]
    private Dialog[]            dialogs;
    [SerializeField]
    private Image[]             imageDialogs;
    [SerializeField]
    private TextMeshProUGUI[]   textNames;
    [SerializeField]
    private TextMeshProUGUI[]   textDialogues;
    [SerializeField]
    private GameObject[]        objectArrows;
    [SerializeField]
    private float               typingSpeed;
    [SerializeField]
    private KeyCode             keyCodeSkip = KeyCode.Space;

    private int                 currentIndex = -1;
    private bool                isTypingEffect = false;
    private Speaker             currentSpeaker = Speaker.You;

    public void Setup()
    {
        for (int i = 0; i < 2; i++)
        {
            // 모든 대화 관련 게임오브젝트 비활성화
            InActiveObjects(i);
        }

        SetNextDialog();
    }

    public bool UpdateDialog()
    {
        if (Input.GetKeyDown(keyCodeSkip) || Input.GetMouseButtonDown(0))
        {
            // 텍스트 타이핑 효과 재생중일 때 마우스 좌클릭 시 타이핑 효과 종료
            if (isTypingEffect == true)
            {
                // 타이핑 효과를 중지하고, 현재 대사 전체를 출력한다
                StopCoroutine("TypingText");
                isTypingEffect = false;
                textDialogues[(int)currentSpeaker].text = dialogs[currentIndex].dialogue;

                // 대사가 완료되었을 떄 출력되는 커서 활성화
                objectArrows[(int)currentSpeaker].SetActive(true);

                return false;
            }

            // 다음 대사 진행
            if (dialogs.Length > currentIndex + 1)
            {
                SetNextDialog();
            }
            // 대사가 더 이상 없을 경우 true 반환
            else
            {
                // 모든 캐릭터 이미지를 어둡게 설정
                for (int i = 0; i < 2; ++i)
                {
                    // 모든 대화 관련 게임오브젝트 비활성화
                    InActiveObjects(i);
                }

                return true;
            }
        }

        return false;
    }

    private void SetNextDialog()
    {
        // 이전 화자의 대화 관련 오브젝트 비활성화
        InActiveObjects((int)currentSpeaker);

        currentIndex++;

        // 현재 화자 설정
        currentSpeaker = dialogs[currentIndex].speaker;

        // 대화창 활성화
        imageDialogs[(int)currentSpeaker].gameObject.SetActive(true);

        // 현재 화자 이름 설정
        textNames[(int)currentSpeaker].gameObject.SetActive(true);
        textNames[(int)currentSpeaker].text = dialogs[currentIndex].speaker.ToString();

        // 화자의 대사 텍스트 활성화 및 설정 (Typing Effect)
        textDialogues[(int)currentSpeaker].gameObject.SetActive(true);
    }

    private void InActiveObjects(int index)
    {
        imageDialogs[index].gameObject.SetActive(false);
        textNames[index].gameObject.SetActive(false);
        textDialogues[index].gameObject.SetActive(false);
        objectArrows[index].SetActive(false);
    }

    private IEnumerator TypingText()
    {
        int index = 0;

        isTypingEffect = true;

        // 타이핑
        while(index < dialogs[currentIndex].dialogue.Length)
        {
            textDialogues[(int)currentSpeaker].text = dialogs[currentIndex].dialogue.Substring(0, index);

            index++;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTypingEffect = false;

        // 대사 완료 시 출력되는 커서 활성화
        objectArrows[(int)currentSpeaker].SetActive(true);
    }
}

[System.Serializable]
public struct Dialog
{
    public Speaker speaker;     // 화자
    [TextArea(3, 5)]
    public string dialogue;     // 대사
}