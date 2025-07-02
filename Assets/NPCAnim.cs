using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCAnim : MonoBehaviour
{
    Animator anim;

    public GameObject side;
    public GameObject tremble;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if(anim != null)
        {
            side.SetActive(true);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Library_2F")
        {
            StageNPC stageNPC = GetComponent<StageNPC>();
            if (!stageNPC.isInteract)
            {
                side.SetActive(false);
                tremble.SetActive(true);
            }
            
            if (anim.GetBool("melEnd"))
            {
                side.SetActive(true);
                tremble.SetActive(false);
            }
        }
    }
}
