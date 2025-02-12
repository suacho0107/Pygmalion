using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkSheet : MonoBehaviour
{
    [SerializeField] PlayerPosition playerPos;
    [SerializeField] Vector3 spawnPos;

    public void SpawnCompany()
    {
        UIManager.u_instance.SetUIState(Define.UI.UIState.Ready);

        playerPos.nextPosition = spawnPos;
        playerPos.isChecked = true;

        SceneManager.LoadScene("Company_Office");
    }
}
