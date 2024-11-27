using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public InteractionEvent interactionEvent; // 이 NPC와 연결된 InteractionEvent
    public MuseumLobbyCSV csv; // A와 상호작용 종료 시 B 대화 파일 변경, csv 파일 목록 한번에 관리하는 스크립트로 변경할 수 있을까?
    public StatueScore statueScore;
    //public StatueController statueController;

    public bool isStatue = false;
    public bool isChecked;
    public bool isJudged = false;
    public bool isEnemy = false;
    public bool isCorrect;
    public bool tutorial = false;
    public bool isInteract = false;

    bool isTutoDialogueChanged = false;
    bool isTutoFin = false;
    bool isDialogueChanged = false;
    bool isFin = false;
    int lastCount = -1;
    
    [SerializeField] public string dialogueFileName;
    [SerializeField] public string selectFileName;
    [SerializeField] public string explainNum;
    [SerializeField] public string[] dialogueFiles; // 파일 변경 배열 추가
    [SerializeField] public string[] selectFiles;
    public int currentIndex = 0;

    private void Start()
    {
        if(isStatue)
        {
            isChecked = false;
        }
        //PlayerPrefs.DeleteAll();
        isInteract = PlayerPrefs.GetInt("isInteract", 0) == 1;
        isTutoDialogueChanged = PlayerPrefs.GetInt("isTutoDialogueChanged", 0) == 1;
        isTutoFin = PlayerPrefs.GetInt("isTutoFin", 0) == 1;
        lastCount = PlayerPrefs.GetInt("lastCount", lastCount);
    }
    private void Update()
    {
        if(tutorial && csv != null)// 미술관장 tutorial V
        {
            // 미술관장과의 첫 대화가 끝나면 isInteract == true;
            if(isInteract)
            {
                if (!isTutoDialogueChanged)
                {
                    csv.npcs[0].ChangeDialogueFile(); // 조각상(npcs[0])의 대화 파일 변경
                    isTutoDialogueChanged = true;
                    SaveTuto();
                }

                // 조각상 판별 개수에 따라 대화 파일 변경
                if (isTutoDialogueChanged && !isTutoFin && statueScore.statueCount != lastCount)
                {
                    // 아예 여기로 안 넘어오는 듯
                    lastCount = statueScore.statueCount;

                    if (lastCount == 1)
                    {
                        ChangeDialogueFile();
                    }
                    else if (lastCount > 1 && lastCount < 6)
                    {
                        ChangeDialogueFile();
                    }
                    else if (lastCount == 6)
                    {
                        ChangeDialogueFile();
                        isTutoFin = true;
                        SaveTuto();
                    }
                }
            }
        }

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
                else if (!isEnemy && isJudged && !isFin)
                {
                    if (isCorrect)
                    {// 이상 없음 --> 정답 --> 기록 효과~ --> count++
                        Debug.Log("이상 없음 > 정답");
                        //StatueScore statueScore = FindObjectOfType<StatueScore>();
                        statueScore.statueCount += 1;
                        statueScore.SaveScore();
                        isFin = true;
                    }
                    else
                    {// 건드린다 --> 오답 --> 조각상이 힘없이 무너져내린다... --> statueState.Destroyed
                        Debug.Log("건드린다 > 오답");
                        ChangeDialogueFile("2");
                        statueScore.statueCount += 1;
                        statueScore.destroyedCount += 1;
                        statueScore.SaveScore();
                        isFin = true;
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
                //Debug.Log("대화: " + dialogueFileName + ", 선지: " + selectFileName);

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

    public void SaveTuto()
    {
        PlayerPrefs.SetInt("isInteract", isInteract ? 1 : 0);
        PlayerPrefs.SetInt("isDialogueChanged", isTutoDialogueChanged ? 1 : 0);
        PlayerPrefs.SetInt("isTutoFin", isTutoFin ? 1 : 0);
        PlayerPrefs.Save();
    }
}
