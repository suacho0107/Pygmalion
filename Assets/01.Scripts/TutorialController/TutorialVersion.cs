using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialVersion : MonoBehaviour
{
    private void Start()
    {
        bool isCorrect = Check_WorkStage();

        var controller0 = transform.GetChild(0).gameObject;
        var controller1 = transform.GetChild(1).gameObject;

        if (isCorrect)
        {
            controller1.SetActive(false);
        }
        else
        {
            controller0.SetActive(false);
        }
    }

    bool Check_WorkStage()
    {
        string curSceneName = SceneManager.GetActiveScene().name;
        string curStageName;

        int stageState = UIManager.u_instance.Get_StageIndex();
        switch (stageState)
        {
            case 0: // 미술관
                curStageName = "Museum_Lobby";
                break;

            case 1: // 도서관
                curStageName = "Library_1F";
                break;

            default:
                curStageName = "Default";
                break;
        }

        if (curSceneName == curStageName)
        {
            return true;
        }
        else
            return false;

    }
}
