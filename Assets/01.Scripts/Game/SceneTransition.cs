using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField]
    private PlayerPosition playerPos;
    [SerializeField]
    private Vector3 nextPos;

    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private string nextScene;

    public StageNPC stageNpc; // StageNPC 직접 할당
    public NPC npc; // NPC
    bool enter = false;
    //bool open = false;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        if (stageNpc == null && npc == null) // 기본: 미술관장 등 NPC null
        {
            LoadNextScene();
        }
        else if (stageNpc != null)
        {
            if (SceneManager.GetActiveScene().name == "Museum_Lobby") // 미술관장
            {
                if (!stageNpc.isTutoFin)
                {
                    if (!enter)
                    {
                        enter = true;
                        string message = "미술관을 구경하고 싶은 그대의 마음은 알겠지만, 우선 아래에 있는 조각상부터 해봐주세요!";

                        DialogueManager dm = FindObjectOfType<DialogueManager>();
                        dm.ShowMessage(message, "미술관장");
                    }
                }
                else
                {
                    LoadNextScene();
                }
            }
        }
        else if (npc != null) // 그 외 상호작용 ** 열쇠 상호작용은 NPC 시스템 활용
        {
            if (SceneManager.GetActiveScene().name == "Library_B1F") // 회의실
            {
                if (npc.isInteract)
                {
                    LoadNextScene();
                }
            }
            //if (SceneManager.GetActiveScene().name == "Library_1F" && !InventoryUI.instance.HasItem(20102)) // 도서관 열쇠
            //{
            //    if (!enter)
            //    {
            //        enter = true;
            //        string message = "굳게 잠겨 있다. 열쇠가 어딘가에 있을 것 같은데...";

            //        DialogueManager dm = FindObjectOfType<DialogueManager>();
            //        dm.ShowMessage(message);
            //    }
            //}
            //else if (SceneManager.GetActiveScene().name == "Library_B1F")
            //{
            //    if (!InventoryUI.instance.HasItem(20101))
            //    {
            //        if (!enter)
            //        {
            //            enter = true;
            //            string message = "굳게 잠겨 있다. 열쇠를 어디서 구해야 하나...";

            //            DialogueManager dm = FindObjectOfType<DialogueManager>();
            //            dm.ShowMessage(message);
            //        }
            //    }
            //    else
            //    {
            //        if (!open)
            //        {
            //            if (!enter)
            //            {
            //                enter = true;
            //                Debug.Log("enter true");
            //                if (Input.GetKeyDown(KeyCode.F))
            //                {
            //                    Debug.Log("GetKeyDown");
            //                    string message = "철컥. 열쇠를 넣고 돌리자 자물쇠가 열렸다.";

            //                    DialogueManager dm = FindObjectOfType<DialogueManager>();
            //                    dm.ShowMessage(message);
            //                    open = true; // 이거 저장이 안 돼서 다른 맵 이동했다 오면 초기화될 텐데...
            //                    LoadNextScene();
            //                }
            //            }
            //        }
            //        else
            //        {
            //            LoadNextScene();
            //        }
            //    }
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D col) // 트리거 초기화
    {
        if (col.CompareTag("Player"))
        {
            enter = false;
        }
    }

    private void LoadNextScene()
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

        SetUIStateWork(nextScene);

        SceneManager.LoadScene(nextScene);
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

    private void SetUIStateWork(string sceneName)
    {
        if (sceneName == "Museum_Lobby" || sceneName == "Library_1F" || sceneName == "Park" || 
            sceneName == "CityHall_Lobby" || sceneName == "Broadcast_1F" || sceneName == "Hospital_1F")
        {
            if(SceneManager.GetActiveScene().name == "GlobalMap" && sceneName == "Museum_Lobby")
            {
                Debug.Log("삭제");
                GetComponent<DeleteAllData>().DeleteAllJsonFiles();
                if (FieldItemManager.Instance != null)
                {
                    FieldItemManager.Instance.ResetFieldItems(); // 필드 아이템 관련 데이터 삭제
                }
            }
            if (UIManager.u_instance != null)
            {
                UIManager.u_instance.SetUIState(Define.UI.UIState.Work);    // 임시
            }
        }
    }
}
