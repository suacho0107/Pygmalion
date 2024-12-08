using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue1 : MonoBehaviour
{
    public NPC npc;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        npc.StartDialogue();
        PlayerPrefs.SetInt("Start2", 1); // ���� ��� ���� ���� ��ȣ
        PlayerPrefs.SetInt("Start1", 0); // ���� ��� ����
        PlayerPrefs.Save();
    }
}
