using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FightCountTest : MonoBehaviour
{
    NPC npc;

    public void WinTest()
    {
        SceneManager.LoadScene("Museum_Lobby");
        npc.isFin = true;
    }

    public void LoseTest()
    {
        SceneManager.LoadScene("Museum_Lobby");
    }
}
