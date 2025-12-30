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
        audioSource.Stop();
        audioSource.loop = true;
        audioSource.volume = 0.4f;
        //audioSource.volume = 0f;
        audioSource.time = 0;
        audioSource.Play();
    }

    public IEnumerator FadeOutBGM(float _duration)
    {
        float time = 0f;
        while (time < _duration)
        {
            float t = time / _duration;

            float volume = Mathf.Lerp(0.4f, 0f, t);
            audioSource.volume = volume;

            time += Time.deltaTime;
            yield return null;
        }

        audioSource.volume = 0f;
    }
}
