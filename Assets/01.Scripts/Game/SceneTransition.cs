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

    [SerializeField] TutorialFadeEffect fadeEffect;

    public StageNPC stageNpc; // StageNPC 직접 할당
    public NPC npc; // NPC
    bool enter = false;

    void LoadNextScene()
    {
        playerPos.nextPosition = nextPos;
        playerPos.isChecked = true;

        Set_UIStateWork(nextScene);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (stageNpc == null && npc == null) // 기본: 미술관장 등 NPC null
            {
                playerPos.nextPosition = nextPos;
                playerPos.isChecked = true;

                //Set_UIStateWork(nextScene);

                LoadNextScene();
            }
            else if (stageNpc != null)
            {
                if (SceneManager.GetActiveScene().name == "Museum_Lobby") // 미술관 로비 미술관장
                {
                    if (!stageNpc.isTutoFin)
                    {
                        if (!enter)
                        {
                            enter = true;
                            string message = "미술관을 구경하고 싶은 그대의 마음은 알겠지만, 우선 아래에 있는 조각상부터 해봐주세요!";

                            DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
                            dialogueUI.ShowMessage(message, "미술관장");
                        }
                    }
                    else
                    {
                        LoadNextScene();
                    }
                }
            }
            else if (npc != null) // 그 외 상호작용
            {
                if (SceneManager.GetActiveScene().name == "Library_B1F") // 도서관 B1 열람실
                {
                    if(!enter)
                    {
                        enter = true;
                    }
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (fadeEffect != null)
        {
            if (fadeEffect.isCompleted)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
        else
            SceneManager.LoadScene(nextScene);
    }

    private void OnTriggerExit2D(Collider2D col) // 트리거 초기화
    {
        if (col.CompareTag("Player"))
        {
            enter = false;
        }
    }

    private void Update()
    {
        //Check_FadeOut();
        Update_Library();
    }

    void Check_FadeOut()
    {
        if (fadeEffect != null)
        {
            if (fadeEffect.isCompleted)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
        else
            SceneManager.LoadScene(nextScene);
    }

    void Update_Library()
    {
        if (npc != null && SceneManager.GetActiveScene().name == "Library_B1F") // 도서관 B1 열람실
        {
            if (!InventoryUI.instance.HasItem(20101))
            {
                npc.isInteract = false;
            }
            else
            {// 최초 상호작용 시 isInteract, 대사 출력
                if (enter && Input.GetKeyDown(KeyCode.F) && !npc.isInteract)
                {
                    npc.isInteract = true;
                }
            }

            if (npc.isInteract && enter)
            {
                LibraryRoom libRoom = FindObjectOfType<LibraryRoom>();
                if (!libRoom.unlock) // 최초 상호작용 시 대사 출력
                {
                    DialogueManager dm = FindObjectOfType<DialogueManager>();
                    if (dm.isEnd)
                    {
                        LoadNextScene();
                    }
                }
                else // 최초 상호작용 이후 대사 출력 없이 이동
                {
                    LoadNextScene();
                }
            }
        }
    }

    private void Set_UIStateWork(string sceneName)
    {
        if (sceneName == "Museum_Lobby" || sceneName == "Library_1F" || sceneName == "Park" || 
            sceneName == "CityHall_Lobby" || sceneName == "Broadcast_1F" || sceneName == "Hospital_1F")
        {
            if(SceneManager.GetActiveScene().name == "GlobalMap" && sceneName == "Museum_Lobby")
            {
                //Debug.Log("삭제");
                GetComponent<DeleteAllData>().DeleteAllJsonFiles();
                if (FieldItemManager.Instance != null)
                {
                    FieldItemManager.Instance.ResetFieldItems(); // 필드 아이템 관련 데이터 삭제
                }
            }
            if (UIManager.u_instance != null) /* to be moved */
            {
                //UIManager.u_instance.Set_UIState(Define.UI.UIState.Work); /* to be deleted */

                /* to be moved -> DialogueUI: 여기에서 하는 건 좀 아닌가 */
                if (sceneName == "Museum_Lobby")
                    UIManager.u_instance.Set_StageState(Define.Stage.StageState.Museum);

                else if (sceneName == "Library_1F")
                    UIManager.u_instance.Set_StageState(Define.Stage.StageState.Library);

                else if (sceneName == "Park")
                    UIManager.u_instance.Set_StageState(Define.Stage.StageState.Park);

                else if (sceneName == "CityHall_Lobby")
                    UIManager.u_instance.Set_StageState(Define.Stage.StageState.CityHall);

                else if (sceneName == "Broadcast_1F")
                    UIManager.u_instance.Set_StageState(Define.Stage.StageState.BroadcastStation);

                else if (sceneName == "Hospital_1F")
                    UIManager.u_instance.Set_StageState(Define.Stage.StageState.Hospital);
            }
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
