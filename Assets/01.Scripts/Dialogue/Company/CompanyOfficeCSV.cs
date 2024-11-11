using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyOfficeCSV : MonoBehaviour
{
    public NPC[] npcs;  // ���� NPC ��ũ��Ʈ�� �迭�� ����

    private void Start()
    {
        if (npcs.Length > 0)
        {
            // ä�� UI
            npcs[0].dialogueFiles = new string[] { "request1_dialogue" };
            npcs[0].selectFiles = new string[] { "request1_select" };
            npcs[0].currentIndex = 0;
            npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
            npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
        }
    }
}