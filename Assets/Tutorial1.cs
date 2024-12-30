using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial1 : MonoBehaviour
{
    public NPC npc;

    bool isEndTutorial1 = false;

    private void Start()
    {
        if (!UIManager.u_instance.isTutorialEnd)
        {
            Invoke("StartTutorial1", 1f);
        }
    }

    private void Update()
    {
        if (UIManager.u_instance.isTutorialRian2)
        {
            gameObject.SetActive(false);
        }
    }

    void StartTutorial1()
    {
        if (!isEndTutorial1)
        {
            npc.StartDialogue();
            PlayerPrefs.SetInt("Start2", 1); // 다음 대사 시작 가능 신호
            PlayerPrefs.SetInt("Start1", 0); // 현재 대사 종료
            PlayerPrefs.Save();
        }

        isEndTutorial1 = true;
    }
}
