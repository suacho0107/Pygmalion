using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBGM : MonoBehaviour
{
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        PlayBGM();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayBGM()
    {
        Debug.Log("PlayBGM()");

        audioSource.Stop();
        audioSource.loop = true;
        audioSource.volume = 0.4f;
        //audioSource.volume = 0f;
        audioSource.time = 0;
        audioSource.Play();
    }
}
