using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatueScore : MonoBehaviour
{
    [SerializeField] GameObject statueScorePanel;
    [SerializeField] Text statueScoreText;

    public int statueCount = 0;
    public int destroyedCount = 0;
    public int checkedCount = 0;
    public int fightCount = 0;

    private void Start()
    {
        // 테스트 초기화
        // PlayerPrefs.DeleteAll();

        statueCount = PlayerPrefs.GetInt("StatueCount", statueCount);
        destroyedCount = PlayerPrefs.GetInt("destroyedCount", destroyedCount);
        checkedCount = PlayerPrefs.GetInt("checkedCount", checkedCount);
        fightCount = PlayerPrefs.GetInt("fightCount", fightCount);
        UpdateScore();
    }

    void UpdateScore()
    {
        statueScoreText.text = "▶ 점검한 조각상: " + statueCount + " / 6";
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("StatueCount", statueCount);
        PlayerPrefs.SetInt("destroyedCount", destroyedCount);
        PlayerPrefs.SetInt("checkedCount", checkedCount);
        PlayerPrefs.SetInt("fightCount", fightCount);
        PlayerPrefs.Save(); // 저장 강제 적용
        UpdateScore();
    }
}