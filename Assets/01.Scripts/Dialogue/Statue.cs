using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.Progress;

public class Statue : NPC
{
    StatueAudio statueAudio;

    public SpriteRenderer spriteRenderer;
    public Sprite destroyedSprite; // 무너진 조각상 스프라이트

    public bool isStatue = true;
    public bool isChecked = false;
    public bool isJudged = false;
    public bool isEnemy = false;
    public bool isCorrect = false;

    public bool isFin = false;
    public bool result = false;

    public bool test1;
    public bool enterFight;
    public bool test3;
    public bool test4;

    public bool isSpriteChanged = false;

    public int _FileIndex;

    protected override void Awake()
    {
        base.Awake();
        LoadStatueData();
    }

    private void Start()
    {
        statueAudio = GetComponent<StatueAudio>();
    }

    private void Update()
    {
        if (isStatue)
        {
            if (SceneManager.GetActiveScene().name == "Museum_Lobby")
            {
                Judge();
            }
            else
            {
                if (statueScore != null)
                {
                    string sceneName = SceneManager.GetActiveScene().name;

                    if (sceneName.StartsWith("Museum"))
                    {
                        if (statueScore.statueCount >= 1 && !isChecked && !isJudged && !isFin)
                        {
                            //Debug.Log("기본대사 -> 판별");
                            ChangeDialogueFile(1);
                            Judge();
                        }
                        else
                        {
                            Judge();
                        }
                    }
                    else if (sceneName.StartsWith("Library"))
                    {
                        Judge();
                    }
                }
            }
        }

        if (isFin)
        {
            Result();
        }
    }


    public void Judge()
    {
        //Debug.Log("Judge 넘어감");
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.StartsWith("Museum")) // && !isChecked && !isJudged)
        {
            _FileIndex = 3;
            if (currentIndex == 1 || currentIndex == 2)
            {
                explainNum = null;
            }
        }
        else
        {
            _FileIndex = 2;
            if (currentIndex == 0 || currentIndex == 1)
            {
                explainNum = null;
            }
        }

        //LoadNPCData();

