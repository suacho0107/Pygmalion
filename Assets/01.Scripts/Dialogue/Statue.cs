//using System.Collections;
//using System.Collections.Generic;
//using System.Net.Sockets;
//using UnityEditor.ShaderGraph;
//using UnityEngine;

//public class Statue : NPC
//{
//    public MuseumLobbyCSV csv;
//    public StatueScore statueScore;
//    public NPCData npcData = new NPCData();
//    //public FightDataTest fightData;

//    public SpriteRenderer spriteRenderer;
//    public Sprite destroyedSprite; // 무너진 조각상 스프라이트

//    StatueAudio statueAudio;

//    public bool isStatue = false;
//    public bool isNPC;
//    public bool isChecked = false;
//    public bool isJudged = false;
//    public bool isEnemy = false;
//    public bool isCorrect = false;
//    public bool tutorial = false;
//    public bool isInteract = false;

//    public bool isTutoDialogueChanged = false;
//    public bool isTutoFin = false;
//    bool isDialogueChanged = false;
//    public bool isFin = false;
//    public bool result = false;

//    bool test1;
//    bool test2;
//    bool test3;
//    bool test4;

//    public bool isSpriteChanged = false;

//    string filePath;

//    [SerializeField] public string[] dialogueFiles; // 파일 변경 배열 추가
//    [SerializeField] public string[] selectFiles;
//    public int currentIndex = 0;

//    public JudgeState judgeState;
//    public enum JudgeState
//    {
//        Normal,
//        Judging,
//        Destroyed
//    }

//    private void Update()
//    {
//        switch (judgeState)
//        {
//            case JudgeState.Normal:
//                Normal();
//                break;
//            case JudgeState.Judging:
//                Judging();
//                break;
//            case JudgeState.Destroyed:
//                Destroyed();
//                break;
//        }
//    }

//    void Normal()
//    {
//        if(isFin && isCorrect)
//        {
//            ChangeDialogueExplain(3, "3");
//        }
//        else
//        {

//        }
//    }

//    void Judging()
//    {
//        if(isFin) // 판별 및 전투 완료 시
//        {
//            if(!isEnemy && isCorrect)
//            {
//                judgeState = JudgeState.Normal;
//            }
//            else
//            {
//                judgeState = JudgeState.Destroyed;
//            }
//        }
//        else
//        {
//            if (isEnemy)
//            {

//            }
//        }
//    }

//    void Destroyed()
//    {
//        ChangeDialogueFileName("Destroyed_dialogue");
//        ChangeSprite();
//    }
//}
