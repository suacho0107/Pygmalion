using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [SerializeField] PlayerPosition     playerPosition;
    [SerializeField] Vector3            nextPos;
    [SerializeField] List<TutorialBase> tutorials;
    [SerializeField] string             nextSceneName = "";
    [SerializeField] SceneController    sceneController;

    private TutorialBase        currentTutorial = null;
    private int                 currentIndex = -1;

    void Start()
    {
        // 튜토리얼 행동 설정
        SetNextTutorial();
    }

    void Update()
    {
        // 현재 튜토리얼 행동 함수 호출
        if (currentTutorial != null)
        {
            currentTutorial.Execute(this);
        }
    }

    public void SetNextTutorial()
    {
        // 현재 튜토리얼의 Exit() 메소드 호출
        if (currentTutorial != null)
        {
            //Debug.Log($"튜토리얼 종료: {currentTutorial.gameObject.name}");
            currentTutorial.Exit();
        }

        // 마지막 튜토리얼을 진행했다면 CompletedAllTutorials() 메소드 호출 후 종료(return)
        if (currentIndex >= tutorials.Count - 1)
        {
            CompletedAllTutorials();
            return;
        }

        // 다음 튜토리얼 과정을 currentTutorial로 등록
        ++currentIndex;
        currentTutorial = tutorials[currentIndex];

        //Debug.Log($"튜토리얼 시작: {currentTutorial.gameObject.name}");

        currentTutorial.Enter();
    }

    public void CompletedAllTutorials()
    {
        currentTutorial = null;

        //Debug.Log("Complete All");

        if (null != sceneController)
        {
            playerPosition.nextPosition = sceneController.nextPos;
            playerPosition.isChecked = true;

            SceneManager.LoadScene(sceneController.nextSceneName);
        }
        else if (!nextSceneName.Equals(""))
        {
            playerPosition.nextPosition = nextPos;
            playerPosition.isChecked = true;

            // (임시)데모 씬으로 전환
            if (UIManager.u_instance.stageIndex >= 2)
                nextSceneName = "Demo_End";

            SceneManager.LoadScene(nextSceneName);
        }
    }
}
