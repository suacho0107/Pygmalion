using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] PlayerPosition playerPos;
    [SerializeField] Vector3 nextPos;

    [SerializeField] SceneData sceneData;
    [SerializeField] string nextScene;

    //[SerializeField] GameObject obj3;
    //[SerializeField] GameObject obj4;

    //private void Awake()
    //{
    //    string currentSceneName = SceneManager.GetActiveScene().name;
    //    if (currentSceneName == "Museum_ExhibitionRoom2")
    //    {
    //        int sceneIndex = GetSceneIndex(currentSceneName);
    //        if (sceneIndex != -1 && sceneData.scenes[sceneIndex].visitCount == 2)
    //        {
    //            Vector3 tempPos = obj3.transform.position;
    //            obj3.transform.position = obj4.transform.position;
    //            obj4.transform.position = tempPos;
    //        }
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            // Scriptable Object에 nextPos 저장
            playerPos.nextPosition = nextPos;
            playerPos.isChecked = true;

            // 방문 횟수 증가
            int sceneIndex = GetSceneIndex(nextScene);
            if (sceneIndex != -1)
            {
                sceneData.scenes[sceneIndex].visitCount++;
                Debug.Log($"현재 씬: {nextScene} / 방문 횟수: {sceneData.scenes[sceneIndex].visitCount}");
            }
            else
            {
                Debug.LogError("SceneData not found for scene: " + nextScene);
            }

            SceneManager.LoadScene(nextScene);
        }
    }

    private int GetSceneIndex(string sceneName)
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
