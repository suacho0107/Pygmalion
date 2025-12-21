using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Assign_Currency : MonoBehaviour
{
    [SerializeField] Text currency_text;

    void Update()
    {
        currency_text.text = DataManager.Instance.GetCurrency().ToString();
    }
}
