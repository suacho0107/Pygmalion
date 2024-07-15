using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public bool activeTP = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "TpZone")
        {
            activeTP = true;
            Debug.Log("activeTP True");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        activeTP = false;
        Debug.Log("activeTP False");
    }

    void Update()
    {
        if (activeTP == true)
        {
            Debug.Log("TP 가능 구역");
            if (Input.GetKeyDown(KeyCode.F))
            {
                SceneManager.LoadScene("Demo_minjoo");
            }
        }

    }
}
