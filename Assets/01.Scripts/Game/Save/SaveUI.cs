using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveUI : MonoBehaviour
{
    [SerializeField] GameObject SavePanel;

    private Scene currentScene;

    bool isPanelOn = false;

    void Start()
    {
        SavePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPanelOn = !isPanelOn;
            SavePanel.SetActive(isPanelOn);
        }        
    }

    public void OnSaveData()
    {
        SaveManager.s_instance.SaveData();
    }
}
