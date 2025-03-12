using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageNPC : NPC
{
    bool isNPC = true;
    public bool tutorial = false;

    public bool isTutoDialogueChanged = false;
    public bool isTutoFin = false;
    //bool isDialogueChanged = false;

    protected override void Awake()
    {
        base.Awake();
        LoadNPCData();
    }

    private void Update()
    {
        #region Tutorial NPC
        if (tutorial && csv != null)// 미술관장 tutorial V
        {
            // 미술관장과의 첫 대화가 끝나면 isInteract == true;
            if (isInteract)
            {
                if (!isTutoDialogueChanged)
                {
                    csv.npcs[0].ChangeDialogueFile(1); // 조각상(npcs[0])의 대화 파일 변경
                    isTutoDialogueChanged = true;
                    SaveNPCData();
                }

                if (isTutoDialogueChanged)
                {
                    if (!isTutoFin)
                    {
                        if (statueScore.statueCount == 0)
                        {
                            ChangeDialogueFileName("Check0_dialogue");
                        }
                        else if (statueScore.statueCount == 1)
                        {
                            ChangeDialogueFileName("Tutorial2_dialogue");
                        }
                        else if (statueScore.statueCount > 1 && statueScore.statueCount < 6) // 튜토 진행 안내 없을 시 튜토 마무리 대화 생략
                        {
                            ChangeDialogueFileName("Check2_dialogue");
                        }
                        else if (statueScore.statueCount >= 6)
                        {
                            ChangeDialogueFileName("Check3_dialogue");
                        }
                    }
                    else
                    {
                        if (statueScore.statueCount == 1)
                        {
                            ChangeDialogueFileName("Check1_dialogue");
                        }
                        else if (statueScore.statueCount > 1 && statueScore.statueCount < 6)
                        {
                            ChangeDialogueFileName("Check2_dialogue");
                        }
                        else if (statueScore.statueCount >= 6)
                        {
                            ChangeDialogueFileName("Check3_dialogue");
                        }
                    }
                }
            }
        }
        #endregion

        #region Library Guard
        if (SceneManager.GetActiveScene().name == "Library_1F" && isNPC) // 도서관 1층 경비원
        {
            if (isInteract)
            {
                ChangeDialogueFileName("Guard_Check0_dialogue");

                if (statueScore.statueCount == 1)
                {
                    ChangeDialogueFileName("Guard_Check1_dialogue");
                }
                else if (statueScore.statueCount > 1 && statueScore.statueCount < 5)
                {
                    ChangeDialogueFileName("Guard_Check2_dialogue");
                }
                else if (statueScore.statueCount == 5)
                {
                    ChangeDialogueFileName("Guard_Check3_dialogue");
                }
            }
        }
        #endregion

        #region Library Librarian

        #endregion
    }

    public new void SaveNPCData()
    {
        npcData.isTutoDialogueChanged = isTutoDialogueChanged;
        npcData.isTutoFin = isTutoFin;

        base.SaveNPCData();
    }

    public new void LoadNPCData()
    {
        Debug.Log(gameObject.name + "StageNPC LoadData");
        isTutoDialogueChanged = npcData.isTutoDialogueChanged;
        isTutoFin = npcData.isTutoFin;

        base.LoadNPCData();
    }
}
