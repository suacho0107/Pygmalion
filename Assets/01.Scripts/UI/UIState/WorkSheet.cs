using System;
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

    private int     statueCount;
    private int     fightCount;
    private int     destroyedCount;
    private int     checkedCount;

    private int     result;
    private string  efficiency;
    public int      currency;

    DataManager dataManager = null;

    private void Awake()
    {
        dataManager = DataManager.Instance;

        #region Test
        //PlayerPrefs.SetInt("StatueCount", 6);
        //PlayerPrefs.SetInt("fightCount", 2);
        //PlayerPrefs.SetInt("destroyedCount", 1);
        //PlayerPrefs.SetInt("checkedCount", 16);
        #endregion

        UpdateEndUI();
    }

    private int SetData()
    {
        int totalCount = 6;
        int enemyCount = 1;     // TODO: totalCount(조각상 개수), enemyCount(적 개수) 모두 매개 변수화

        statueCount     = PlayerPrefs.GetInt("StatueCount");
        fightCount      = PlayerPrefs.GetInt("fightCount");
        destroyedCount  = PlayerPrefs.GetInt("destroyedCount");
        checkedCount    = PlayerPrefs.GetInt("checkedCount");

        #region 업무 효율 계산식
        double checkEff     = Math.Round((double)checkedCount / (totalCount * 2.0), 4);
        double battleEff    = Math.Round((double)fightCount / (double)enemyCount, 4);
        double workEff      = Math.Round((checkEff + battleEff) / 2.0, 4);

        //Debug.Log($"( checkEff : {checkEff} + battleEff : {battleEff} ) / 2 = workEff : {workEff}");
        if (1 >= workEff)
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

        #endregion

        #region 파손한 조각상
        double accuracy = Math.Round((double)destroyedCount / (double)(totalCount - enemyCount), 4);
        #endregion

        double totalGrade = Math.Round(workEff + accuracy, 4);
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

    private void UpdateEndUI()
    {
        statueCount = PlayerPrefs.GetInt("StatueCount");

        if (dataManager == null || statueCount < 6)
            return;

        // TODO: 스테이지별 조각상 개수와 적 조각상 개수를 업무 효율 계산식의 매개변수로 전달해줘야 함, SetData 함수의 매개변수로 전달
        result = SetData();

        _check.text = statueCount.ToString();
        _fight.text = fightCount.ToString();
        _destroy.text = destroyedCount.ToString();
        _efficiency.text = efficiency;

        //Debug.Log($"UpdateEndUI : {efficiency}");

        switch (result)
        {
            case 65:
                gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                dataManager.AddCurrency(300);
                break;

            case 66:
                gameObject.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                dataManager.AddCurrency(200);
                break;

            case 67:
                gameObject.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                dataManager.AddCurrency(150);
                break;

            case 68:
                gameObject.transform.GetChild(1).GetChild(3).gameObject.SetActive(true);
                dataManager.AddCurrency(100);
                break;

            case 70:
                gameObject.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);
                dataManager.AddCurrency(50);
                break;

            default:
                break;
        }
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