        if (isStatue && isChecked)
        {
            //Debug.Log("조사 끝난 후 판별로 넘어가야");
            isChecked = true;

            if (!isJudged) // 조사 완료, 판별로 전환
            {
                //Debug.Log("조사 이후 판별");
                ChangeDialogueFile(2);
            }
            else // 판별 선지 선택 후 결과 출력
            {
                if (isEnemy) // 적
                {
                    if (isCorrect) // 적, 정답
                    {
                        if (!enterFight)
                        {
                            Debug.Log("건드린다 > 정답");
                            isCorrect = true;
                            statueAudio.PlayEnterFight();
                            ChangeDialogueExplain(_FileIndex, "1");
                            statueScore.fightCount += 1;
                            statueScore.SaveScore();
                            enterFight = true;
                            //Debug.Log("enterFight True");
                            StartCoroutine(DelayLoadScene(2.2f, "Battle"));
                        }
                    }
                    else // 적, 오답
                    {
                        Debug.Log("이상 없음 > 오답");
                        if (!test1 && !enterFight) // 오답 전투 최초 진입: 기록 효과음 재생
                        {
                            statueAudio.PlayPencil();
                            EnterFight();
                            test1 = true;
                            //Debug.Log("오답 최초 진입 test1, enterFight True");
                        }
                        else if (test1 && !enterFight) // 오답 전투 재진입: 기록 효과음 재생 X
                        {
                            EnterFight();
                            //Debug.Log("오답 재진입 enterFight True");
                        }

                        isCorrect = false;
                        SaveStatueData();
                        //Debug.Log("전투 진입 전 NPCData 저장");
                    }
                }
                else // 적 아님
                {
                    if (isCorrect) // 적 아님, 정답
                    {
                        //Debug.Log("이상 없음 > 정답");
                        if (!test3)
                        {
                            statueAudio.PlayPencil();
                            statueScore.statueCount += 1;
                            statueScore.SaveScore();
                            test3 = true;
                        }
                        ChangeDialogueExplain(_FileIndex, "3");
                        isCorrect = true;
                        isFin = true;
                        SaveStatueData();
                    }
                    else // 적 아님, 오답
                    {
                        if (!test3)
                        {
                            statueAudio.PlayDestroyed();
                            ChangeDialogueExplain(_FileIndex, "2");
                            statueScore.statueCount += 1;
                            statueScore.destroyedCount += 1;
                            statueScore.SaveScore();
                            test3 = true;
                        }
                        Debug.Log("건드린다 > 오답");
                        ChangeSprite();
                        isCorrect = false;
                        isSpriteChanged = true;
                        isFin = true;
                        SaveStatueData();
                    }
                }
            }
        }
    }

    void EnterFight()
    {
        ChangeDialogueExplain(_FileIndex, "1");
        StartCoroutine(PlaySound());
        statueScore.fightCount += 1;
        statueScore.SaveScore();
        enterFight = true;
    }

    void Result()
    {
        if (!isEnemy) // 적 아님, 완전 종료
        {
            if (result)
            {
                if (isCorrect)
                {
                    ChangeDialogueExplain(_FileIndex, "3");
                    //Debug.Log("!isEnemy isCorrect");
                }
                else
                {
                    ChangeDialogueExplain(_FileIndex + 1, "1");
                    //Debug.Log("!isEnemy !isCorrect");
                }
            }
        }
        else // 적
        {
            if (!result) // 무너져 내린다 출력
            {
                ChangeDialogueFile(5);
                //Debug.Log("!result");

                ChangeSprite();
                //result = true;
                if (!test4 && isEnemy)
                {
                    statueAudio.PlayDestroyed();
                    statueScore.statueCount += 1;
                    statueScore.SaveScore();
                    StartCoroutine(TriggerDialogue(2f));

                    test4 = true;
                    StartCoroutine(DelayResult());
                }
            }
            else // 무너져 있다 출력
            {
                ChangeSprite();
                ChangeDialogueExplain(_FileIndex + 1, "1");
            }
        }
    }

    public void CheckResult()
    {
        if (!isEnemy && isFin && !result)
        {
            if (currentIndex == 3)
            {
                Debug.Log("selected Statue currentIndex = 3");
                if (isCorrect == true)
                {
                    //npc.ChangeDialogueExplain(3, "3");
                    result = true;
                    Debug.Log("CheckResult 실행");
                }
                else
                {
                    //npc.ChangeDialogueFileName("Destroyed_dialogue");
                    result = true;
                    Debug.Log("CheckResult 실행");
                }
            }
        }
    }

    public void ChangeDialogueExplain(int _currentIndex, string _explainNum)
    {
        currentIndex = _currentIndex;
        explainNum = _explainNum;

        dialogueFileName = dialogueFiles[currentIndex];
        selectFileName = selectFiles[currentIndex];
        // currentName = dialogueFileName;
        //Debug.Log("대화: " + dialogueFileName + ", 선지: " + selectFileName);
        if (isJudged && ((!isCorrect && !isEnemy) || (isCorrect && isEnemy) || (!isCorrect && isEnemy)) && !isFin)
        {
            StartCoroutine(TriggerDialogue(0.1f));
        }
        //Debug.Log("ChangeDialogueExplain(" + dialogueFileName + ", " + explainNum + ")");
    }

    IEnumerator TriggerDialogue(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isEnemy && isFin)
        {
            explainNum = null;
            ChangeDialogueFileName("stage1_exhibit2_Item");
            InventoryUI.instance.GetAnItem(10301);
            dialogueManager = FindObjectOfType<DialogueManager>();
            dialogueManager.ItemPopup();
        }
        StartDialogue();
        Debug.Log("triggerDialogue");
    }

    IEnumerator DelayLoadScene(float delay, string sceneName)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator DelayResult()
    {

        yield return new WaitForSeconds(4f);

        //test4 = true;
        result = true;
        SaveStatueData();
    }

    IEnumerator PlaySound()
    {
        //if(isEnemy && !isCorrect)
        //{
        //    statueAudio.PlayEnterFight();
        //}

        yield return new WaitForSeconds(1f);
        statueAudio.PlayEnterFight();

        //yield return new WaitForSeconds(2f);
        StartCoroutine(DelayLoadScene(2.2f, "Battle"));
    }

    IEnumerator PlayReEnterSound()
    {
        statueAudio.PlayEnterFight();
        yield return new WaitForSeconds(4f);
        SceneManager.LoadScene("Battle");
    }

    public void ChangeSprite()
    {
        if (spriteRenderer != null && destroyedSprite != null)
        {
            spriteRenderer.sprite = destroyedSprite;
        }
    }

    public void SaveStatueData()
    {
        npcData.isChecked = isChecked;
        npcData.isJudged = isJudged;
        npcData.isCorrect = isCorrect;
        npcData.isFin = isFin;
        npcData.result = result;
        npcData.isSpriteChanged = isSpriteChanged;
        npcData.test1 = test1;
        npcData.enterFight = enterFight;
        npcData.test3 = test3;
        npcData.test4 = test4;

        npcData.isDialogueChanged = isDialogueChanged;
        npcData.currentIndex = currentIndex;
        npcData.dialogueFileName = dialogueFileName;
        npcData.selectFileName = selectFileName;
        npcData.isInteract = isInteract;

        string json = JsonUtility.ToJson(npcData);
        File.WriteAllText(filePath, json);

        //Debug.Log(gameObject.name + " 데이터 저장");
        //Debug.Log("조각상 result: " + result);
    }

    public void LoadStatueData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            npcData = JsonUtility.FromJson<NPCData>(json);

            isDialogueChanged = npcData.isDialogueChanged;
            currentIndex = npcData.currentIndex;
            dialogueFileName = npcData.dialogueFileName;
            selectFileName = npcData.selectFileName;
            isInteract = npcData.isInteract;
            isChecked = npcData.isChecked;
            isJudged = npcData.isJudged;
            isCorrect = npcData.isCorrect;
            isFin = npcData.isFin;
            result = npcData.result;
            isSpriteChanged = npcData.isSpriteChanged;
            if (isSpriteChanged)
            {
                ChangeSprite();
            }
            test1 = npcData.test1;
            enterFight = npcData.enterFight;
            test3 = npcData.test3;
            test4 = npcData.test4;

            Debug.Log(gameObject.name + " 데이터 로드");
        }
    }
}