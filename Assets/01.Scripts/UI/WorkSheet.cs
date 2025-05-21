using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkSheet : MonoBehaviour
{
    [SerializeField] PlayerPosition playerPos;
    [SerializeField] Vector3 spawnPos;

    // Work 상태에 띄워지는 StatueScore 오브젝트의 점수 정보 저장
    // End 상태로 변경되기 전 해당 오브젝트의 값을 복사해와야함

    public void SpawnCompany()
    {
        UIManager.u_instance.SetUIState(Define.UI.UIState.Ready);

        playerPos.nextPosition = spawnPos;
        playerPos.isChecked = true;

        SceneManager.LoadScene("Company_Office");
    }
}
