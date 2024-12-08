using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue1 : MonoBehaviour
{
    public NPC npc;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        npc.StartDialogue();
        PlayerPrefs.SetInt("Start2", 1); // 다음 대사 시작 가능 신호
        PlayerPrefs.SetInt("Start1", 0); // 현재 대사 종료
        PlayerPrefs.Save();
    }
}
