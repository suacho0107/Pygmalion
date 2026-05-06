using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveUI : MonoBehaviour
{
    [SerializeField] GameObject SLPanel;

    private Scene currentScene;

    bool isPanelOn = false;

    void Start()
    {
        SLPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPanelOn = !isPanelOn;
            SLPanel.SetActive(isPanelOn);
        }        
    }

    public void OnSaveData()
    {
        // Save
        currentScene = SceneManager.GetActiveScene();
    }
}
