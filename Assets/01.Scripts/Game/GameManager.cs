using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Define;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 전시실2 조각상 위치 변경 스크립트
    /// </summary>

    [SerializeField] SceneData sceneData;

    [SerializeField] GameObject statue_3;
    [SerializeField] GameObject statue_4;

    private void Awake()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        //if (currentSceneName == "Museum_ExhibitionRoom2")
        //{
        //    int sceneIndex = GetSceneIndex(currentSceneName);
        //    if (sceneIndex != -1 && sceneData.scenes[sceneIndex].visitCount >= 2)
        //    {
        //        Vector3 tempPos = statue_3.transform.position;
        //        statue_3.transform.position = statue_4.transform.position;
        //        statue_4.transform.position = tempPos;
        //    }
        //}
    }

    private void Start()
    {
        UpdateStageState();
    }

    private void UpdateStageState()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Stage.StageState newState;

        if (currentSceneName == "Museum_Lobby")
        {
            newState = Stage.StageState.Museum;
        }
        else if (currentSceneName == "GlobalMap")
        {
            newState = Stage.StageState.Global;
        }
        else if (currentSceneName == "Company_Office")
        {
            newState = Stage.StageState.Company;
        }
        else
        {
            newState = Stage.StageState.Main;
        }

        SoundManager.Instance.SetStageBGM(newState);
    }

    int GetSceneIndex(string sceneName)
    {
        for (int i = 0; i < sceneData.scenes.Length; i++)
        {
            if (sceneData.scenes[i].sceneName == sceneName)
            {
                return i;
            }
        }
        return -1;
    }

}
