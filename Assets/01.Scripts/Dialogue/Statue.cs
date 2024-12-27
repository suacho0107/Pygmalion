using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : NPC
{
    public MuseumLobbyCSV csv;
    public StatueScore statueScore;
    public NPCData npcData = new NPCData();
    //public FightDataTest fightData;

    public SpriteRenderer spriteRenderer;
    public Sprite destroyedSprite; // 무너진 조각상 스프라이트

    StatueAudio statueAudio;

    public bool isStatue = false;
    public bool isNPC;
    public bool isChecked = false;
    public bool isJudged = false;
    public bool isEnemy = false;
    public bool isCorrect = false;
    public bool tutorial = false;
    public bool isInteract = false;

    public bool isTutoDialogueChanged = false;
    public bool isTutoFin = false;
    bool isDialogueChanged = false;
    public bool isFin = false;
    public bool result = false;

    bool test1;
    bool test2;
    bool test3;
    bool test4;

    public bool isSpriteChanged = false;

    string filePath;

    [SerializeField] public string[] dialogueFiles; // 파일 변경 배열 추가
    [SerializeField] public string[] selectFiles;
    public int currentIndex = 0;
}
