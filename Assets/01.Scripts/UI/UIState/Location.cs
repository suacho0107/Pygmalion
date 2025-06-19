using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Location : MonoBehaviour
{
    [SerializeField] Text    location;

    private void Awake()
    {
        UpdateStartUI();
    }

    private void UpdateStartUI()
    {
        location.text = UIManager.u_instance.locations[UIManager.u_instance.stageIndex];
    }

}
