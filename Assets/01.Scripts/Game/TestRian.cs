using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRian : MonoBehaviour
{
    void Update()
    {
        if (UIManager.u_instance.isTutorialRian2)
        {
            gameObject.SetActive(false);
        }
    }
}
