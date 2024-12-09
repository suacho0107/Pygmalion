using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatueScore : MonoBehaviour
{
    [SerializeField] GameObject statueScorePanel;
    [SerializeField] Text statueScoreText;

    public int statueCount = 0;
    public int destroyedCount = 0;
    public int checkedCount = 0;
    public int fightCount = 0;

    public int checkCount = 0;

    private void Start()
    {
        // �׽�Ʈ �ʱ�ȭ
        //PlayerPrefs.DeleteAll();

        statueCount = PlayerPrefs.GetInt("StatueCount", statueCount);
        destroyedCount = PlayerPrefs.GetInt("destroyedCount", destroyedCount);
        checkedCount = PlayerPrefs.GetInt("checkedCount", checkedCount);
        fightCount = PlayerPrefs.GetInt("fightCount", fightCount);

        checkCount = PlayerPrefs.GetInt("checkCount", checkCount);
        UpdateScore();
    }

    void UpdateScore()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.StartsWith("Museum"))
        {
            statueScoreText.text = "�� ������ ������: " + statueCount + " / 6";

            //statueCount = UIManager.u_instance.checkCount_test;
        }
        //else if(sceneName.StartsWith("Library"))
        //{
        //    statueScoreText.text = "�� ������ ������: " + statueCount + " / 5";
        //}
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("StatueCount", statueCount);
        PlayerPrefs.SetInt("destroyedCount", destroyedCount);
        PlayerPrefs.SetInt("checkedCount", checkedCount);
        PlayerPrefs.SetInt("fightCount", fightCount);
        PlayerPrefs.SetInt("checkCount", checkCount);

        PlayerPrefs.Save(); // ���� ���� ����
        UpdateScore();
    }
}