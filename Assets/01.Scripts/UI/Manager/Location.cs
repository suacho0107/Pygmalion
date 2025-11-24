using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Location : MonoBehaviour
{
    [SerializeField] Text    location;

    private void Start()
    {
        UpdateStartUI();
    }

    private void UpdateStartUI()
    {
        if (UIManager.u_instance == null)
            return; 

        location.text = UIManager.u_instance.locationList[UIManager.u_instance.stageIndex];
    }

}
