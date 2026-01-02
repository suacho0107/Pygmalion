using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    public int currency = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadCurrency();
    }

    private void Update()
    {
        #region Test : Reset curreny
        if (Input.GetKeyDown(KeyCode.Z))
        {
            PlayerPrefs.SetInt("Currency", 0);
        }
        #endregion
    }

    public void AddCurrency(int _amount)
    {
        currency += _amount;
        SaveCurrency();
    }

    public void SaveCurrency()
    {
        PlayerPrefs.SetInt("Currency", currency);
    }

    public void LoadCurrency()
    {
        currency = PlayerPrefs.GetInt("Currency");
    }

    public int GetCurrency()
    {
        return currency;
    }
}
