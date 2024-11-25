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

    public bool isInteract;
    public bool isTutoDialogueChanged;
    public bool isTutoFin;
}
