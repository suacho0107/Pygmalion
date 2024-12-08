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

            UpdateUIManagerState(nextScene);

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

    private void UpdateUIManagerState(string sceneName)
    {
        if (sceneName == "Museum_Lobby" || sceneName == "Library_1F" || sceneName == "Park" || 
            sceneName == "CityHall_Lobby" || sceneName == "Broadcast_1F" || sceneName == "Hospital_1F")
        {
            if(sceneName == "Museum_Lobby" && SceneManager.GetActiveScene().name == "GlobalMap")
            {
                GetComponent <DeleteAllData>().DeleteAllJsonFiles();
            }

            if (UIManager.u_instance != null)
            {
                UIManager.u_instance.SetUIState(Define.UI.UIState.Work);
            }
        }
    }
}
