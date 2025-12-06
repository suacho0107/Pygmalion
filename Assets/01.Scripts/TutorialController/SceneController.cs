using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    public Vector3 nextPos { get; private set; }
    public string nextSceneName { get; private set; } = "";

    int curStageIndex = 0;

    void Update()
    {
        curStageIndex = UIManager.u_instance.stageIndex;

        switch (curStageIndex)
        {
            case 0:
                nextSceneName = "Museum_Lobby";
                nextPos = new Vector3(2, 7.3f, 0);
                break;

            case 1:
                nextSceneName = "Library_1F";
                nextPos = new Vector3(0, 0, 0);
                break;

            case 2:
                nextSceneName = "Park";
                nextPos = new Vector3(0, 0, 0);
                break;

            case 3:
                nextSceneName = "CityHall_Lobby";
                nextPos = new Vector3(0, 0, 0);
                break;

            case 4:
                nextSceneName = "Broadcast_1F";
                nextPos = new Vector3(0, 0, 0);
                break;

            case 5:
                nextSceneName = "Hospital_1F";
                nextPos = new Vector3(0, 0, 0);
                break;

            default:
                break;
        }
    }
}
