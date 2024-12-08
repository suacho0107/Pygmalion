using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    [SerializeField] PlayerPosition playerPos;

    void Start()
    {
        playerPos.currentPosition = new Vector3(3f, -0.5f, 0f);
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            SceneManager.LoadScene("Company_Lobby");
        }
    }
}
