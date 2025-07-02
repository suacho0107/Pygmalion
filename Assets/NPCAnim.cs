using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnim : MonoBehaviour
{
    Animator anim;

    public GameObject side;

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

    }
}
