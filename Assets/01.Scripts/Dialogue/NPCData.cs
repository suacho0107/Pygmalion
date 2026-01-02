using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCData
{
    public bool isChecked;
    public bool isJudged;
    public bool isCorrect;
    public bool isDialogueChanged;
    public int currentIndex;
    public string dialogueFileName;
    public string selectFileName;

    public bool isFin;
    public bool result;
    public bool isSpriteChanged;
    public bool enter1st;
    public bool enterFight;
    public bool trg_nEnmy;
    public bool trg_destroyed;

    public bool isInteract;
    public bool isTutoDialogueChanged;
    public bool isTutoFin;
    public bool questStart;
    public bool questEnd;
    public bool unlock;
    //public string judgeState;
}
