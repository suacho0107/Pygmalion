using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WorkSheet : MonoBehaviour
{
    [SerializeField] PlayerPosition playerPos;
    [SerializeField] Vector3        spawnPos;
    [SerializeField] Text           _check;
    [SerializeField] Text           _fight;
    [SerializeField] Text           _destroy;
    [SerializeField] Text           _efficiency;

    private int     totalCount;
    private int     enemyCount;
    private int     statueCount;
    private int     fightCount;
    private int     destroyedCount;
    private int     result;

    private string  efficiency;

    public int currency;

    private void Awake()
    {
        UpdateEndUI();
    }

    private void UpdateEndUI()
    {
        result = SetData();

        _check.text = statueCount.ToString();
        _fight.text = fightCount.ToString();
        _destroy.text = destroyedCount.ToString();
        _efficiency.text = efficiency;

        Debug.Log($"UpdateEndUI : {efficiency}");

        switch (result)
        {
            case 65:
                gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                DataManager.Instance.AddCurrency(300);
                break;

            case 66:
                gameObject.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                DataManager.Instance.AddCurrency(200);
                break;

            case 67:
                gameObject.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                DataManager.Instance.AddCurrency(150);
                break;

            case 68:
                gameObject.transform.GetChild(1).GetChild(3).gameObject.SetActive(true);
                DataManager.Instance.AddCurrency(100);
                break;

            case 70:
                gameObject.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);
                DataManager.Instance.AddCurrency(50);
                break;

            default:
                break;
        }
    }

    private int SetData()
    {
        totalCount = 6;
        enemyCount = 1;

        statueCount = PlayerPrefs.GetInt("StatueCount");
        fightCount = PlayerPrefs.GetInt("fightCount");
        destroyedCount = PlayerPrefs.GetInt("destroyedCount");

        float checkEff = statueCount / (totalCount * 2);
        float battleEff = fightCount / enemyCount;
        float workEff = checkEff + battleEff;

        if (1 == workEff)
            efficiency = "탁월";

        else if (1 < workEff && 1.5 >= workEff)
            efficiency = "우수";

        else if (1.5 < workEff && 2 >= workEff)
            efficiency = "충족";

        else if (2 < workEff && 2.5 > workEff)
            efficiency = "개선 필요";

        else if (2.5 <= workEff)
            efficiency = "미흡";

        else
            Debug.Log($"업무 효율 계산 오류! workEff : {workEff}");

        Debug.Log($"SetData : {efficiency}");

        float accuracy = destroyedCount / (totalCount - enemyCount);
        float totalGrade = workEff + accuracy;
        int grade = 0;

        if (1 > totalGrade)
            grade = 65;

        else if (1 <= totalGrade && 1.3 > totalGrade)
            grade = 66;

        else if (1.3 <= totalGrade && 1.7 > totalGrade)
            grade = 67;

        else if (1.7 < totalGrade && 2.2 > totalGrade)
            grade = 68;

        else if (2.2 <= totalGrade)
            grade = 70;

        else
            Debug.Log($"평가등급 계산 오류! totalGrade : {totalGrade}");

        return grade;
    }

    public void SpawnCompany()
    {
        UIManager.u_instance.SetUIState(Define.UI.UIState.Ready);
        // location 등록
        UIManager.u_instance.stageIndex++;

        playerPos.nextPosition = spawnPos;
        playerPos.isChecked = true;

        SceneManager.LoadScene("Company_Office");
    }
}
