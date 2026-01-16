using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Define;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] PlayerPosition playerPos;
    [SerializeField] Vector3 nextPos;

    [SerializeField] SceneData sceneData;
    [SerializeField] string nextScene;

    [SerializeField] TutorialFadeEffect fadeEffect;
    [SerializeField] TutorialFadeEffect fadeEffect2;

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
            if (BlockGlobalMap())
                return;

            if (SceneManager.GetActiveScene().name == "Company_Lobby-1" && gameObject.name == "Transition_O")
            {
                if (Define.UI.UIState.Start == UIManager.u_instance.Get_CurrentState())
                {
                    if (UIManager.u_instance.stageIndex == 0)
                    {
                        nextScene = "Company_Office-1";
                    }
                    if (UIManager.u_instance.stageIndex == 1)
                    {
                        nextScene = "Company_Office-2";
                    }
                }
            }

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
                else if(SceneManager.GetActiveScene().name == "Library_1F")
                {
                    //if(!InventoryUI.instance.HasItem(20102)) gameObject.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        if (BlockGlobalMap())
            return;

        if (fadeEffect != null && fadeEffect.gameObject.activeInHierarchy)
        {
            if (fadeEffect.isCompleted)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
        else if (fadeEffect2 != null && fadeEffect2.gameObject.activeInHierarchy)
        {
            if (fadeEffect2.isCompleted)
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

    private bool BlockGlobalMap()
    {
        // UIManager 없으면(초기화 전) 막지 않음
        if (UIManager.u_instance == null) return false;

        // UI 상태가 Work인데 GlobalMap으로 가려는 경우만 차단
        return UIManager.u_instance.Get_CurrentState() == Define.UI.UIState.Work
               && nextScene == "GlobalMap";
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
            if (SceneManager.GetActiveScene().name == "GlobalMap" && (sceneName == "Museum_Lobby" || sceneName == "Library_1F"))
            {
                //GlobalMap 내 트랜지션에 DeleteAllData 넣기
                GetComponent<DeleteAllData>().DeleteInteractJsonFiles();
                if (FieldItemManager.Instance != null)
                {
                    FieldItemManager.Instance.ResetFieldItems(); // 필드 아이템 관련 데이터 삭제
                }
            }
            if (UIManager.u_instance != null) /* to be moved */
            {
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
