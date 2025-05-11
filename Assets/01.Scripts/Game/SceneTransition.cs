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

    public StageNPC stageNpc; // StageNPC 직접 할당
    public NPC npc; // NPC
    bool enter = false;
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (stageNpc == null && npc == null) // 기본: 미술관장 등 NPC null
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
            else if (stageNpc != null && !stageNpc.isTutoFin) // 미술관장
            {
                if (!enter)
                {
                    enter = true;
                    string message = "미술관을 구경하고 싶은 그대의 마음은 알겠지만, 우선 아래에 있는 조각상부터 해봐주세요!";

                    DialogueManager dm = FindObjectOfType<DialogueManager>();
                    dm.ShowMessage(message, "미술관장");
                }
            }
            else if (npc != null) // 그 외 상호작용
            {
                if (SceneManager.GetActiveScene().name == "Library_1F" && !InventoryUI.instance.HasItem(20102)) // 도서관 열쇠
                {
                    if (!enter)
                    {
                        enter = true;
                        string message = "굳게 잠겨 있다. 열쇠가 어딘가에 있을 것 같은데...";

                        DialogueManager dm = FindObjectOfType<DialogueManager>();
                        dm.ShowMessage(message);
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) // 트리거 초기화
    {
        if (collision.CompareTag("Player"))
        {
            enter = false;
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
                UIManager.u_instance.SetUIState(Define.UI.UIState.Work);
            }
        }
    }
}
