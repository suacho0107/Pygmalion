using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatueScore : MonoBehaviour
{
    [SerializeField] GameObject statueScorePanel;
    [SerializeField] Text statueScoreText;

    public int statueCount = 0;

    private void Start()
    {
        //statueScorePanel.SetActive(false);
    }

    private void Update()
    {
        int[] counts = { 0, 1, 2, 3, 4, 5, 6 };
        foreach(int i in counts)
        {
            if (statueCount == i)
            {
                //statueScorePanel.SetActive(true);
                statueScoreText.text = "▶ 점검한 조각상: " + i + " / 6";
            }
        }
    }
}
