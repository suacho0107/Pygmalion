using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LibraryRoom : NPC
{
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Library_1F") // 회의실
        {
            //Debug.Log("SceneManager name");
            if (explainNum != "11")
            {
                isInteract = false;
                //Debug.Log("explainNum " + explainNum);
            }// isObject 체크 풀어서 NPCData 저장 -> isInteract 한 번 된 후로는 BoxCollider2D 비활성화해서 상호작용키 없이도 이동 가능하도록

            ChangeDialogueFileName("Stage2_1F_dialogue");
            //dialogueFileName = "Stage2_1F_dialogue";
            if (InventoryUI.instance.HasItem(20102))
            {
                //Debug.Log("HasItem 20102");
                ChangeExplainNum("11");
                if (isInteract)
                {
                    gameObject.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
            else ChangeExplainNum("10");
        }
        else if (SceneManager.GetActiveScene().name == "Library_B1F") // 열람실
        {
            if (!InventoryUI.instance.HasItem(20101))   // 열쇠 미소지 시 !isInteract(최초 상호작용 위해)
            {
                //Debug.Log("미소지");
                isInteract = false;
            }
            else // 열쇠 소지 후 최초 상호작용 시 isInteract
            {
                //Debug.Log("소지");
                ChangeExplainNum("2");
                if (isInteract)
                {
                    //gameObject.GetComponent<BoxCollider2D>().enabled = false;
                }
            }
        }
    }
}
