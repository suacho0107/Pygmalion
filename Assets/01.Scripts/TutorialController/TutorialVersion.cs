using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class TutorialVersion : MonoBehaviour
{
    private void Start()
    {
        var u_instance = UIManager.u_instance;

        var controller0 = transform.GetChild(0).gameObject;
        var controller1 = transform.GetChild(1).gameObject;
        GameObject controller2 = null;
        if (transform.childCount > 2)
            controller2 = transform.GetChild(2).gameObject;

        if (u_instance != null && u_instance.isRespawn)
        {
            controller0.SetActive(false);
            controller1.SetActive(false);
            if (controller2 != null)
                controller2.SetActive(true);

            u_instance.isRespawn = false;
            return;
        }

        if (!u_instance.isRespawn)
        {
            bool isCorrect = Check_WorkStage();

            if (isCorrect)
            {
                controller1.SetActive(false);
                if (controller2)
                    controller2.SetActive(false);
            }
            else
            {
                controller0.SetActive(false);
                if (controller2)
                    controller2.SetActive(false);
            }
        }
        else
        {
            controller1.SetActive(false);
            if (controller2)
                controller2.SetActive(false);
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
                UIManager.u_instance.isRespawn = false;
                break;

            case 1: // 도서관
                curStageName = "Library_1F";
                UIManager.u_instance.isRespawn = false;
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
