using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class museumExhibit1CSV : MonoBehaviour
{
    public NPC[] npcs;  // ���� NPC ��ũ��Ʈ�� �迭�� ����

    private void Start()
    {
        if (npcs.Length > 0)
        {
            // ������ �Ǻ� csv(�׽�Ʈ)
            npcs[0].dialogueFiles = new string[] { "judge1_dialogue", "judge1_dialogue", "battleDialogue1" };
            npcs[0].selectFiles = new string[] { "judge1_select", "judge2_select", "" };
            npcs[0].currentIndex = 0;
            npcs[0].dialogueFileName = npcs[0].dialogueFiles[npcs[0].currentIndex];
            npcs[0].selectFileName = npcs[0].selectFiles[npcs[0].currentIndex];
        }
    }
}
