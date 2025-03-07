using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Statue : NPC
{
    StatueAudio statueAudio;

    public SpriteRenderer spriteRenderer;
    public Sprite destroyedSprite; // 무너진 조각상 스프라이트

    public bool isStatue = false;
    public bool isChecked = false;
    public bool isJudged = false;
    public bool isEnemy = false;
    public bool isCorrect = false;

    public bool isFin = false;
    public bool result = false;

    public bool test1;
    public bool test2;
    public bool test3;
    public bool test4;

    public bool isSpriteChanged = false;

    protected override void Awake()
    {
        base.Awake();
        LoadNPCData();
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
                            Debug.Log("기본대사 -> 판별");
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
    }

    public void Judge()
    {
        //Debug.Log("Judge 넘어감");
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.StartsWith("Museum"))
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
                if (!isFin) // 판별 완전 종료 전
                {
                    if (isEnemy) // 적
                    {
                        if (isCorrect) // 적, 정답
                        {
                            if (!test2)
                            {
                                Debug.Log("건드린다 > 정답");
                                isCorrect = true;
                                statueAudio.PlayEnterFight();
                                statueScore.fightCount += 1;
                                statueScore.SaveScore();
                                ChangeDialogueExplain(_FileIndex, "1");
                                test2 = true;
                                //Debug.Log("test2 True");
                                StartCoroutine(DelayLoadScene(2.2f, "Battle"));
                            }
                        }
                        else // 적, 오답
                        {
                            Debug.Log("이상 없음 > 오답");
                            if (!test1 && !test2) // 전투 최초 진입
                            {
                                statueAudio.PlayPencil();
                                ChangeDialogueExplain(_FileIndex, "1");
                                StartCoroutine(PlaySound());
                                statueScore.fightCount += 1;
                                statueScore.SaveScore();
                                test1 = true;
                                test2 = true;
                                //SaveNPCData();
                                //Debug.Log("오답 최초 진입 test1, 2 True");
                            }
                            else if (test1 && !test2) // 전투 재진입
                            {
                                ChangeDialogueExplain(_FileIndex, "1");
                                StartCoroutine(PlaySound());
                                statueScore.fightCount += 1;
                                statueScore.SaveScore();
                                test2 = true;
                                //Debug.Log("오답 재진입 test2 True");
                                //SaveNPCData();
                            }

                            isCorrect = false;
                            SaveNPCData();
                            //Debug.Log("전투 진입 전 NPCData 저장");
                        }
                    }
                    else // 적 아님
                    {
                        if (isCorrect) // 적 아님, 정답
                        {
                            Debug.Log("이상 없음 > 정답");
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
                            SaveNPCData();
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
                            SaveNPCData();
                        }
                    }
                }
                else // 판별 종료(isFin)
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
                                ChangeDialogueFileName("Destroyed_dialogue");
                                //_FileIndex = 4;
                                //currentIndex = 4;
                                //explainNum = "1";
                                //ChangeDialogueExplain(_FileIndex, "1");
                                //Debug.Log("!isEnemy !isCorrect");
                            }
                        }
                    }
                    else // 적
                    {
                        if (!result) // 무너져 내린다 출력
                        {
                            ChangeDialogueFile(5);
                            Debug.Log("!result");

                            ChangeSprite();
                            //result = true;
                            if (!test4 && isEnemy)
                            {
                                statueAudio.PlayDestroyed();
                                statueScore.statueCount += 1;
                                statueScore.SaveScore();
                                StartCoroutine(TriggerDialogue(2f));

                                //ChangeDialogueFile(5);
                                //SaveNPCData();
                                //StartCoroutine(TriggerDialogue(0.1f));
                                //WinDialogue();

                                test4 = true;
                                StartCoroutine(DelayResult());
                                //result = true;
                                //SaveNPCData();
                            }
                        }
                        else // 무너져 있다 출력
                        {
                            //Debug.Log("result Destroyed");
                            ChangeDialogueFileName("Destroyed_dialogue");
                        }
                    }
                }
            }
        }
    }

    public void ChangeDialogueExplain(int _currentIndex, string _explainNum)
    {
        currentIndex = _currentIndex;
        explainNum = _explainNum;
        if (currentIndex < dialogueFiles.Length - 1)
        {
            dialogueFileName = dialogueFiles[currentIndex];
            selectFileName = selectFiles[currentIndex];
            currentName = dialogueFileName;
            //Debug.Log("대화: " + dialogueFileName + ", 선지: " + selectFileName);
            if (isJudged && ((!isCorrect && !isEnemy) || (isCorrect && isEnemy) || (!isCorrect && isEnemy)) && !isFin)
            {
                StartCoroutine(TriggerDialogue(0.1f));
            }
        }
    }

    IEnumerator TriggerDialogue(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isEnemy && isFin)
        {
            ChangeDialogueFileName("stage1_exhibit2_Item");
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
        //ChangeDialogueFile(5);
        //SaveNPCData();
        //WinDialogue();

        yield return new WaitForSeconds(4f);

        //test4 = true;
        result = true;
        SaveNPCData();
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

    public new void SaveNPCData()
    {
        npcData.isChecked = isChecked;
        npcData.isJudged = isJudged;
        npcData.isCorrect = isCorrect;
        npcData.isFin = isFin;
        npcData.result = result;
        npcData.isSpriteChanged = isSpriteChanged;
        npcData.test1 = test1;
        npcData.test2 = test2;
        npcData.test3 = test3;
        npcData.test4 = test4;

        base.SaveNPCData();
    }

    public new void LoadNPCData()
    {
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
        test2 = npcData.test2;
        test3 = npcData.test3;
        test4 = npcData.test4;

        base.LoadNPCData();
    }
}

//public JudgeState judgeState;
//public enum JudgeState
//{
//    Normal,
//    Judging,
//    Destroyed
//}

//private void Update()
//{
//    switch (judgeState)
//    {
//        case JudgeState.Normal:
//            Normal();
//            break;
//        case JudgeState.Judging:
//            Judging();
//            break;
//        case JudgeState.Destroyed:
//            Destroyed();
//            break;
//    }
//}

//void Normal()
//{
//    if (isFin && isCorrect)
//    {
//        ChangeDialogueExplain(3, "3");
//    }
//    else
//    {

//    }
//}

//void Judging()
//{
//    if (isFin) // 판별 및 전투 완료 시
//    {
//        if (!isEnemy && isCorrect)
//        {
//            judgeState = JudgeState.Normal;
//        }
//        else
//        {
//            judgeState = JudgeState.Destroyed;
//        }
//    }
//    else
//    {
//        if (isEnemy)
//        {

//        }
//    }
//}

//void Destroyed()
//{
//    ChangeDialogueFileName("Destroyed_dialogue");
//    ChangeSprite();
//}