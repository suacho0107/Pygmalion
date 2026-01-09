using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Define;

public class WorkSheet : MonoBehaviour
{
    public enum GRADE { A, B, C, D, F, END }

    [SerializeField] GameObject     ToggleObject;
    [SerializeField] PlayerPosition playerPos;
    [SerializeField] Vector3        spawnPos;
    [SerializeField] Text           _check;
    [SerializeField] Text           _fight;
    [SerializeField] Text           _destroy;
    [SerializeField] Text           _efficiency;

    [SerializeField] Image[]        npcSigns;

    private int     statueCount;
    private int     fightCount;
    private int     destroyedCount;
    private int     checkedCount;

    private GRADE   result;
    private string  efficiency;
    public int      currency;
    private bool    isNext = false;

    DataManager dataManager = null;

    private void Start()
    {
        dataManager = DataManager.Instance;

        UpdateEndUI();
    }

    private GRADE SetData()
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
        GRADE grade = GRADE.END;

        if (1 > totalGrade)
            grade = GRADE.A;

        else if (1 <= totalGrade && 1.3 > totalGrade)
            grade = GRADE.B;

        else if (1.3 <= totalGrade && 1.7 > totalGrade)
            grade = GRADE.C;

        else if (1.7 < totalGrade && 2.2 > totalGrade)
            grade = GRADE.D;

        else if (2.2 <= totalGrade)
            grade = GRADE.F;

        else
            Debug.Log($"평가등급 계산 오류! totalGrade : {totalGrade}");

        return grade;
    }

    private void UpdateEndUI()
    {
        ToggleObject.SetActive(false);

        statueCount = PlayerPrefs.GetInt("StatueCount");

        // TODO: 스테이지별 조각상 개수와 적 조각상 개수를 업무 효율 계산식의 매개변수로 전달해줘야 함, SetData 함수의 매개변수로 전달
        result = SetData();

        _check.text         = statueCount.ToString();
        //_fight.text         = fightCount.ToString();
        _destroy.text       = destroyedCount.ToString();
        _efficiency.text    = efficiency;

        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Museum_Lobby") _fight.text = "1";
        else if (sceneName == "Library_1F") _fight.text = "2";

        #region 평가 등급 출력, 성과급 지급
        switch (result)
        {
            case GRADE.A:
                gameObject.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
                dataManager.AddCurrency(300);
                break;

            case GRADE.B:
                gameObject.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                dataManager.AddCurrency(200);
                break;

            case GRADE.C:
                gameObject.transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                dataManager.AddCurrency(150);
                break;

            case GRADE.D:
                gameObject.transform.GetChild(1).GetChild(3).gameObject.SetActive(true);
                dataManager.AddCurrency(100);
                break;

            case GRADE.F:
                gameObject.transform.GetChild(1).GetChild(4).gameObject.SetActive(true);
                dataManager.AddCurrency(50);
                break;

            default:
                break;
        }
        #endregion

        Write_Sign();

        StartCoroutine(ToggleOn());                             
    }

    void Write_Sign()
    {
        int stageIndex = UIManager.u_instance.stageIndex;
        Image signImage = npcSigns[stageIndex];
        Image playersignImage = npcSigns[2]; // 마지막 번째 : 플레이어 사인

        var color = signImage.color;
        color.a = 1f;
        signImage.color = color;

        // FillMethod와 fillOrigin을 설정 (수평, 왼쪽에서 시작)
        playersignImage.fillMethod = Image.FillMethod.Horizontal;
        playersignImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        playersignImage.type = Image.Type.Filled;
        playersignImage.fillAmount = 0f;

        // 애니메이션 코루틴 시작
        StartCoroutine(FillSignImage(playersignImage, 1f, 0.8f)); // 0.8초 동안 fillAmount 1까지
    }

    IEnumerator FillSignImage(Image image, float targetFill, float duration)
    {
        float startFill = image.fillAmount;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            yield return null;
        }
        image.fillAmount = targetFill;
    }

    IEnumerator ToggleOn()
    {
        yield return new WaitForSeconds(1.5f);

        ToggleObject.SetActive(true);
        isNext = true;
    }

    private void Update()
    {
        if (isNext && Input.GetKeyDown(KeyCode.Space))
            SpawnCompany();
    }

    public void SpawnCompany()
    {
        UIManager.u_instance.Set_UIState(Define.UI.UIState.Ready);

        playerPos.nextPosition = spawnPos;
        playerPos.isChecked = true;

        SceneManager.LoadScene("Monologue_success");

        #region refactor
        //Stage.StageState stageStage = UIManager.u_instance.Get_StageState();

        //switch (stageStage)
        //{
        //    case Stage.StageState.Museum:
        //        SceneManager.LoadScene("WorktoCompany");
        //        break;

        //    case Stage.StageState.Library:
        //        SceneManager.LoadScene("Company_Office");
        //        break;

        //    case Stage.StageState.Park:
        //        SceneManager.LoadScene("Company_Office");
        //        break;

        //    case Stage.StageState.CityHall:
        //        SceneManager.LoadScene("Company_Office");
        //        break;

        //    case Stage.StageState.BroadcastStation:
        //        SceneManager.LoadScene("Company_Office");
        //        break;

        //    case Stage.StageState.Hospital:
        //        SceneManager.LoadScene("Company_Office");
        //        break;
        //    default:
        //        break;
        //}
        #endregion
    }
}
