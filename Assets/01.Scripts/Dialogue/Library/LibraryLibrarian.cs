using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LibraryLibrarian : StageNPC
{
    public GameObject tremble;
    public GameObject idle;

    public librarianState LState;

    public enum librarianState
    {
        Tremble,
        MelEnd1,
        MelEnd2,
        AllEnd1,
        AllEnd2
    }

    // 수아: 가급적 public 함수들은 위로, private 함수들은 아래로 숨겨주세요
    private bool trg1 = false;
    private bool trg2 = false;
    private string sceneName = "";

    protected override void Awake()
    {
        base.Awake();
        LoadStageNPCData();
        dialogueManager = FindObjectOfType<DialogueManager>();
        dialogueUI = FindObjectOfType<DialogueUI>();
    }

    private void Start()
    {
        LState = librarianState.Tremble;
        sceneName = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        switch (LState)
        {
            case librarianState.Tremble:
                LStateTremble();
                break;
            case librarianState.MelEnd1:
                LStateMelEnd1();
                break;
            case librarianState.MelEnd2:
                LStateMelEnd2();
                break;
            case librarianState.AllEnd1:
                LStateAllEnd1();
                break;
            case librarianState.AllEnd2:
                LStateAllEnd2();
                break;
        }

        /* (확인하시면 지워주세요)
         수아 : Update에서 SceneManager.GetActiveScene()로 매번 씬 이름을 가져오는건 불필요한 호출입니다.
         Start에서 한번만 가져와서 조건문 비교할 땐 String 값으로만 비교하는게 비용적 측면에서 이득입니다.
         */
        if (sceneName == "Library_2F")
        {
            Statue mel = (csv.npcs[0] as Statue); // 멜포메네
            string transPath = Application.persistentDataPath + "/stage2_statue 5_data.json"; // 책읽는아이

            if (!mel.result) LState = librarianState.Tremble;
            else
            {
                gameObject.layer = 6;
                if (!questStart) LState = librarianState.MelEnd1;
                else LState = librarianState.MelEnd2;

                if (File.Exists(transPath))
                {
                    string json = File.ReadAllText(transPath);
                    NPCData childData = JsonUtility.FromJson<NPCData>(json);

                    if (childData.result)
                    {
                        if (!questEnd) LState = librarianState.AllEnd1;
                        else LState = librarianState.AllEnd2;
                    }
                }
            }
        }
    }

    void LStateTremble()
    {
        tremble.SetActive(true);
        idle.SetActive(false);

        if (!isInteract)
        {
            dialogueFileName = "Library-Librarian0_dialogue";
            selectFileName = "Library-Librarian0_select";
        }
        else // 재상호작용 시 반응 없게
        {
            gameObject.layer = 0;
            dialogueFileName = "";
            selectFileName = "";
        }

    }

    void LStateMelEnd1()
    {
        tremble.SetActive(false);
        idle.SetActive(true);

        //Debug.Log("melEnd1");
        ChangeDialogueFile(1);
        if (!trg1 && dialogueUI.lineCount == 7 && dialogueManager.CurrentNPC == this)
        {
            questStart = true;
            SaveStageNPCData();
            trg1 = true;
        }
        Animator anim = GetComponent<Animator>();
        anim.SetBool("melEnd", true);
    }

    void LStateMelEnd2()
    {
        tremble.SetActive(false);
        idle.SetActive(true);

        //Debug.Log("melEnd2");
        ChangeDialogueFile(2);
    }

    void LStateAllEnd1()
    {
        tremble.SetActive(false);
        idle.SetActive(true);

        //Debug.Log("allEnd1");
        ChangeDialogueFile(3);

        if (!trg2 && dialogueManager.isEnd && dialogueManager.CurrentNPC == this)
        {
            questEnd = true;
            SaveStageNPCData();
            trg2 = true;
        }
    }

    void LStateAllEnd2()
    {
        tremble.SetActive(false);
        idle.SetActive(true);

        //Debug.Log("allEnd2");
        ChangeDialogueFile(4);
    }
}
