using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public InteractionEvent interactionEvent; // 이 NPC와 연결된 InteractionEvent
    //public StatueController statueController;

    public bool isStatue = false;
    public bool isChecked;
    public bool isJudged = false;
    public bool isEnemy = false;
    public bool isCorrect;

    bool isDialogueChanged = false;

    [SerializeField] public string dialogueFileName;
    [SerializeField] public string selectFileName;
    [SerializeField] public string explainNum;
    [SerializeField] public string[] dialogueFiles; // 파일 변경 배열 추가
    [SerializeField] public string[] selectFiles;
    public int currentIndex = 0;

    //public bool isChecked = false;

    //private void OnMouseDown()
    //{
    //    StartDialogue();
    //}
    //private void Start()
    //{
    //    dialogueFiles = new string[] { "Tutorial1_dialogue", "Tutorial2_dialogue", "Check1_dialogue", "Check2_dialogue", "Check3_dialogue" };
    //    selectFiles = new string[] { "Tutorial1_select", "", "", "", ""};
    //    dialogueFileName = dialogueFiles[currentIndex];
    //    selectFileName = selectFiles[currentIndex];
    //}

    private void Start()
    {
        if(isStatue)
        {
            isChecked = false;
        }
    }
    private void Update()
    {
        //if (isChecked)
        //{
        //    ChangeDialogueFile();
        //}
        if (isStatue && isChecked)
        {
            if (!isDialogueChanged)
            {
                ChangeDialogueFile();
                isDialogueChanged = true;
            }
            else
            {
                if (isEnemy && isJudged)
                {
                    if (isCorrect)
                    {// 건드린다 --> 정답 --> battleDialogue.csv --> 전투 진입(플레이어 선공)
                        Debug.Log("건드린다 > 정답");
                        ChangeDialogueFile("1");
                        StartCoroutine(DelayLoadScene(1.5f, "Demo_minjoo"));
                    }
                    else
                    {// 이상 없음 --> 오답 --> 기록 효과~ --> 전투 진입(적 선공)
                        Debug.Log("이상 없음 > 오답");
                        SceneManager.LoadScene("Demo_minjoo");
                    }
                }
                else if (!isEnemy && isJudged)
                {
                    if (isCorrect)
                    {// 이상 없음 --> 정답 --> 기록 효과~ --> count++
                        Debug.Log("이상 없음 > 정답");
                    }
                    else
                    {// 건드린다 --> 오답 --> 조각상이 힘없이 무너져내린다... --> statueState.Destroyed
                        Debug.Log("건드린다 > 오답");
                        ChangeDialogueFile("2");
                        //statueController.sState = statueController.StatueState.Destroyed;
                    }
                }
            }
        }
    }

    public void StartDialogue()
    {
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.SetNPC(this);
        }
        else //null 처리
        {
            Debug.LogError("DialogueManager is null.");
        }

        InteractionEvent interactionEvent = GetComponent<InteractionEvent>();
        if (interactionEvent != null)
        {

            if (!string.IsNullOrEmpty(explainNum)) //explainNum 있으면 전달
            {
                interactionEvent.LoadDialogue(dialogueFileName, explainNum);
            }
            else //explainNum 없으면 그냥
            {
                interactionEvent.LoadDialogue(dialogueFileName);
            }
        }
    }

    void ChangeDialogueFile(string _explainNum = null)
    {
        if (string.IsNullOrEmpty(_explainNum))
        {
            if (currentIndex < dialogueFiles.Length - 1)
            {
                currentIndex++;
                dialogueFileName = dialogueFiles[currentIndex];
                selectFileName = selectFiles[currentIndex];
                Debug.Log("대화: " + dialogueFileName + ", 선지: " + selectFileName);
            }
        }
        else
        {
            explainNum = _explainNum;
            if (currentIndex < dialogueFiles.Length - 1)
            {
                currentIndex++;
                dialogueFileName = dialogueFiles[currentIndex];
                selectFileName = selectFiles[currentIndex];
                Debug.Log("대화: " + dialogueFileName + ", 선지: " + selectFileName);

                StartCoroutine(TriggerDialogue());
            }
        }
    }

    IEnumerator TriggerDialogue()
    {
        yield return new WaitForSeconds(0.1f);
        StartDialogue();
    }

    IEnumerator DelayLoadScene(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
